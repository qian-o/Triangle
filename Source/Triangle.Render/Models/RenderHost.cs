using System.Runtime.InteropServices;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using Triangle.Core;
using Triangle.Core.Contracts;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Applications;

namespace Triangle.Render.Models;

public unsafe class RenderHost<TApplication> : Disposable where TApplication : IApplication, new()
{
    private readonly IWindow _window;
    private readonly IApplication _application;
    private readonly HashSet<string> _persistentMenuItems;
    private readonly Dictionary<string, bool> _persistentMenuStates;
    private readonly Dictionary<string, Action> _persistentMenuActions;

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

        _persistentMenuItems = [];
        _persistentMenuStates = [];
        _persistentMenuActions = [];
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

        TrTextureManager.InitializeImages(trContext, "Resources/Textures".PathFormatter());

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
            ImGui.GetStyle().WindowMenuButtonPosition = ImGuiDir.None;

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
                PersistentMenuItem("Style Editor", ImGui.ShowStyleEditor);

                if (ImGui.MenuItem("Save Layout"))
                {
                    ImGui.SaveIniSettingsToDisk($"{_application.GetType().Name}.ini");
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Tools"))
            {
                PersistentMenuItem("Texture Manager", TrTextureManager.Manager);

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }

        ExecutePersistentMenu();

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

    private void PersistentMenuItem(string label, Action action)
    {
        _persistentMenuItems.Add(label);
        _persistentMenuStates.TryAdd(label, false);
        _persistentMenuActions.TryAdd(label, action);

        if (ImGui.MenuItem(label))
        {
            _persistentMenuStates[label] = true;
        }
    }

    private void ExecutePersistentMenu()
    {
        foreach (string label in _persistentMenuItems)
        {
            if (_persistentMenuStates[label])
            {
                _persistentMenuStates[label] = ImGuiHelper.ShowDialog(label, _persistentMenuActions[label]);
            }
        }
    }
}
