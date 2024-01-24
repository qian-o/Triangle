using System.Collections.ObjectModel;
using Silk.NET.Maths;

namespace Triangle.Core.Widgets.Layouts;

public abstract class Layout
{
    private readonly List<Control> _children = [];

    private Vector2D<float> lastSize;
    private Vector2D<float> lastFrameSize;
    private Thickness lastFramePadding;

    protected Layout()
    {
    }

    protected Layout(float width, float height)
    {
        Width = width;
        Height = height;
    }

    public float Width { get; set; } = float.NaN;

    public float Height { get; set; } = float.NaN;

    public ReadOnlyCollection<Control> Children => _children.AsReadOnly();

    public void Add(Control element)
    {
        _children.Add(element);
    }

    public void Remove(Control element)
    {
        _children.Remove(element);
    }

    public void Clear()
    {
        _children.Clear();
    }

    public void Measure()
    {
        MeasureCore(new Vector2D<float>(Width, Height), new Thickness());
    }

    public void Measure(Vector2D<float> frameSize, Thickness framePadding)
    {
        if (CheckLayout(frameSize, framePadding))
        {
            MeasureCore(frameSize, framePadding);
        }
    }

    protected abstract void MeasureCore(Vector2D<float> frameSize, Thickness framePadding);

    protected bool CheckLayout(Vector2D<float> frameSize, Thickness framePadding)
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
