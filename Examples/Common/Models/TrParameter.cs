using Silk.NET.Maths;

namespace Common.Models;

public class TrParameter(Camera camera, Matrix4X4<float> model)
{
    public Camera Camera { get; } = camera;

    public Matrix4X4<float> Model { get; } = model;
}
