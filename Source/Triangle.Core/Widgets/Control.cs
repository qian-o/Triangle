﻿using Silk.NET.Maths;

namespace Triangle.Core.Widgets;

public class Control
{
    public struct RenderData
    {
        public Vector2D<float> Position;

        public Vector2D<float> Size;

        public object? Tag;
    }

    public Control()
    {
    }

    public Control(float width, float height)
    {
        Width = width;
        Height = height;
    }

    public float Width { get; set; } = float.NaN;

    public float Height { get; set; } = float.NaN;

    public Thickness Margin { get; set; }

    public object? Tag { get; set; }

    public Vector4D<float> Frame { get; set; }

    public void Render(Action<RenderData> renderer)
    {
        renderer.Invoke(new RenderData
        {
            Position = new Vector2D<float>(Frame[0], Frame[1]),
            Size = new Vector2D<float>(Frame[2], Frame[3]),
            Tag = Tag
        });
    }
}
