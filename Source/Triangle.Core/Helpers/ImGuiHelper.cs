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

    public static void ImageButton(string label, TrTexture? texture, Action action)
    {
        ImageButton(label, texture, action, Vector2D<float>.Zero);
    }

    public static void ImageButton(string label, TrTexture? texture, Action action, Vector2D<float> size)
    {
        nint handle = 0;

        if (texture != null)
        {
            if (size.X == 0.0f)
            {
                size.X = texture.Width;
            }

            if (size.Y == 0.0f)
            {
                size.Y = texture.Height;
            }

            handle = (nint)texture.Handle;
        }

        Vector2 padding = ImGui.GetStyle().FramePadding;

        size.X -= padding.X * 2.0f;
        size.Y -= padding.Y * 2.0f;

        if (ImGui.ImageButton(label, handle, size.ToSystem()))
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

    public static void Image(TrTexture? texture)
    {
        Vector2D<float> size = Vector2D<float>.Zero;
        if (texture != null)
        {
            size.X = texture.Width;
            size.Y = texture.Height;
        }

        Image(texture, ImGui.GetContentRegionAvail().ToGeneric(), size);
    }

    public static void Image(TrTexture? texture, Vector2D<float> area, Vector2D<float> size, TrHorizontalAlignment horizontalAlignment = TrHorizontalAlignment.Center, TrVerticalAlignment verticalAlignment = TrVerticalAlignment.Center)
    {
        // 如果区域比图片小，等比例缩放图片。
        if (area.X < size.X || area.Y < size.Y)
        {
            float ratio = Math.Min(area.X / size.X, area.Y / size.Y);

            size.X *= ratio;
            size.Y *= ratio;
        }

        Vector2D<float> offset = new(horizontalAlignment.Alignment(area, size), verticalAlignment.Alignment(area, size));

        ImGui.SetCursorPos(ImGui.GetCursorPos() + offset.ToSystem());
        ImGui.Image(texture != null ? (nint)texture.Handle : 0, size.ToSystem());
    }
}
