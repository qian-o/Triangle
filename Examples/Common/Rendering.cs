using Common.Contracts;
using Common.Helpers;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using Silk.NET.OpenGLES.Extensions.ImGui;
using Silk.NET.Windowing;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using Triangle.Core;
using Triangle.Core.Contracts;
using Triangle.Core.Graphics;

namespace Common;

public unsafe class Rendering : TrObject
{
    private readonly IWindow _window;
    private readonly IApplication _application;

    #region Contexts
    private GL gl = null!;
    private IInputContext inputContext = null!;
    private TrContext trContext = null!;
    private ImGuiController imGuiController = null!;
    #endregion

    #region Info
    private string renderer = string.Empty;
    #endregion

    #region Frame
    private TrFrame frame = null!;
    private int frameWidth;
    private int frameHeight;
    private bool isWindowHovered;
    #endregion

    #region Input
    private IMouse mouse = null!;
    private IKeyboard keyboard = null!;
    private bool firstMove = true;
    private Vector2D<float> lastPos;
    #endregion

    #region Camera
    private Camera camera = null!;
    private float cameraSpeed = 2.0f;
    private float cameraSensitivity = 0.1f;
    #endregion

    #region Parameters
    private bool firstFrame = true;
    #endregion

    public Rendering([NotNull] IApplication application, string title)
    {
        _application = application;

        WindowOptions windowOptions = WindowOptions.Default;
        windowOptions.Title = title;
        windowOptions.API = new GraphicsAPI(ContextAPI.OpenGLES, new APIVersion(3, 2));
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
    }

    protected override void Destroy(bool disposing = false)
    {
        frame.Dispose();
        imGuiController.Dispose();
        trContext.Dispose();
        inputContext.Dispose();
    }

    private void OnLoad()
    {
        gl = _window.CreateOpenGLES();
        inputContext = _window.CreateInput();
        trContext = new TrContext(gl);
        imGuiController = new ImGuiController(gl, _window, inputContext);

        renderer = Marshal.PtrToStringAnsi((nint)gl.GetString(GLEnum.Renderer))!;

        frame = new TrFrame(trContext, samples: _window.Samples == null ? 1 : (uint)_window.Samples);

        mouse = inputContext.Mice[0];
        keyboard = inputContext.Keyboards[0];

        camera = new Camera
        {
            Position = new Vector3D<float>(0.0f, 2.0f, 8.0f),
            Fov = 45.0f
        };

        _application.Initialize(_window, trContext, camera);
    }

    private void OnUpdate(double deltaSeconds)
    {
        if (isWindowHovered)
        {
            if (mouse.IsButtonPressed(MouseButton.Middle))
            {
                Vector2D<float> vector = new(mouse.Position.X, mouse.Position.Y);

                if (firstMove)
                {
                    lastPos = vector;

                    firstMove = false;
                }
                else
                {
                    float deltaX = vector.X - lastPos.X;
                    float deltaY = vector.Y - lastPos.Y;

                    camera.Yaw += deltaX * cameraSensitivity;
                    camera.Pitch += -deltaY * cameraSensitivity;

                    lastPos = vector;
                }
            }
            else
            {
                firstMove = true;
            }

            if (keyboard.IsKeyPressed(Key.W))
            {
                camera.Position += camera.Front * cameraSpeed * (float)deltaSeconds;
            }

            if (keyboard.IsKeyPressed(Key.A))
            {
                camera.Position -= camera.Right * cameraSpeed * (float)deltaSeconds;
            }

            if (keyboard.IsKeyPressed(Key.S))
            {
                camera.Position -= camera.Front * cameraSpeed * (float)deltaSeconds;
            }

            if (keyboard.IsKeyPressed(Key.D))
            {
                camera.Position += camera.Right * cameraSpeed * (float)deltaSeconds;
            }

            if (keyboard.IsKeyPressed(Key.Q))
            {
                camera.Position -= camera.Up * cameraSpeed * (float)deltaSeconds;
            }

            if (keyboard.IsKeyPressed(Key.E))
            {
                camera.Position += camera.Up * cameraSpeed * (float)deltaSeconds;
            }
        }

        camera.Width = frameWidth;
        camera.Height = frameHeight;

        _application.Update(deltaSeconds);
    }

    private void OnRender(double deltaSeconds)
    {
        gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);

        frame.Update(frameWidth, frameHeight);
        _application.Render(frame, deltaSeconds);

        gl.Viewport(0, 0, (uint)_window.Size.X, (uint)_window.Size.Y);
        gl.BindFramebuffer(GLEnum.Framebuffer, 0);

        imGuiController.Update((float)deltaSeconds);

        if (firstFrame)
        {
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            if (File.Exists($"{_application.GetType().Name}.ini"))
            {
                ImGui.LoadIniSettingsFromDisk($"{_application.GetType().Name}.ini");
            }

            firstFrame = false;
        }

        ImGui.DockSpaceOverViewport();

        DrawHost();

        ImGui.Begin("Info");
        ImGui.Text(renderer);
        ImGui.Value("FPS", ImGui.GetIO().Framerate);
        ImGui.End();

        ImGui.Begin("Settings");
        ImGui.DragFloat("Camera Speed", ref cameraSpeed, 0.5f, 0.5f, 20.0f);
        ImGui.DragFloat("Camera Sensitivity", ref cameraSensitivity, 0.2f, 0.2f, 10.0f);
        ImGuiHelper.Button("Save Layout", () =>
        {
            ImGui.SaveIniSettingsToDisk($"{_application.GetType().Name}.ini");
        });
        ImGui.End();

        ImGui.Begin("Application");
        _application.DrawImGui();
        ImGui.End();

        imGuiController.Render();
    }

    private void OnResize(Vector2D<int> size)
    {
        gl.Viewport(size);

        _application.WindowResize(size);
    }

    private void OnClosing()
    {
        Dispose(true);
    }

    private void DrawHost()
    {
        ImGui.Begin("Main");

        isWindowHovered = ImGui.IsWindowHovered();

        Vector2 size = ImGui.GetContentRegionAvail();

        int newWidth = Convert.ToInt32(size.X);
        int newHeight = Convert.ToInt32(size.Y);

        if (newWidth - frameWidth != 0 || newHeight - frameHeight != 0)
        {
            frameWidth = newWidth;
            frameHeight = newHeight;

            _application.FramebufferResize(new Vector2D<int>(frameWidth, frameHeight));
        }

        ImGui.Image((nint)frame.ColorBuffer, size, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 0.0f));

        ImGui.End();
    }

    public void Run() => _window.Run();
}
