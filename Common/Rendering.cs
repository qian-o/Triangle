using Common.Contracts;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using Silk.NET.OpenGLES.Extensions.ImGui;
using Silk.NET.Windowing;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Triangle.Core;

namespace Common;

public unsafe class Rendering : IDisposable
{
    private readonly IApplication _application;
    private readonly IWindow _window;

    #region IDisposable
    private bool disposedValue;
    #endregion

    #region Contexts
    private GL gl = null!;
    private IInputContext inputContext = null!;
    private TrContext trContext = null!;
    private ImGuiController imGuiController = null!;
    private Camera camera = null!;
    private string renderer = string.Empty;
    #endregion

    #region Input
    private IMouse mouse = null!;
    private IKeyboard keyboard = null!;
    private bool firstMove = true;
    private Vector2D<float> lastPos;
    #endregion

    #region Speeds
    private float cameraSpeed = 4.0f;
    private float cameraSensitivity = 0.2f;
    #endregion

    public Rendering([NotNull] IApplication application)
    {
        _application = application;

        WindowOptions windowOptions = WindowOptions.Default;
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
        _window.FramebufferResize += OnFramebufferResize;
        _window.Closing += OnClosing;
    }

    ~Rendering()
    {
        Dispose(disposing: false);
    }

    private void OnLoad()
    {
        gl = _window.CreateOpenGLES();
        inputContext = _window.CreateInput();
        trContext = new TrContext(gl);
        imGuiController = new ImGuiController(gl, _window, inputContext);
        camera = new Camera
        {
            Position = new Vector3D<float>(0.0f, 2.0f, 8.0f),
            Fov = 45.0f
        };
        renderer = Marshal.PtrToStringAnsi((nint)gl.GetString(GLEnum.Renderer))!;

        mouse = inputContext.Mice[0];
        keyboard = inputContext.Keyboards[0];

        _application.Initialize(_window, trContext, camera);
    }

    private void OnUpdate(double deltaSeconds)
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

        camera.Width = _window.Size.X;
        camera.Height = _window.Size.Y;

        _application.Update(deltaSeconds);
    }

    private void OnRender(double deltaSeconds)
    {
        gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);

        _application.Render(deltaSeconds);

        imGuiController.Update((float)deltaSeconds);

        ImGui.Begin("Info");
        ImGui.Text(renderer);
        ImGui.Value("FPS", ImGui.GetIO().Framerate);
        ImGui.End();

        ImGui.Begin("Settings");
        ImGui.DragFloat("Camera Speed", ref cameraSpeed, 0.5f, 0.5f, 20.0f);
        ImGui.DragFloat("Camera Sensitivity", ref cameraSensitivity, 0.2f, 0.2f, 10.0f);
        ImGui.End();

        _application.ImGui();

        imGuiController.Render();
    }

    private void OnFramebufferResize(Vector2D<int> size)
    {
        _application.Resize(size);
    }

    private void OnClosing()
    {
        Dispose(true);
    }

    public void Run() => _window.Run();

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _application.Destroy();

                imGuiController.Dispose();
                trContext.Dispose();
                inputContext.Dispose();
                gl.Dispose();
            }
            else
            {
                _window.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
