using System.Runtime.InteropServices;
using Common.Contracts.Applications;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using Triangle.Core;
using Triangle.Core.Contracts;

namespace Common;

public unsafe class RenderHost<TApplication> : TrObject where TApplication : IApplication, new()
{
    private readonly IWindow _window;
    private readonly IApplication _application;

    #region Contexts
    private GL gl = null!;
    private IInputContext inputContext = null!;
    private TrContext trContext = null!;
    private ImGuiController imGuiController = null!;
    #endregion

    #region Parameters
    private string renderer = string.Empty;
    private bool firstFrame = true;
    #endregion

    public RenderHost(string title)
    {
        WindowOptions windowOptions = WindowOptions.Default;
        windowOptions.Title = title;
        windowOptions.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(4, 6));
        windowOptions.Samples = 8;
        windowOptions.VSync = false;
        windowOptions.PreferredDepthBufferBits = 32;
        windowOptions.PreferredStencilBufferBits = 32;
        windowOptions.PreferredBitDepth = new Vector4D<int>(8);

        _window = Window.Create(windowOptions);
        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
        _window.Resize += OnResize;
        _window.Closing += OnClosing;

        _application = new TApplication();
    }

    protected override void Destroy(bool disposing = false)
    {
        imGuiController.Dispose();
        trContext.Dispose();
        inputContext.Dispose();

        _application.Dispose();
    }

    private void OnLoad()
    {
        gl = _window.CreateOpenGL();
        inputContext = _window.CreateInput();
        trContext = new TrContext(gl);
        imGuiController = new ImGuiController(gl, _window, inputContext, new ImGuiFontConfig("Resources/Fonts/MSYH.TTC", 14, (a) => a.Fonts.GetGlyphRangesChineseFull()));

        renderer = Marshal.PtrToStringAnsi((nint)gl.GetString(GLEnum.Renderer))!;

        _application.Initialize(_window, inputContext, trContext);
    }

    private void OnUpdate(double deltaSeconds)
    {
        _application.Update(deltaSeconds);
    }

    private void OnRender(double deltaSeconds)
    {
        gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);

        _application.Render(deltaSeconds);

        gl.Viewport(0, 0, (uint)_window.Size.X, (uint)_window.Size.Y);
        gl.BindFramebuffer(GLEnum.Framebuffer, 0);

        imGuiController.Update((float)deltaSeconds);

        if (firstFrame)
        {
            ImGui.StyleColorsLight();
            ImGui.GetStyle().FrameRounding = 6.0f;
            ImGui.GetStyle().FrameBorderSize = 1.0f;

            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            if (File.Exists($"{_application.GetType().Name}.ini"))
            {
                ImGui.LoadIniSettingsFromDisk($"{_application.GetType().Name}.ini");
            }

            firstFrame = false;
        }

        ImGui.DockSpaceOverViewport();

        _application.ImGuiRender();

        if (ImGui.Begin("Info"))
        {
            ImGui.Text(renderer);
            ImGui.Value("FPS", ImGui.GetIO().Framerate);

            ImGui.End();
        }

        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Settings"))
            {
                if (ImGui.MenuItem("Save Layout"))
                {
                    ImGui.SaveIniSettingsToDisk($"{_application.GetType().Name}.ini");
                }

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }

        imGuiController.Render();
    }

    private void OnResize(Vector2D<int> size)
    {
        _application.WindowResize(size);
    }

    private void OnClosing()
    {
        Dispose(true);
    }

    public void Run() => _window.Run();
}
