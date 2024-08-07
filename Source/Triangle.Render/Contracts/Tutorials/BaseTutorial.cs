﻿using System.Collections.ObjectModel;
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
    private readonly List<TrPointLight> _pointLights;
    #endregion

    private bool disposedValue;

    protected BaseTutorial(IInputContext input, TrContext context)
    {
        Input = input;
        Context = context;
        Scene = new TrScene(input, context, GetType().GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? GetType().Name);
        Scene.DrawContentInWindow += SceneDrawContentInWindow;

        SceneController = new(Scene);
        PickupController = new(Context, Scene, SceneController);

        SkyMat = new SkyMat(Context);
        GridMat = new GridMat(Context);

        _sky = new("Sky", [Context.GetSphere()], SkyMat);
        _sky.Transform.Rotate(new Vector3D<float>(0.0f, 180.0f, 0.0f));

        _grid = new("Grid", [Context.GetCanvas()], GridMat);

        _ambientLight = new(Context, Scene.Camera, "Ambient Light");
        _ambientLight.Transform.Translate(new Vector3D<float>(3.0f, 4.0f, 3.0f));
        _ambientLight.Transform.Scaled(new Vector3D<float>(0.2f));

        _directionalLight = new(Context, Scene.Camera, "Directional Light");
        _directionalLight.Transform.Translate(new Vector3D<float>(3.0f, 5.0f, 3.0f));
        _directionalLight.Transform.Scaled(new Vector3D<float>(0.2f));
        _directionalLight.Transform.Rotate(new Vector3D<float>(-45.0f, 45.0f, 0.0f));

        _pointLights = [];

        SceneController.Add(_sky);
        SceneController.Add(_ambientLight);
        SceneController.Add(_directionalLight);

        Parameters = new(Scene.Camera,
                         Scene.SceneData,
                         _ambientLight,
                         _directionalLight,
                         [.. _pointLights]);

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

    protected GlobalParameters Parameters { get; }

    #region Materials
    protected SkyMat SkyMat { get; }

    protected GridMat GridMat { get; }
    #endregion

    #region Lights
    protected TrAmbientLight AmbientLight => _ambientLight;

    protected TrDirectionalLight DirectionalLight => _directionalLight;

    protected ReadOnlyCollection<TrPointLight> PointLights => _pointLights.AsReadOnly();
    #endregion

    public void Update(double deltaSeconds)
    {
        Parameters.SceneData = Scene.SceneData;
        Parameters.PointLights = [.. _pointLights];

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
            _pointLights.ForEach(light => light.Render());

            _sky.Render(Parameters);
            _grid.Render(Parameters);

            PickupController.PostEffects(Parameters);
        }
        Scene.End();

        PickupController.Render(Parameters);
    }

    public virtual void ImGuiRender()
    {
        SceneController.Controller();

        SceneController.PropertyEditor();
    }

    protected abstract void Loaded();

    protected virtual void SceneDrawContentInWindow()
    {
    }

    protected abstract void UpdateScene(double deltaSeconds);

    protected abstract void RenderScene(double deltaSeconds);

    protected void AddPointLight(string name, out TrPointLight pointLight)
    {
        pointLight = new(Context, Scene.Camera, name);
        pointLight.Transform.Scaled(new Vector3D<float>(0.2f));
        _pointLights.Add(pointLight);

        SceneController.Add(pointLight);
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

            SkyMat.Dispose();
            GridMat.Dispose();

            _ambientLight.Dispose();
            _directionalLight.Dispose();
            _pointLights.ForEach(light => light.Dispose());

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
