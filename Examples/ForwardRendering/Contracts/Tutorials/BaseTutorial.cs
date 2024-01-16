using Common.Models;
using ForwardRendering.Materials;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Triangle.Core;
using Triangle.Render.Graphics;
using Triangle.Render.Helpers;

namespace ForwardRendering.Contracts.Tutorials;

public abstract class BaseTutorial : ITutorial
{
    #region Meshes
    private readonly TrMesh _grid;
    #endregion

    #region Materials
    private readonly GridMat _gridMat;
    #endregion

    private bool disposedValue;

    protected BaseTutorial(IInputContext input, TrContext context, string name)
    {
        Input = input;
        Context = context;
        Scene = new TrScene(input, context, name);

        _grid = Context.CreateGrid();
        _gridMat = new(Context);

        Loaded();
    }

    ~BaseTutorial()
    {
        Dispose(disposing: false);
    }

    public IInputContext Input { get; }

    public TrContext Context { get; }

    public TrScene Scene { get; }

    public void Update(double deltaSeconds)
    {
        Scene.Update(deltaSeconds);

        UpdateScene(deltaSeconds);
    }

    public void Render(double deltaSeconds)
    {
        GL gl = Context.GL;

        Scene.Begin();

        gl.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
        gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);

        RenderScene(deltaSeconds);

        TrParameter parameter = new(Scene.Camera, Matrix4X4<float>.Identity);
        _gridMat.Draw(_grid, parameter);

        Scene.End();
    }

    public virtual void ImGuiRender()
    {
        if (ImGui.Begin("Properties"))
        {
            ImGui.SeparatorText("Scene");

            int samples = Scene.Samples;
            ImGui.SliderInt("Samples", ref samples, 1, 16);
            Scene.Samples = samples;

            ImGui.SeparatorText("Camera");

            float cameraSpeed = Scene.CameraSpeed;
            ImGui.SliderFloat("Speed", ref cameraSpeed, 0.1f, 10.0f);
            Scene.CameraSpeed = cameraSpeed;

            float cameraSensitivity = Scene.CameraSensitivity;
            ImGui.SliderFloat("Sensitivity", ref cameraSensitivity, 0.1f, 1.0f);
            Scene.CameraSensitivity = cameraSensitivity;

            _gridMat.AdjustImGuiProperties();

            EditProperties();
        }
    }

    protected abstract void Loaded();

    protected abstract void UpdateScene(double deltaSeconds);

    protected abstract void RenderScene(double deltaSeconds);

    protected abstract void EditProperties();

    protected abstract void Destroy(bool disposing = false);

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Destroy(disposing);
            }

            _gridMat.Dispose();
            _grid.Dispose();

            Scene.Dispose();

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
