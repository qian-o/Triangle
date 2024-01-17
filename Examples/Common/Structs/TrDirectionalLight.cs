﻿using Silk.NET.Maths;

namespace Common.Structs;

public struct TrDirectionalLight(Vector3D<float> color, Vector3D<float> direction)
{
    public Vector3D<float> Color = color;

    public Vector3D<float> Direction = direction;
}
