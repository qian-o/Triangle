using Silk.NET.Maths;

namespace Triangle.Core.Structs;

public struct TrSceneData(Vector2D<float> resolution, Vector4D<float> mouse, Vector4D<float> date, float time, float deltaTime, float frameRate, int frameCount)
{
    public Vector2D<float> Resolution = resolution;

    public Vector4D<float> Mouse = mouse;

    public Vector4D<float> Date = date;

    public float Time = time;

    public float DeltaTime = deltaTime;

    public float FrameRate = frameRate;

    public int FrameCount = frameCount;
}
