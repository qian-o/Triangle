using System.ComponentModel;
using System.Reflection;
using Hexa.NET.ImGui;
using Hexa.NET.ImGuizmo;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Controllers;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Controllers;
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

    private ImGuizmoOperation currentGizmoOperation = ImGuizmoOperation.Translate;
    private bool disposedValue;

    protected BaseTutorial(IInputContext input, TrContext context)
    {
        Input = input;
        Context = context;
        Scene = new TrScene(input, context, GetType().GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? GetType().Name);
        Scene.DrawContentInWindow += Scene_DrawContentInWindow;

        TransformController = new();
        LightingController = new();
        PickupController = new(Context, Scene);

        _gridMeshes = [Context.CreateGrid()];

        _gridMat = new(Context);

        _grid = new("Grid", _gridMeshes, _gridMat);

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

    public SceneController PickupController { get; }

    private void Scene_DrawContentInWindow()
    {
        foreach (MeshModel model in PickupController.PickupModels)
        {
            float[] view = Scene.Camera.View.ToArray();
            float[] projection = Scene.Camera.Projection.ToArray();
            float[] transform = model.Transform.Model.ToArray();

            ImGuizmo.Manipulate(ref view[0], ref projection[0], currentGizmoOperation, ImGuizmoMode.Local, ref transform[0]);

            if (ImGuizmo.IsUsing())
            {
                model.Transform.Matrix(transform.ToMatrix());
            }
        }
    }

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

        Scene.Begin();
        {
            Context.Clear(new Vector4D<float>(0.2f, 0.2f, 0.2f, 1.0f));

            RenderScene(deltaSeconds);

            _grid.Render(GetSceneParameters());

            PickupController.PostEffects(GetSceneParameters());
        }
        Scene.End();

        PickupController.Render(GetSceneParameters());
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

            if (PickupController.PickupModels.Count != 0)
            {
                if (ImGui.RadioButton("Translate", currentGizmoOperation == ImGuizmoOperation.Translate))
                {
                    currentGizmoOperation = ImGuizmoOperation.Translate;
                }

                ImGui.SameLine();

                if (ImGui.RadioButton("Rotate", currentGizmoOperation == ImGuizmoOperation.Rotate))
                {
                    currentGizmoOperation = ImGuizmoOperation.Rotate;
                }

                ImGui.SameLine();

                if (ImGui.RadioButton("Scale", currentGizmoOperation == ImGuizmoOperation.Scale))
                {
                    currentGizmoOperation = ImGuizmoOperation.Scale;
                }
            }

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

    protected GlobalParameters GetSceneParameters()
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
