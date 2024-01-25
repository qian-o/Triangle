using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;

namespace Triangle.Core.Helpers;

public static class ImGuiHelper
{
    public static Vector4D<float> SelectedFillColor => new(0.259f, 0.588f, 0.980f, 1.0f);

    public static Vector4D<float> SelectedTextColor => new(1.0f, 1.0f, 1.0f, 1.0f);

    public static Vector4D<float> UnselectedFillColor => new(0.0f, 0.0f, 0.0f, 0.0f);

    public static Vector4D<float> UnselectedTextColor => new(0.0f, 0.0f, 0.0f, 1.0f);

    public static void Button(string label, Action action)
    {
        Button(label, action, Vector2D<float>.Zero);
    }

    public static void Button(string label, Action action, Vector2D<float> size)
    {
        if (ImGui.Button(label, size.ToSystem()))
        {
            action();
        }
    }

    public static void ButtonSelected(string label, Action action, bool selected)
    {
        ButtonSelected(label, action, selected, Vector2D<float>.Zero);
    }

    public static void ButtonSelected(string label, Action action, bool selected, Vector2D<float> size)
    {
        ImGui.PushStyleColor(ImGuiCol.Button, selected ? SelectedFillColor.ToSystem() : UnselectedFillColor.ToSystem());
        ImGui.PushStyleColor(ImGuiCol.Text, selected ? SelectedTextColor.ToSystem() : UnselectedTextColor.ToSystem());

        if (ImGui.Button(label, size.ToSystem()))
        {
            action();
        }

        ImGui.PopStyleColor(2);
    }

    public static void ImageButton(TrTexture texture, Action action)
    {
        ImageButton(texture, action, Vector2D<float>.Zero);
    }

    public static void ImageButton(TrTexture texture, Action action, Vector2D<float> size)
    {
        if (size.X == 0.0f)
        {
            size.X = texture.Width;
        }

        if (size.Y == 0.0f)
        {
            size.Y = texture.Height;
        }

        Vector2 padding = ImGui.GetStyle().FramePadding;

        size.X -= padding.X * 2.0f;
        size.Y -= padding.Y * 2.0f;

        if (ImGui.ImageButton($"{texture.Handle} {size}", (nint)texture.Handle, size.ToSystem()))
        {
            action();
        }
    }

    public static void ImageButtonSelected(TrTexture texture, Action action, bool selected)
    {
        ImageButtonSelected(texture, action, selected, Vector2D<float>.Zero);
    }

    public static void ImageButtonSelected(TrTexture texture, Action action, bool selected, Vector2D<float> size)
    {
        if (size.X == 0.0f)
        {
            size.X = texture.Width;
        }

        if (size.Y == 0.0f)
        {
            size.Y = texture.Height;
        }

        Vector2 padding = ImGui.GetStyle().FramePadding;

        size.X -= padding.X * 2.0f;
        size.Y -= padding.Y * 2.0f;

        ImGui.PushStyleColor(ImGuiCol.Button, selected ? SelectedFillColor.ToSystem() : UnselectedFillColor.ToSystem());

        if (ImGui.ImageButton($"{texture.Handle} {size}", (nint)texture.Handle, size.ToSystem()))
        {
            action();
        }

        ImGui.PopStyleColor();
    }

    public static void ShowHelpMarker(string description)
    {
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text(description);
            ImGui.EndTooltip();
        }
    }

    public static void ShowHelpMarker(TrTexture texture, Vector2D<float> size)
    {
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Image((nint)texture.Handle, size.ToSystem());
            ImGui.EndTooltip();
        }
    }

    public static bool ShowDialog(string label, Action action, float width = 300.0f, float height = 400.0f)
    {
        ImGui.SetNextWindowSize(new Vector2(width, height), ImGuiCond.FirstUseEver);

        bool isOpen = true;
        if (ImGui.Begin(label, ref isOpen))
        {
            action();

            ImGui.End();
        }

        return isOpen;
    }

    public static void EnumCombo<T>(string label, ref T @enum) where T : Enum
    {
        if (ImGui.BeginCombo(label, @enum.ToString()))
        {
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                if (ImGui.Selectable(value.ToString(), value.Equals(@enum)))
                {
                    @enum = value;
                }
            }

            ImGui.EndCombo();
        }
    }

    public static void Image(TrTexture texture)
    {
        Image(texture, Vector2D<float>.Zero);
    }

    public static void Image(TrTexture texture, Vector2D<float> size, TrHorizontalAlignment horizontalAlignment = TrHorizontalAlignment.Center, TrVerticalAlignment verticalAlignment = TrVerticalAlignment.Center)
    {
        if (size.X == 0.0f)
        {
            size.X = texture.Width;
        }

        if (size.Y == 0.0f)
        {
            size.Y = texture.Height;
        }

        Vector2D<float> area = ImGui.GetContentRegionAvail().ToGeneric();

        Vector4D<float> content = new(0.0f, 0.0f, size.X, size.Y);
        Alignment(horizontalAlignment, area, ref content);
        Alignment(verticalAlignment, area, ref content);

        if (content.Z < 0.0f || content.W < 0.0f)
        {
            return;
        }

        ImGui.SetCursorPos(ImGui.GetCursorPos() + new Vector2D<float>(content.X, content.Y).ToSystem());
        ImGui.Image((nint)texture.Handle, new Vector2D<float>(content.Z, content.W).ToSystem());
    }

    private static void Alignment(TrHorizontalAlignment horizontalAlignment, Vector2D<float> area, ref Vector4D<float> content)
    {
        if (area.X < content.W)
        {
            content.Z = area.X;
        }

        switch (horizontalAlignment)
        {
            case TrHorizontalAlignment.Left:
                content.X = 0.0f;
                break;
            case TrHorizontalAlignment.Center:
                content.X = (area.X - content.Z) / 2.0f;
                break;
            case TrHorizontalAlignment.Right:
                content.X = area.X - content.X;
                break;
            case TrHorizontalAlignment.Stretch:
                content.X = 0.0f;
                break;
        }
    }

    private static void Alignment(TrVerticalAlignment verticalAlignment, Vector2D<float> area, ref Vector4D<float> content)
    {
        if (area.Y < content.W)
        {
            content.W = area.Y;
        }

        switch (verticalAlignment)
        {
            case TrVerticalAlignment.Top:
                content.Y = 0.0f;
                break;
            case TrVerticalAlignment.Center:
                content.Y = (area.Y - content.W) / 2.0f;
                break;
            case TrVerticalAlignment.Bottom:
                content.Y = area.Y - content.W;
                break;
            case TrVerticalAlignment.Stretch:
                content.Y = 0.0f;
                break;
        }
    }
}
