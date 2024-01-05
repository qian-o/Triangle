﻿using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using System.Diagnostics.CodeAnalysis;
using Triangle.Core;

namespace Common.Contracts;

public interface IApplication : IDisposable
{
    void Initialize([NotNull] IWindow window, [NotNull] IInputContext input, [NotNull] TrContext context);

    void Update(double deltaSeconds);

    void Render(double deltaSeconds);

    void DrawImGui();

    void WindowResize(Vector2D<int> size);
}
