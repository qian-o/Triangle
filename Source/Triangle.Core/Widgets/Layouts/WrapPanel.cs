using Silk.NET.Maths;

namespace Triangle.Core.Widgets.Layouts;

public class WrapPanel : Layout
{
    public WrapPanel()
    {
    }

    public WrapPanel(float width, float height) : base(width, height)
    {
    }

    protected override void MeasureCore(Vector2D<float> frameSize, Thickness framePadding)
    {
        Flex.Item frame = new(frameSize.X, frameSize.Y)
        {
            PaddingLeft = framePadding.Left,
            PaddingTop = framePadding.Top,
            PaddingRight = framePadding.Right,
            PaddingBottom = framePadding.Bottom
        };

        Flex.Item content = new(Width, Height)
        {
            Direction = Flex.Direction.Row,
            Wrap = Flex.Wrap.Wrap,
            AlignContent = Flex.AlignContent.Start
        };

        foreach (Control control in Children)
        {
            Flex.Item item = new(control.Width, control.Height)
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
        foreach (Flex.Item item in content)
        {
            Children[index].Frame = new Vector4D<float>(content.Frame[0] + item.Frame[0], content.Frame[1] + item.Frame[1], item.Frame[2], item.Frame[3]);

            index++;
        }
    }
}
