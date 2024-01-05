﻿using Common;
using ForwardRendering.Applications;

namespace ForwardRendering;

internal sealed class Program
{
    static void Main(string[] _)
    {
        using RenderHost<Application3> renderHost = new("Forward Rendering");
        renderHost.Run();
    }
}
