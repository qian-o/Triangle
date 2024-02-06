using System.ComponentModel;
using System.Reflection;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
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
    private readonly TrMesh[] _gridMeshes;
    #endregion

    #region Materials
    private readonly GridMat _gridMat;
    #endregion

    #region Models
    private readonly MeshModel _grid;
    #endregion

    private bool disposedValue;

    protected BaseTutorial(IInputContext input, TrContext context)
    {
        Input = input;
        Context = context;
        Scene = new TrScene(input, context, GetType().GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? GetType().Name);
        TransformController = new();
        LightingController = new();
        PickupController = new(Context, Scene);

        _gridMeshes = [Context.CreateGrid()];

        _gridMat = new(Context);

        _grid = new(TransformController, "Grid", _gridMeshes, _gridMat);

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

    public PickupController PickupController { get; }

    public void Update(double deltaSeconds)
    {
        Scene.Update(deltaSeconds);

        UpdateScene(deltaSeconds);

        PickupController.Update();
    }

    public void Render(double deltaSeconds)
    {
        if (!Scene.IsVisible)
        {
            return;
        }

        Scene.Clear(new Vector4D<float>(0.2f, 0.2f, 0.2f, 1.0f));
        Scene.Begin();

        RenderScene(deltaSeconds);

        _grid.Render(GetBaseParameters());

        PickupController.PostEffects(GetBaseParameters());

        Scene.End();

        PickupController.Render(GetBaseParameters());
    }

    public virtual void ImGuiRender()
    {
        PickupController.Controller();

        if (ImGui.Begin("Properties"))
        {
            if (ImGui.TreeNode("Scene"))
            {
                int samples = Scene.Samples;
                ImGui.SliderInt("Samples", ref samples, 1, 16);
                Scene.Samples = samples;

                _gridMat.Controller();

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

            LightingController.Controller();

            foreach (MeshModel model in PickupController.PickupModels)
            {
                model.Controller();
            }

            EditProperties();
        }
    }

    protected abstract void Loaded();

    protected abstract void UpdateScene(double deltaSeconds);

    protected abstract void RenderScene(double deltaSeconds);

    protected abstract void EditProperties();

    protected GlobalParameters GetBaseParameters()
    {
        return new(Scene.Camera,
                   Matrix4X4<float>.Identity,
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

            foreach (TrMesh mesh in _gridMeshes)
            {
                mesh.Dispose();
            }

            PickupController.Dispose();

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
