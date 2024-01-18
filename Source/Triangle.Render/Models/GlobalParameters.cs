using Silk.NET.Maths;
using Triangle.Core.Graphics;
using Triangle.Core.Structs;

namespace Triangle.Render.Models;

public class GlobalParameters(TrCamera camera, Matrix4X4<float> model, TrSceneData sceneData, TrAmbientLight ambientLight = default, TrDirectionalLight directionalLight = default)
{
    public TrCamera Camera { get; set; } = camera;

    public Matrix4X4<float> Model { get; set; } = model;

    public TrSceneData SceneData { get; set; } = sceneData;

    public TrAmbientLight AmbientLight { get; set; } = ambientLight;

    public TrDirectionalLight DirectionalLight { get; set; } = directionalLight;
}
