using Common.Structs;
using Silk.NET.Maths;

namespace Common.Models;

public class TrParameter(Camera camera, Matrix4X4<float> model, TrAmbientLight ambientLight = default, TrDirectionalLight directionalLight = default)
{
    public Camera Camera { get; set; } = camera;

    public Matrix4X4<float> Model { get; set; } = model;

    public TrAmbientLight AmbientLight { get; set; } = ambientLight;

    public TrDirectionalLight DirectionalLight { get; set; } = directionalLight;
}
