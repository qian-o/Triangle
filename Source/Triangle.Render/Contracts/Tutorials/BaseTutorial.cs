using System.ComponentModel;
using System.Reflection;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Controllers;
using Triangle.Core.GameObjects;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Controllers;
using Triangle.Render.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Contracts.Tutorials;

public abstract class BaseTutorial : ITutorial
{
    #region Models
    private readonly TrModel _sky;
    private readonly TrModel _grid;
    private readonly TrAmbientLight _ambientLight;
    private readonly TrDirectionalLight _directionalLight;
    #endregion

    private bool disposedValue;

    protected BaseTutorial(IInputContext input, TrContext context)
    {
        Input = input;
        Context = context;
        Scene = new TrScene(input, context, GetType().GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? GetType().Name);

        SceneController = new(Scene);
        PickupController = new(Context, Scene, SceneController);

        _sky = new("Sky", [Context.CreateSphere()], new SkyMat(Context));

        _grid = new("Grid", [Context.CreateGrid()], new GridMat(Context));

        _ambientLight = new(Context, Scene.Camera, "Ambient Light");
        _ambientLight.Transform.Translate(new Vector3D<float>(3.0f, 4.0f, 3.0f));
        _ambientLight.Transform.Scaled(new Vector3D<float>(0.2f));

        _directionalLight = new(Context, Scene.Camera, "Directional Light");
        _directionalLight.Transform.Translate(new Vector3D<float>(3.0f, 5.0f, 3.0f));
        _directionalLight.Transform.Scaled(new Vector3D<float>(0.2f));
        _directionalLight.Transform.Rotate(new Vector3D<float>(-45.0f, 45.0f, 0.0f));

        SceneController.Add(_sky);
        SceneController.Add(_ambientLight);
        SceneController.Add(_directionalLight);

        Loaded();
    }

    ~BaseTutorial()
    {
        Dispose(disposing: false);
    }

    public IInputContext Input { get; }

    public TrContext Context { get; }

    public TrScene Scene { get; }

    public SceneController SceneController { get; }

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

        Scene.Begin();
        {
            Context.Clear(new Vector4D<float>(0.2f, 0.2f, 0.2f, 1.0f));

            RenderScene(deltaSeconds);

            _ambientLight.Render();
            _directionalLight.Render();

            _sky.Render(GetSceneParameters());
            _grid.Render(GetSceneParameters());

            PickupController.PostEffects(GetSceneParameters());
        }
        Scene.End();

        PickupController.Render(GetSceneParameters());
    }

    public virtual void ImGuiRender()
    {
        SceneController.Controller();

        SceneController.PropertyEditor();
    }

    protected abstract void Loaded();

    protected abstract void UpdateScene(double deltaSeconds);

    protected abstract void RenderScene(double deltaSeconds);

    protected GlobalParameters GetSceneParameters()
    {
        return new(Scene.Camera,
                   Scene.SceneData,
                   _ambientLight,
                   _directionalLight);
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

            _sky.Dispose();
            _grid.Dispose();
            _ambientLight.Dispose();
            _directionalLight.Dispose();

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
