using Silk.NET.Maths;
using Triangle.Core.Structs;

namespace Triangle.Core.Widgets.Layouts;

public class TrWrapPanel : TrLayout
{
    public TrWrapPanel()
    {
    }

    public TrWrapPanel(float width, float height) : base(width, height)
    {
    }

    protected override void MeasureCore(Vector2D<float> frameSize, TrThickness framePadding)
    {
        TrFlex.Item frame = new(frameSize.X, frameSize.Y)
        {
            PaddingLeft = framePadding.Left,
            PaddingTop = framePadding.Top,
            PaddingRight = framePadding.Right,
            PaddingBottom = framePadding.Bottom
        };

        TrFlex.Item content = new(Width, Height)
        {
            Direction = TrFlex.Direction.Row,
            Wrap = TrFlex.Wrap.Wrap,
            AlignContent = TrFlex.AlignContent.Start
        };

        foreach (TrControl control in Children)
        {
            TrFlex.Item item = new(control.Width, control.Height)
            {
                MarginLeft = control.Margin.Left,
                MarginTop = control.Margin.Top,
                MarginRight = control.Margin.Right,
                MarginBottom = control.Margin.Bottom
            };

            content.Add(item);
        }

        frame.Add(content);
        frame.Layout();

        int index = 0;
        foreach (TrFlex.Item item in content)
        {
            Children[index].Frame = new Vector4D<float>(content.Frame[0] + item.Frame[0], content.Frame[1] + item.Frame[1], item.Frame[2], item.Frame[3]);

            index++;
        }
    }
}
