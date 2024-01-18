using System.Numerics;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Structs;

namespace Triangle.Core.Graphics;

public class TrScene : TrGraphics<TrContext>
{
    public event Action<Vector2D<int>>? FramebufferResize;
    public event Action? DrawContentInWindow;

    private readonly IMouse _mouse;
    private readonly IKeyboard _keyboard;
    private readonly TrFrame _frame;

    private bool firstMove = true;
    private Vector2D<float> lastPos;

    public TrScene(IInputContext input, TrContext context, string name) : base(context)
    {
        Name = name;
        Camera = new TrCamera
        {
            Position = new Vector3D<float>(0.0f, 2.0f, 8.0f),
            Fov = 45.0f
        };

        _mouse = input.Mice[0];
        _keyboard = input.Keyboards[0];
        _frame = new TrFrame(Context);
    }

    public string Name { get; }

    public string HostName => $"{Name} - Frame Id: {_frame.Handle}";

    public TrCamera Camera { get; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public bool IsHovered { get; private set; }

    public bool IsFocused { get; private set; }

    public bool IsClosed { get; private set; }

    public int Samples { get; set; } = 4;

    public float CameraSpeed { get; set; } = 2.0f;

    public float CameraSensitivity { get; set; } = 0.1f;

    public TrSceneData SceneData { get; }

    public void Update(double deltaSeconds)
    {
        if (IsHovered)
        {
            if (_mouse.IsButtonPressed(MouseButton.Right))
            {
                Vector2D<float> vector = new(_mouse.Position.X, _mouse.Position.Y);

                if (firstMove)
                {
                    lastPos = vector;

                    firstMove = false;
                }
                else
                {
                    float deltaX = vector.X - lastPos.X;
                    float deltaY = vector.Y - lastPos.Y;

                    Camera.Yaw += deltaX * CameraSensitivity;
                    Camera.Pitch += -deltaY * CameraSensitivity;

                    lastPos = vector;
                }
            }
            else
            {
                firstMove = true;
            }

            if (_keyboard.IsKeyPressed(Key.W))
            {
                Camera.Position += Camera.Front * CameraSpeed * (float)deltaSeconds;
            }

            if (_keyboard.IsKeyPressed(Key.A))
            {
                Camera.Position -= Camera.Right * CameraSpeed * (float)deltaSeconds;
            }

            if (_keyboard.IsKeyPressed(Key.S))
            {
                Camera.Position -= Camera.Front * CameraSpeed * (float)deltaSeconds;
            }

            if (_keyboard.IsKeyPressed(Key.D))
            {
                Camera.Position += Camera.Right * CameraSpeed * (float)deltaSeconds;
            }

            if (_keyboard.IsKeyPressed(Key.Q))
            {
                Camera.Position -= Camera.Up * CameraSpeed * (float)deltaSeconds;
            }

            if (_keyboard.IsKeyPressed(Key.E))
            {
                Camera.Position += Camera.Up * CameraSpeed * (float)deltaSeconds;
            }
        }

        Camera.Width = Width;
        Camera.Height = Height;
    }

    public void Begin()
    {
        _frame.Update(Width, Height, Samples);

        _frame.Bind();
    }

    public void End()
    {
        _frame.Unbind();
    }

    public void DrawHost()
    {
        if (IsClosed)
        {
            return;
        }

        bool isOpen = true;
        if (ImGui.Begin(HostName, ref isOpen, ImGuiWindowFlags.NoSavedSettings))
        {
            IsHovered = ImGui.IsWindowHovered();
            IsFocused = ImGui.IsWindowFocused();

            Vector2 size = ImGui.GetContentRegionAvail();

            int newWidth = Convert.ToInt32(size.X);
            int newHeight = Convert.ToInt32(size.Y);

            if (newWidth - Width != 0 || newHeight - Height != 0)
            {
                Width = newWidth;
                Height = newHeight;

                FramebufferResize?.Invoke(new Vector2D<int>(Width, Height));
            }

            ImGui.Image((nint)_frame.Texture, size, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 0.0f));

            DrawContentInWindow?.Invoke();

            ImGui.End();
        }

        IsClosed = !isOpen;
    }

    protected override void Destroy(bool disposing = false)
    {
        _frame.Dispose();
    }
}
