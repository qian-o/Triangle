using System.Drawing;
using System.Numerics;
using Hexa.NET.ImGui;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Triangle.Core.Contracts;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Structs;

namespace Triangle.Core.Controllers;

public unsafe class ImGuiController : Disposable
{
    private static readonly Key[] _keyEnumArr = (Key[])Enum.GetValues(typeof(Key));

    private readonly TrContext _trContext;
    private readonly GL _gl;
    private readonly IView _view;
    private readonly IInputContext _input;
    private readonly List<char> _pressedChars = [];

    private bool _frameBegun;
    private IKeyboard _keyboard = null!;

    private int _windowWidth;
    private int _windowHeight;

    private int _attribLocationTex;
    private int _attribLocationProjMtx;
    private int _attribLocationVtxPos;
    private int _attribLocationVtxUV;
    private int _attribLocationVtxColor;

    private uint _vboHandle;
    private uint _elementsHandle;
    private uint _vertexArrayObject;

    private TrTexture _fontTexture = null!;
    private TrRenderPipeline _pipeline = null!;

    private ImGuiContextPtr _context;

    /// <summary>
    /// Constructs a new ImGuiController.
    /// </summary>
    /// <param name="trContext">trContext</param>
    /// <param name="view">view</param>
    /// <param name="input">input</param>
    public ImGuiController(TrContext trContext, IView view, IInputContext input)
        : this(trContext, view, input, null, null)
    {
    }

    /// <summary>
    /// Constructs a new ImGuiController with font configuration.
    /// </summary>
    /// <param name="trContext">trContext</param>
    /// <param name="view">view</param>
    /// <param name="input">input</param>
    /// <param name="imGuiFontConfig">imGuiFontConfig</param>
    public ImGuiController(TrContext trContext, IView view, IInputContext input, ImGuiFontConfig imGuiFontConfig)
        : this(trContext, view, input, imGuiFontConfig, null)
    {
    }

    /// <summary>
    /// Constructs a new ImGuiController with an onConfigureIO Action.
    /// </summary>
    /// <param name="trContext">trContext</param>
    /// <param name="view">view</param>
    /// <param name="input">input</param>
    /// <param name="onConfigureIO">onConfigureIO</param>
    public ImGuiController(TrContext trContext, IView view, IInputContext input, Action onConfigureIO)
        : this(trContext, view, input, null, onConfigureIO)
    {
    }

    /// <summary>
    /// Constructs a new ImGuiController with font configuration and onConfigure Action.
    /// </summary>
    /// <param name="trContext">trContext</param>
    /// <param name="view">view</param>
    /// <param name="input">input</param>
    /// <param name="imGuiFontConfig">imGuiFontConfig</param>
    /// <param name="onConfigureIO">onConfigureIO</param>
    public ImGuiController(TrContext trContext, IView view, IInputContext input, ImGuiFontConfig? imGuiFontConfig = null, Action? onConfigureIO = null)
    {
        _trContext = trContext;
        _gl = trContext.GL;
        _view = view;
        _input = input;
        _windowWidth = view.Size.X;
        _windowHeight = view.Size.Y;
        _context = ImGui.CreateContext();

        ImGui.SetCurrentContext(_context);
        ImGui.StyleColorsDark();

        ImGuiIOPtr iO = ImGui.GetIO();

        if (imGuiFontConfig.HasValue)
        {
            nint glyph_ranges = imGuiFontConfig.Value.GetGlyphRange?.Invoke(iO) ?? 0;
            iO.Fonts.AddFontFromFileTTF(imGuiFontConfig.Value.FontPath, imGuiFontConfig.Value.FontSize, null, (char*)glyph_ranges);
        }

        onConfigureIO?.Invoke();
        iO.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

        CreateDeviceResources();
        SetKeyMappings();
        SetPerFrameImGuiData(1f / 60f);
        BeginFrame();
    }

    public void MakeCurrent()
    {
        ImGui.SetCurrentContext(_context);
    }

    private void BeginFrame()
    {
        ImGui.NewFrame();

        _frameBegun = true;
        _keyboard = _input.Keyboards[0];

        _view.Resize += WindowResized;
        _keyboard.KeyChar += OnKeyChar;
    }

    private void OnKeyChar(IKeyboard arg1, char arg2)
    {
        _pressedChars.Add(arg2);
    }

    private void WindowResized(Vector2D<int> size)
    {
        _windowWidth = size.X;
        _windowHeight = size.Y;
    }

    /// <summary>
    /// Renders the ImGui draw list data. This method requires a GraphicsDevice because 
    /// it may create new DeviceBuffers if the size of vertex or index data has increased
    /// beyond the capacity of the existing buffers. A CommandList is needed to submit
    /// drawing and resource update commands.
    /// </summary>
    public void Render()
    {
        if (_frameBegun)
        {
            ImGuiContextPtr currentContext = ImGui.GetCurrentContext();

            if (currentContext != _context)
            {
                ImGui.SetCurrentContext(_context);
            }

            _frameBegun = false;

            ImGui.Render();
            RenderImDrawData(ImGui.GetDrawData());

            if (currentContext != _context)
            {
                ImGui.SetCurrentContext(currentContext);
            }
        }
    }

    /// <summary>
    /// Updates ImGui input and IO configuration state.
    /// </summary>
    /// <param name="deltaSeconds">deltaSeconds</param>
    public void Update(float deltaSeconds)
    {
        ImGuiContextPtr currentContext = ImGui.GetCurrentContext();
        if (currentContext != _context)
        {
            ImGui.SetCurrentContext(_context);
        }

        if (_frameBegun)
        {
            ImGui.Render();
        }

        SetPerFrameImGuiData(deltaSeconds);
        UpdateImGuiInput();

        _frameBegun = true;

        ImGui.NewFrame();

        if (currentContext != _context)
        {
            ImGui.SetCurrentContext(currentContext);
        }
    }

    /// <summary>
    /// Sets per-frame data based on the associated window. This is called by Update(float).
    /// </summary>
    /// <param name="deltaSeconds"></param>
    private void SetPerFrameImGuiData(float deltaSeconds)
    {
        ImGuiIOPtr iO = ImGui.GetIO();

        iO.DisplaySize = new Vector2(_windowWidth, _windowHeight);

        if (_windowWidth > 0 && _windowHeight > 0)
        {
            iO.DisplayFramebufferScale = new Vector2(_view.FramebufferSize.X / _windowWidth, _view.FramebufferSize.Y / _windowHeight);
        }

        iO.DeltaTime = deltaSeconds;
    }

    private void UpdateImGuiInput()
    {
        ImGuiIOPtr iO = ImGui.GetIO();

        IMouse mouse = _input.Mice[0];
        IKeyboard keyboard = _input.Keyboards[0];

        iO.MouseDown[0] = mouse.IsButtonPressed(MouseButton.Left);
        iO.MouseDown[1] = mouse.IsButtonPressed(MouseButton.Right);
        iO.MouseDown[2] = mouse.IsButtonPressed(MouseButton.Middle);

        Point point = new((int)mouse.Position.X, (int)mouse.Position.Y);
        iO.MousePos = new Vector2(point.X, point.Y);

        ScrollWheel scrollWheel = mouse.ScrollWheels[0];
        iO.MouseWheel = scrollWheel.Y;
        iO.MouseWheelH = scrollWheel.X;

        Key[] array = _keyEnumArr;
        foreach (Key key in array)
        {
            if (key != Key.Unknown)
            {
                iO.KeysDown[(int)key] = keyboard.IsKeyPressed(key);
            }
        }

        foreach (char pressedChar in _pressedChars)
        {
            iO.AddInputCharacter(pressedChar);
        }

        _pressedChars.Clear();
        iO.KeyCtrl = keyboard.IsKeyPressed(Key.ControlLeft) || keyboard.IsKeyPressed(Key.ControlRight);
        iO.KeyAlt = keyboard.IsKeyPressed(Key.AltLeft) || keyboard.IsKeyPressed(Key.AltRight);
        iO.KeyShift = keyboard.IsKeyPressed(Key.ShiftLeft) || keyboard.IsKeyPressed(Key.ShiftRight);
        iO.KeySuper = keyboard.IsKeyPressed(Key.SuperLeft) || keyboard.IsKeyPressed(Key.SuperRight);
    }

    private static void SetKeyMappings()
    {
        ImGuiIOPtr iO = ImGui.GetIO();

        iO.KeyMap[512] = 258;
        iO.KeyMap[513] = 263;
        iO.KeyMap[514] = 262;
        iO.KeyMap[515] = 265;
        iO.KeyMap[516] = 264;
        iO.KeyMap[517] = 266;
        iO.KeyMap[518] = 267;
        iO.KeyMap[519] = 268;
        iO.KeyMap[520] = 269;
        iO.KeyMap[522] = 261;
        iO.KeyMap[523] = 259;
        iO.KeyMap[525] = 257;
        iO.KeyMap[526] = 256;
        iO.KeyMap[546] = 65;
        iO.KeyMap[548] = 67;
        iO.KeyMap[567] = 86;
        iO.KeyMap[569] = 88;
        iO.KeyMap[570] = 89;
        iO.KeyMap[571] = 90;
    }

    private unsafe void SetupRenderState(ImDrawDataPtr drawDataPtr, int framebufferWidth, int framebufferHeight)
    {
        float x = drawDataPtr.DisplayPos.X;
        float num = drawDataPtr.DisplayPos.X + drawDataPtr.DisplaySize.X;
        float y = drawDataPtr.DisplayPos.Y;
        float num2 = drawDataPtr.DisplayPos.Y + drawDataPtr.DisplaySize.Y;

        Span<float> span =
        [
            2f / (num - x),
            0f,
            0f,
            0f,
            0f,
            2f / (y - num2),
            0f,
            0f,
            0f,
            0f,
            -1f,
            0f,
            (num + x) / (x - num),
            (y + num2) / (num2 - y),
            0f,
            1f
        ];

        _pipeline.Bind();
        _gl.Uniform1(_attribLocationTex, 0);
        _gl.UniformMatrix4(_attribLocationProjMtx, 1u, transpose: false, span);
        _gl.BindSampler(0u, 0u);

        _vertexArrayObject = _gl.GenVertexArray();
        _gl.BindVertexArray(_vertexArrayObject);
        _gl.BindBuffer(GLEnum.ArrayBuffer, _vboHandle);
        _gl.BindBuffer(GLEnum.ElementArrayBuffer, _elementsHandle);

        _gl.EnableVertexAttribArray((uint)_attribLocationVtxPos);
        _gl.EnableVertexAttribArray((uint)_attribLocationVtxUV);
        _gl.EnableVertexAttribArray((uint)_attribLocationVtxColor);

        _gl.VertexAttribPointer((uint)_attribLocationVtxPos, 2, GLEnum.Float, normalized: false, (uint)sizeof(ImDrawVert), null);
        _gl.VertexAttribPointer((uint)_attribLocationVtxUV, 2, GLEnum.Float, normalized: false, (uint)sizeof(ImDrawVert), (void*)8);
        _gl.VertexAttribPointer((uint)_attribLocationVtxColor, 4, GLEnum.UnsignedByte, normalized: true, (uint)sizeof(ImDrawVert), (void*)16);
    }

    private unsafe void RenderImDrawData(ImDrawDataPtr drawDataPtr)
    {
        int num = (int)(drawDataPtr.DisplaySize.X * drawDataPtr.FramebufferScale.X);
        int num2 = (int)(drawDataPtr.DisplaySize.Y * drawDataPtr.FramebufferScale.Y);
        if (num <= 0 || num2 <= 0)
        {
            return;
        }

        _gl.GetInteger(GLEnum.ActiveTexture, out var data);
        _gl.ActiveTexture(GLEnum.Texture0);
        _gl.GetInteger(GLEnum.CurrentProgram, out var data2);
        _gl.GetInteger(GLEnum.TextureBinding2D, out var data3);
        _gl.GetInteger(GLEnum.SamplerBinding, out var data4);
        _gl.GetInteger(GLEnum.ArrayBufferBinding, out var data5);
        _gl.GetInteger(GLEnum.VertexArrayBinding, out var data6);
        Span<int> data7 = stackalloc int[2];
        _gl.GetInteger(GLEnum.PolygonMode, data7);
        Span<int> data8 = stackalloc int[4];
        _gl.GetInteger(GLEnum.ScissorBox, data8);
        _gl.GetInteger(GLEnum.BlendSrcRgb, out var data9);
        _gl.GetInteger(GLEnum.BlendDstRgb, out var data10);
        _gl.GetInteger(GLEnum.BlendSrcAlpha, out var data11);
        _gl.GetInteger(GLEnum.BlendDstAlpha, out var data12);
        _gl.GetInteger(GLEnum.BlendEquation, out var data13);
        _gl.GetInteger(GLEnum.BlendEquationAlpha, out var data14);
        bool flag = _gl.IsEnabled(GLEnum.Blend);
        bool flag2 = _gl.IsEnabled(GLEnum.CullFace);
        bool flag3 = _gl.IsEnabled(GLEnum.DepthTest);
        bool flag4 = _gl.IsEnabled(GLEnum.StencilTest);
        bool flag5 = _gl.IsEnabled(GLEnum.ScissorTest);
        bool flag6 = _gl.IsEnabled(GLEnum.PrimitiveRestart);
        SetupRenderState(drawDataPtr, num, num2);
        Vector2 displayPos = drawDataPtr.DisplayPos;
        Vector2 framebufferScale = drawDataPtr.FramebufferScale;
        Vector4 vector = default;
        for (int i = 0; i < drawDataPtr.CmdListsCount; i++)
        {
            ImDrawListPtr imDrawListPtr = drawDataPtr.CmdLists.Data[i];
            _gl.BufferData(GLEnum.ArrayBuffer, (nuint)(imDrawListPtr.VtxBuffer.Size * sizeof(ImDrawVert)), (void*)imDrawListPtr.VtxBuffer.Data, GLEnum.StreamDraw);
            _gl.BufferData(GLEnum.ElementArrayBuffer, (nuint)(imDrawListPtr.IdxBuffer.Size * 2), (void*)imDrawListPtr.IdxBuffer.Data, GLEnum.StreamDraw);
            for (int j = 0; j < imDrawListPtr.CmdBuffer.Size; j++)
            {
                ImDrawCmd imDrawCmd = imDrawListPtr.CmdBuffer.Data[j];

                if (imDrawCmd.UserCallback != null)
                {
                    throw new NotImplementedException();
                }

                vector.X = (imDrawCmd.ClipRect.X - displayPos.X) * framebufferScale.X;
                vector.Y = (imDrawCmd.ClipRect.Y - displayPos.Y) * framebufferScale.Y;
                vector.Z = (imDrawCmd.ClipRect.Z - displayPos.X) * framebufferScale.X;
                vector.W = (imDrawCmd.ClipRect.W - displayPos.Y) * framebufferScale.Y;

                if (vector.X < num && vector.Y < num2 && vector.Z >= 0f && vector.W >= 0f)
                {
                    _gl.Scissor((int)vector.X, (int)(num2 - vector.W), (uint)(vector.Z - vector.X), (uint)(vector.W - vector.Y));
                    _gl.BindTexture(GLEnum.Texture2D, (uint)(int)imDrawCmd.TextureId.Handle);
                    _gl.DrawElementsBaseVertex(GLEnum.Triangles, imDrawCmd.ElemCount, GLEnum.UnsignedShort, (void*)(imDrawCmd.IdxOffset * 2), (int)imDrawCmd.VtxOffset);
                }
            }
        }

        _gl.DeleteVertexArray(_vertexArrayObject);
        _vertexArrayObject = 0u;
        _gl.UseProgram((uint)data2);
        _gl.BindTexture(GLEnum.Texture2D, (uint)data3);
        _gl.BindSampler(0u, (uint)data4);
        _gl.ActiveTexture((GLEnum)data);
        _gl.BindVertexArray((uint)data6);
        _gl.BindBuffer(GLEnum.ArrayBuffer, (uint)data5);
        _gl.BlendEquationSeparate((GLEnum)data13, (GLEnum)data14);
        _gl.BlendFuncSeparate((GLEnum)data9, (GLEnum)data10, (GLEnum)data11, (GLEnum)data12);
        if (flag)
        {
            _gl.Enable(GLEnum.Blend);
        }
        else
        {
            _gl.Disable(GLEnum.Blend);
        }

        if (flag2)
        {
            _gl.Enable(GLEnum.CullFace);
        }
        else
        {
            _gl.Disable(GLEnum.CullFace);
        }

        if (flag3)
        {
            _gl.Enable(GLEnum.DepthTest);
        }
        else
        {
            _gl.Disable(GLEnum.DepthTest);
        }

        if (flag4)
        {
            _gl.Enable(GLEnum.StencilTest);
        }
        else
        {
            _gl.Disable(GLEnum.StencilTest);
        }

        if (flag5)
        {
            _gl.Enable(GLEnum.ScissorTest);
        }
        else
        {
            _gl.Disable(GLEnum.ScissorTest);
        }

        if (flag6)
        {
            _gl.Enable(GLEnum.PrimitiveRestart);
        }
        else
        {
            _gl.Disable(GLEnum.PrimitiveRestart);
        }

        _gl.PolygonMode(GLEnum.FrontAndBack, (GLEnum)data7[0]);
        _gl.Scissor(data8[0], data8[1], (uint)data8[2], (uint)data8[3]);
    }

    private void CreateDeviceResources()
    {
        _gl.GetInteger(GLEnum.TextureBinding2D, out int data);
        _gl.GetInteger(GLEnum.ArrayBufferBinding, out int data2);
        _gl.GetInteger(GLEnum.VertexArrayBinding, out int data3);

        string vertexShader = "#version 330\n        layout (location = 0) in vec2 Position;\n        layout (location = 1) in vec2 UV;\n        layout (location = 2) in vec4 Color;\n        uniform mat4 ProjMtx;\n        out vec2 Frag_UV;\n        out vec4 Frag_Color;\n        void main()\n        {\n            Frag_UV = UV;\n            Frag_Color = Color;\n            gl_Position = ProjMtx * vec4(Position.xy,0,1);\n        }";
        string fragmentShader = "#version 330\n        in vec2 Frag_UV;\n        in vec4 Frag_Color;\n        uniform sampler2D Texture;\n        layout (location = 0) out vec4 Out_Color;\n        void main()\n        {\n            Out_Color = Frag_Color * texture(Texture, Frag_UV.st);\n        }";

        using TrShader vs = new(_trContext, TrShaderType.Vertex, vertexShader, false);
        using TrShader fs = new(_trContext, TrShaderType.Fragment, fragmentShader, false);
        _pipeline = new(_trContext, [vs, fs]);
        _pipeline.SetRenderLayer(TrRenderLayer.Overlay);

        _attribLocationTex = _pipeline.GetUniformLocation("Texture");
        _attribLocationProjMtx = _pipeline.GetUniformLocation("ProjMtx");
        _attribLocationVtxPos = _pipeline.GetAttribLocation("Position");
        _attribLocationVtxUV = _pipeline.GetAttribLocation("UV");
        _attribLocationVtxColor = _pipeline.GetAttribLocation("Color");

        _vboHandle = _gl.GenBuffer();
        _elementsHandle = _gl.GenBuffer();

        RecreateFontDeviceTexture();

        _gl.BindTexture(GLEnum.Texture2D, (uint)data);
        _gl.BindBuffer(GLEnum.ArrayBuffer, (uint)data2);
        _gl.BindVertexArray((uint)data3);
    }

    /// <summary>
    /// Creates the texture used to render text.
    /// </summary>
    private void RecreateFontDeviceTexture()
    {
        ImGuiIOPtr iO = ImGui.GetIO();

        byte* pixels;
        int width;
        int height;
        iO.Fonts.GetTexDataAsRGBA32(&pixels, &width, &height);

        _gl.GetInteger(GLEnum.TextureBinding2D, out int data);

        _fontTexture = new TrTexture(_trContext)
        {
            TextureMagFilter = TrTextureFilter.Linear,
            TextureMinFilter = TrTextureFilter.Linear
        };

        _fontTexture.Write((uint)width, (uint)height, TrPixelFormat.RGBA8, pixels);
        _fontTexture.UpdateParameters();

        iO.Fonts.SetTexID((nint)_fontTexture.Handle);
        _gl.BindTexture(GLEnum.Texture2D, (uint)data);
    }

    /// <summary>
    /// Frees all graphics resources used by the renderer.
    /// </summary>
    protected override void Destroy(bool disposing = false)
    {
        _view.Resize -= WindowResized;
        _keyboard.KeyChar -= OnKeyChar;
        _gl.DeleteBuffer(_vboHandle);
        _gl.DeleteBuffer(_elementsHandle);
        _gl.DeleteVertexArray(_vertexArrayObject);
        _fontTexture.Dispose();
        _pipeline.Dispose();
        ImGui.DestroyContext(_context);
    }
}
