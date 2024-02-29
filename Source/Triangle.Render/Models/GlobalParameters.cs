using Triangle.Core.GameObjects;
using Triangle.Core.Structs;

namespace Triangle.Render.Models;

public class GlobalParameters(TrCamera camera, TrSceneData sceneData, TrAmbientLight ambientLight, TrDirectionalLight directionalLight)
{
    public TrCamera Camera { get; set; } = camera;

    public TrSceneData SceneData { get; set; } = sceneData;

    public TrAmbientLight AmbientLight { get; set; } = ambientLight;

    public TrDirectionalLight DirectionalLight { get; set; } = directionalLight;
}
