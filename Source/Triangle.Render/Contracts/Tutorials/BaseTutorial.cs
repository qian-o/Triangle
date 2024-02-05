using System.ComponentModel;
using System.Reflection;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Core.Models;
using Triangle.Render.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Contracts.Tutorials;

public abstract class BaseTutorial : ITutorial
{
    #region Meshes
    private readonly TrMesh _grid;
    #endregion

    #region Materials
    private readonly GridMat _gridMat;
    #endregion

    private bool disposedValue;

    protected BaseTutorial(IInputContext input, TrContext context)
    {
        Input = input;
        Context = context;
        Scene = new TrScene(input, context, GetType().GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? GetType().Name);
        TransformController = new();
        LightingController = new();

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

    public TransformController TransformController { get; }

    public LightingController LightingController { get; }

    public void Update(double deltaSeconds)
    {
        Scene.Update(deltaSeconds);

        UpdateScene(deltaSeconds);
    }

    public void Render(double deltaSeconds)
    {
        if (!Scene.IsVisible)
        {
            return;
        }

        GL gl = Context.GL;

        Scene.Begin();

        gl.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
        gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);

        RenderScene(deltaSeconds);

        GlobalParameters parameters = new(Scene.Camera, Matrix4X4<float>.Identity, Scene.SceneData);
        _gridMat.Draw(_grid, parameters);

        Scene.End();
    }

    public virtual void ImGuiRender()
    {
        if (ImGui.Begin("Properties"))
        {
            if (ImGui.TreeNode("Scene"))
            {
                int samples = Scene.Samples;
                ImGui.SliderInt("Samples", ref samples, 1, 16);
                Scene.Samples = samples;

                ImGui.TreePop();
            }

            if (ImGui.TreeNode("Camera"))
            {
                float cameraSpeed = Scene.CameraSpeed;
                ImGui.SliderFloat("Speed", ref cameraSpeed, 0.1f, 10.0f);
                Scene.CameraSpeed = cameraSpeed;

                float cameraSensitivity = Scene.CameraSensitivity;
                ImGui.SliderFloat("Sensitivity", ref cameraSensitivity, 0.1f, 1.0f);
                Scene.CameraSensitivity = cameraSensitivity;

                ImGui.TreePop();
            }

            _gridMat.AdjustProperties();

            // TransformController.Controller();
            LightingController.Controller();

            EditProperties();
        }
    }

    protected abstract void Loaded();

    protected abstract void UpdateScene(double deltaSeconds);

    protected abstract void RenderScene(double deltaSeconds);

    protected abstract void EditProperties();

    protected GlobalParameters GetParameters(string transformName = "")
    {
        return new(Scene.Camera,
                   string.IsNullOrEmpty(transformName) ? Matrix4X4<float>.Identity : TransformController[transformName],
                   Scene.SceneData,
                   LightingController.AmbientLight,
                   LightingController.DirectionalLight);
    }

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
