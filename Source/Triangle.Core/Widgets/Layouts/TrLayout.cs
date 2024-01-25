using System.Collections.ObjectModel;
using Silk.NET.Maths;
using Triangle.Core.Structs;

namespace Triangle.Core.Widgets.Layouts;

public abstract class TrLayout
{
    private readonly List<TrControl> _children = [];

    private Vector2D<float> lastSize;
    private Vector2D<float> lastFrameSize;
    private TrThickness lastFramePadding;

    protected TrLayout()
    {
    }

    protected TrLayout(float width, float height)
    {
        Width = width;
        Height = height;
    }

    public float Width { get; set; } = float.NaN;

    public float Height { get; set; } = float.NaN;

    public ReadOnlyCollection<TrControl> Children => _children.AsReadOnly();

    public void Add(TrControl element)
    {
        _children.Add(element);
    }

    public void Remove(TrControl element)
    {
        _children.Remove(element);
    }

    public void Clear()
    {
        _children.Clear();
    }

    public void Measure()
    {
        MeasureCore(new Vector2D<float>(Width, Height), new TrThickness());
    }

    public void Measure(Vector2D<float> frameSize, TrThickness framePadding)
    {
        if (CheckLayout(frameSize, framePadding))
        {
            MeasureCore(frameSize, framePadding);
        }
    }

    protected abstract void MeasureCore(Vector2D<float> frameSize, TrThickness framePadding);

    protected bool CheckLayout(Vector2D<float> frameSize, TrThickness framePadding)
    {
        if (lastSize != new Vector2D<float>(Width, Height) || lastFrameSize != frameSize || lastFramePadding != framePadding)
        {
            lastSize = new Vector2D<float>(Width, Height);
            lastFrameSize = frameSize;
            lastFramePadding = framePadding;

            return true;
        }

        return false;
    }
}
