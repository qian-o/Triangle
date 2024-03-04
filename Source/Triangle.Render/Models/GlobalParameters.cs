﻿using Triangle.Core.GameObjects;
using Triangle.Core.Structs;

namespace Triangle.Render.Models;

public class GlobalParameters(TrCamera camera, TrSceneData sceneData, TrAmbientLight ambientLight, TrDirectionalLight directionalLight, TrPointLight[]? pointLights = null)
{
    public TrCamera Camera { get; set; } = camera;

    public TrSceneData SceneData { get; set; } = sceneData;

    public TrAmbientLight AmbientLight { get; set; } = ambientLight;

    public TrDirectionalLight DirectionalLight { get; set; } = directionalLight;

    public TrPointLight[] PointLights { get; set; } = pointLights ?? [];

    public object? Tag { get; set; }
}
