using System.Numerics;
using Hexa.NET.ImGui;
using Hexa.NET.ImGuizmo;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Triangle.Core.Contracts;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
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

        ImGuizmo.SetImGuiContext(_context);

        ImGuiIOPtr io = ImGui.GetIO();

        if (imGuiFontConfig.HasValue)
        {
            nint glyph_ranges = imGuiFontConfig.Value.GetGlyphRange?.Invoke(io) ?? 0;
            io.Fonts.AddFontFromFileTTF(imGuiFontConfig.Value.FontPath, imGuiFontConfig.Value.FontSize, null, (char*)glyph_ranges);

            AddResourceFont(FontAwesome6.FontIconFileNameFAR, FontAwesome6.IconMin, FontAwesome6.IconMax);
            AddResourceFont(FontAwesome6.FontIconFileNameFAS, FontAwesome6.IconMin, FontAwesome6.IconMax);
        }

        onConfigureIO?.Invoke();
        io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

        CreateDeviceResources();
        SetPerFrameImGuiData(1f / 60f);
        BeginFrame();
    }

    public void MakeCurrent()
    {
        ImGui.SetCurrentContext(_context);
        ImGuizmo.SetImGuiContext(_context);
    }

    private void BeginFrame()
    {
        ImGui.NewFrame();
        ImGuizmo.BeginFrame();

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
                ImGuizmo.SetImGuiContext(_context);
            }

            _frameBegun = false;

            ImGui.Render();
            RenderImDrawData(ImGui.GetDrawData());

            if (currentContext != _context)
            {
                ImGui.SetCurrentContext(currentContext);
                ImGuizmo.SetImGuiContext(currentContext);
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
            ImGuizmo.SetImGuiContext(_context);
        }

        if (_frameBegun)
        {
            ImGui.Render();
        }

        SetPerFrameImGuiData(deltaSeconds);
        UpdateImGuiInput();

        _frameBegun = true;

        ImGui.NewFrame();
        ImGuizmo.BeginFrame();

        if (currentContext != _context)
        {
            ImGui.SetCurrentContext(currentContext);
            ImGuizmo.SetImGuiContext(currentContext);
        }
    }

    /// <summary>
    /// Sets per-frame data based on the associated window. This is called by Update(float).
    /// </summary>
    /// <param name="deltaSeconds"></param>
    private void SetPerFrameImGuiData(float deltaSeconds)
    {
        ImGuiIOPtr io = ImGui.GetIO();

        io.DisplaySize = new Vector2(_windowWidth, _windowHeight);

        if (_windowWidth > 0 && _windowHeight > 0)
        {
            io.DisplayFramebufferScale = new Vector2(_view.FramebufferSize.X / _windowWidth, _view.FramebufferSize.Y / _windowHeight);
        }

        io.DeltaTime = deltaSeconds;
    }

    private void UpdateImGuiInput()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        IMouse mouse = _input.Mice[0];
        IKeyboard keyboard = _input.Keyboards[0];
        ScrollWheel scrollWheel = mouse.ScrollWheels[0];

        io.AddMousePosEvent(mouse.Position.X, mouse.Position.Y);
        io.AddMouseButtonEvent(0, mouse.IsButtonPressed(MouseButton.Left));
        io.AddMouseButtonEvent(1, mouse.IsButtonPressed(MouseButton.Right));
        io.AddMouseButtonEvent(2, mouse.IsButtonPressed(MouseButton.Middle));
        io.AddMouseButtonEvent(3, mouse.IsButtonPressed(MouseButton.Button4));
        io.AddMouseButtonEvent(4, mouse.IsButtonPressed(MouseButton.Button5));
        io.AddMouseWheelEvent(scrollWheel.X, scrollWheel.Y);

        foreach (char pressedChar in _pressedChars)
        {
            io.AddInputCharacter(pressedChar);
        }

        foreach (Key key in _keyEnumArr)
        {
            if (key != Key.Unknown && TryMapKey(key, out ImGuiKey imGuiKey))
            {
                io.AddKeyEvent(imGuiKey, keyboard.IsKeyPressed(key));
            }
        }

        _pressedChars.Clear();
    }

    /// <summary>
    /// Tries to map a Silk.NET Key to an ImGuiKey.
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="key">key</param>
    /// <returns></returns>
    private static bool TryMapKey(Key key, out ImGuiKey result)
    {
        static ImGuiKey KeyToImGuiKeyShortcut(Key keyToConvert, Key startKey1, ImGuiKey startKey2)
        {
            int changeFromStart1 = (int)keyToConvert - (int)startKey1;
            return startKey2 + changeFromStart1;
        }

        result = key switch
        {
            >= Key.F1 and <= Key.F24 => KeyToImGuiKeyShortcut(key, Key.F1, ImGuiKey.F1),
            >= Key.Keypad0 and <= Key.Keypad9 => KeyToImGuiKeyShortcut(key, Key.Keypad0, ImGuiKey.Keypad0),
            >= Key.A and <= Key.Z => KeyToImGuiKeyShortcut(key, Key.A, ImGuiKey.A),
            >= Key.Number0 and <= Key.Number9 => KeyToImGuiKeyShortcut(key, Key.Number0, ImGuiKey.Key0),
            Key.ShiftLeft or Key.ShiftRight => ImGuiKey.ModShift,
            Key.ControlLeft or Key.ControlRight => ImGuiKey.ModCtrl,
            Key.AltLeft or Key.AltRight => ImGuiKey.ModAlt,
            Key.SuperLeft or Key.SuperRight => ImGuiKey.ModSuper,
            Key.Menu => ImGuiKey.Menu,
            Key.Up => ImGuiKey.UpArrow,
            Key.Down => ImGuiKey.DownArrow,
            Key.Left => ImGuiKey.LeftArrow,
            Key.Right => ImGuiKey.RightArrow,
            Key.Enter => ImGuiKey.Enter,
            Key.Escape => ImGuiKey.Escape,
            Key.Space => ImGuiKey.Space,
            Key.Tab => ImGuiKey.Tab,
            Key.Backspace => ImGuiKey.Backspace,
            Key.Insert => ImGuiKey.Insert,
            Key.Delete => ImGuiKey.Delete,
            Key.PageUp => ImGuiKey.PageUp,
            Key.PageDown => ImGuiKey.PageDown,
            Key.Home => ImGuiKey.Home,
            Key.End => ImGuiKey.End,
            Key.CapsLock => ImGuiKey.CapsLock,
            Key.ScrollLock => ImGuiKey.ScrollLock,
            Key.PrintScreen => ImGuiKey.PrintScreen,
            Key.Pause => ImGuiKey.Pause,
            Key.NumLock => ImGuiKey.NumLock,
            Key.KeypadDivide => ImGuiKey.KeypadDivide,
            Key.KeypadMultiply => ImGuiKey.KeypadMultiply,
            Key.KeypadSubtract => ImGuiKey.KeypadSubtract,
            Key.KeypadAdd => ImGuiKey.KeypadAdd,
            Key.KeypadDecimal => ImGuiKey.KeypadDecimal,
            Key.KeypadEnter => ImGuiKey.KeypadEnter,
            Key.GraveAccent => ImGuiKey.GraveAccent,
            Key.Minus => ImGuiKey.Minus,
            Key.Equal => ImGuiKey.Equal,
            Key.LeftBracket => ImGuiKey.LeftBracket,
            Key.RightBracket => ImGuiKey.RightBracket,
            Key.Semicolon => ImGuiKey.Semicolon,
            Key.Apostrophe => ImGuiKey.Apostrophe,
            Key.Comma => ImGuiKey.Comma,
            Key.Period => ImGuiKey.Period,
            Key.Slash => ImGuiKey.Slash,
            Key.BackSlash => ImGuiKey.Backslash,
            _ => ImGuiKey.None
        };

        return result != ImGuiKey.None;
    }

    /// <summary>
    /// Add a font to the ImGui context from a file in the Resources/Fonts directory.
    /// </summary>
    /// <param name="fontName">fontName</param>
    /// <param name="min">min</param>
    /// <param name="max">max</param>
    /// <param name="fontSize">fontSize</param>
    private static void AddResourceFont(string fontName, int min, int max, float fontSize = 17 * 2.0f / 3.0f)
    {
        ImFontConfigPtr fontConfig = ImGui.ImFontConfig();
        fontConfig.MergeMode = true;
        fontConfig.PixelSnapH = true;
        fontConfig.GlyphMinAdvanceX = fontSize;
        char[] ranges = [(char)min, (char)max];

        ImGui.GetIO().Fonts.AddFontFromFileTTF(Path.Combine("Resources", "Fonts", fontName), fontSize, fontConfig, ref ranges[0]);
    }

    private unsafe void SetupRenderState(ImDrawDataPtr drawDataPtr)
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
        int framebufferWidth = (int)(drawDataPtr.DisplaySize.X * drawDataPtr.FramebufferScale.X);
        int framebufferHeight = (int)(drawDataPtr.DisplaySize.Y * drawDataPtr.FramebufferScale.Y);
        if (framebufferWidth <= 0 || framebufferHeight <= 0)
        {
            return;
        }

        _gl.GetInteger(GLEnum.ActiveTexture, out int lastActiveTexture);
        _gl.ActiveTexture(GLEnum.Texture0);

        _gl.GetInteger(GLEnum.CurrentProgram, out int lastProgram);
        _gl.GetInteger(GLEnum.TextureBinding2D, out int lastTexture);

        _gl.GetInteger(GLEnum.SamplerBinding, out int lastSampler);

        _gl.GetInteger(GLEnum.ArrayBufferBinding, out int lastArrayBuffer);
        _gl.GetInteger(GLEnum.VertexArrayBinding, out int lastVertexArrayObject);

        Span<int> lastPolygonMode = stackalloc int[2];
        _gl.GetInteger(GLEnum.PolygonMode, lastPolygonMode);

        Span<int> lastScissorBox = stackalloc int[4];
        _gl.GetInteger(GLEnum.ScissorBox, lastScissorBox);

        _gl.GetInteger(GLEnum.BlendSrcRgb, out int lastBlendSrcRgb);
        _gl.GetInteger(GLEnum.BlendDstRgb, out int lastBlendDstRgb);

        _gl.GetInteger(GLEnum.BlendSrcAlpha, out int lastBlendSrcAlpha);
        _gl.GetInteger(GLEnum.BlendDstAlpha, out int lastBlendDstAlpha);

        _gl.GetInteger(GLEnum.BlendEquationRgb, out int lastBlendEquationRgb);
        _gl.GetInteger(GLEnum.BlendEquationAlpha, out int lastBlendEquationAlpha);

        bool lastEnableBlend = _gl.IsEnabled(GLEnum.Blend);
        bool lastEnableCullFace = _gl.IsEnabled(GLEnum.CullFace);
        bool lastEnableDepthTest = _gl.IsEnabled(GLEnum.DepthTest);
        bool lastEnableStencilTest = _gl.IsEnabled(GLEnum.StencilTest);
        bool lastEnableScissorTest = _gl.IsEnabled(GLEnum.ScissorTest);
        bool lastEnablePrimitiveRestart = _gl.IsEnabled(GLEnum.PrimitiveRestart);

        SetupRenderState(drawDataPtr);

        Vector2 displayPos = drawDataPtr.DisplayPos;
        Vector2 framebufferScale = drawDataPtr.FramebufferScale;
        Vector4 vector = default;
        for (int i = 0; i < drawDataPtr.CmdListsCount; i++)
        {
            ImDrawListPtr imDrawListPtr = drawDataPtr.CmdLists.Data[i];
            _gl.BufferData(GLEnum.ArrayBuffer, (nuint)(imDrawListPtr.VtxBuffer.Size * sizeof(ImDrawVert)), imDrawListPtr.VtxBuffer.Data, GLEnum.StreamDraw);
            _gl.BufferData(GLEnum.ElementArrayBuffer, (nuint)(imDrawListPtr.IdxBuffer.Size * 2), imDrawListPtr.IdxBuffer.Data, GLEnum.StreamDraw);

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

                if (vector.X < framebufferWidth && vector.Y < framebufferHeight && vector.Z >= 0f && vector.W >= 0f)
                {
                    _gl.Scissor((int)vector.X, (int)(framebufferHeight - vector.W), (uint)(vector.Z - vector.X), (uint)(vector.W - vector.Y));
                    _gl.BindTexture(GLEnum.Texture2D, (uint)(int)imDrawCmd.TextureId.Handle);
                    _gl.DrawElementsBaseVertex(GLEnum.Triangles, imDrawCmd.ElemCount, GLEnum.UnsignedShort, (void*)(imDrawCmd.IdxOffset * 2), (int)imDrawCmd.VtxOffset);
                }
            }
        }

        _gl.DeleteVertexArray(_vertexArrayObject);
        _vertexArrayObject = 0u;

        _gl.UseProgram((uint)lastProgram);
        _gl.BindTexture(GLEnum.Texture2D, (uint)lastTexture);

        _gl.BindSampler(0, (uint)lastSampler);
        _gl.ActiveTexture((GLEnum)lastActiveTexture);
        _gl.BindVertexArray((uint)lastVertexArrayObject);

        _gl.BindBuffer(GLEnum.ArrayBuffer, (uint)lastArrayBuffer);
        _gl.BlendEquationSeparate((GLEnum)lastBlendEquationRgb, (GLEnum)lastBlendEquationAlpha);
        _gl.BlendFuncSeparate((GLEnum)lastBlendSrcRgb, (GLEnum)lastBlendDstRgb, (GLEnum)lastBlendSrcAlpha, (GLEnum)lastBlendDstAlpha);

        if (lastEnableBlend)
        {
            _gl.Enable(GLEnum.Blend);
        }
        else
        {
            _gl.Disable(GLEnum.Blend);
        }

        if (lastEnableCullFace)
        {
            _gl.Enable(GLEnum.CullFace);
        }
        else
        {
            _gl.Disable(GLEnum.CullFace);
        }

        if (lastEnableDepthTest)
        {
            _gl.Enable(GLEnum.DepthTest);
        }
        else
        {
            _gl.Disable(GLEnum.DepthTest);
        }

        if (lastEnableStencilTest)
        {
            _gl.Enable(GLEnum.StencilTest);
        }
        else
        {
            _gl.Disable(GLEnum.StencilTest);
        }

        if (lastEnableScissorTest)
        {
            _gl.Enable(GLEnum.ScissorTest);
        }
        else
        {
            _gl.Disable(GLEnum.ScissorTest);
        }

        if (lastEnablePrimitiveRestart)
        {
            _gl.Enable(GLEnum.PrimitiveRestart);
        }
        else
        {
            _gl.Disable(GLEnum.PrimitiveRestart);
        }

        _gl.PolygonMode(GLEnum.FrontAndBack, (GLEnum)lastPolygonMode[0]);
        _gl.Scissor(lastScissorBox[0], lastScissorBox[1], (uint)lastScissorBox[2], (uint)lastScissorBox[3]);
    }

    private void CreateDeviceResources()
    {
        _gl.GetInteger(GLEnum.TextureBinding2D, out int lastTexture);
        _gl.GetInteger(GLEnum.ArrayBufferBinding, out int lastArrayBuffer);
        _gl.GetInteger(GLEnum.VertexArrayBinding, out int lastVertexArray);

        string vertexShader = "#version 330\n        layout (location = 0) in vec2 Position;\n        layout (location = 1) in vec2 UV;\n        layout (location = 2) in vec4 Color;\n        uniform mat4 ProjMtx;\n        out vec2 Frag_UV;\n        out vec4 Frag_Color;\n        void main()\n        {\n            Frag_UV = UV;\n            Frag_Color = Color;\n            gl_Position = ProjMtx * vec4(Position.xy,0,1);\n        }";
        string fragmentShader = "#version 330\n        in vec2 Frag_UV;\n        in vec4 Frag_Color;\n        uniform sampler2D Texture;\n        layout (location = 0) out vec4 Out_Color;\n        void main()\n        {\n            Out_Color = Frag_Color * texture(Texture, Frag_UV.st);\n        }";

        using TrShader vs = new(_trContext, TrShaderType.Vertex, vertexShader, false);
        using TrShader fs = new(_trContext, TrShaderType.Fragment, fragmentShader, false);
        _pipeline = new(_trContext, [vs, fs])
        {
            IsBlend = true,
            BlendEquation = TrBlendEquation.Add,
            BlendFuncSeparate = new TrBlendFuncSeparate(TrBlendFactor.SrcAlpha, TrBlendFactor.OneMinusSrcAlpha, TrBlendFactor.One, TrBlendFactor.OneMinusSrcAlpha),
            IsCullFace = false,
            IsDepthTest = false,
            IsStencilTest = false,
            IsScissorTest = true,
            IsPrimitiveRestart = false,
            Polygon = new TrPolygon(TrTriangleFace.FrontAndBack, TrPolygonMode.Fill)
        };

        _attribLocationTex = _pipeline.GetUniformLocation("Texture");
        _attribLocationProjMtx = _pipeline.GetUniformLocation("ProjMtx");
        _attribLocationVtxPos = _pipeline.GetAttribLocation("Position");
        _attribLocationVtxUV = _pipeline.GetAttribLocation("UV");
        _attribLocationVtxColor = _pipeline.GetAttribLocation("Color");

        _vboHandle = _gl.GenBuffer();
        _elementsHandle = _gl.GenBuffer();

        RecreateFontDeviceTexture();

        _gl.BindTexture(GLEnum.Texture2D, (uint)lastTexture);
        _gl.BindBuffer(GLEnum.ArrayBuffer, (uint)lastArrayBuffer);

        _gl.BindVertexArray((uint)lastVertexArray);
    }

    /// <summary>
    /// Creates the texture used to render text.
    /// </summary>
    private void RecreateFontDeviceTexture()
    {
        ImGuiIOPtr io = ImGui.GetIO();

        byte* pixels;
        int width;
        int height;
        io.Fonts.GetTexDataAsRGBA32(&pixels, &width, &height);

        _gl.GetInteger(GLEnum.TextureBinding2D, out int lastTexture);

        _fontTexture = new TrTexture(_trContext)
        {
            TextureMagFilter = TrTextureFilter.Linear,
            TextureMinFilter = TrTextureFilter.Linear
        };

        _fontTexture.Write((uint)width, (uint)height, TrPixelFormat.RGBA8, pixels);
        _fontTexture.UpdateParameters();

        io.Fonts.SetTexID((nint)_fontTexture.Handle);
        _gl.BindTexture(GLEnum.Texture2D, (uint)lastTexture);
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
