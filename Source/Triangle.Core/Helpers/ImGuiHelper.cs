using System.Numerics;
using Hexa.NET.ImGui;
using Silk.NET.Maths;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;

namespace Triangle.Core.Helpers;

public static class ImGuiHelper
{
    public static Vector4D<float> HoveredColor => new(0.19f, 0.37f, 0.55f, 1.00f);

    public static Vector4D<float> SelectedColor => new(0.06f, 0.53f, 0.98f, 1.00f);

    public static Vector4D<float> SelectedFillColor => new(0.259f, 0.588f, 0.980f, 1.0f);

    public static Vector4D<float> SelectedTextColor => new(1.0f, 1.0f, 1.0f, 1.0f);

    public static Vector4D<float> UnselectedFillColor => new(0.24f, 0.24f, 0.25f, 1.00f);

    public static Vector4D<float> UnselectedTextColor => new(0.50f, 0.50f, 0.50f, 1.00f);

    /// <summary>
    /// 设置ImGui主题，主题风格参考Prowl。
    /// </summary>
    public static void SetThemeByProwl()
    {
        ImGuiStylePtr style = ImGui.GetStyle();

        style.Colors[(int)ImGuiCol.Text] = new(1.00f, 1.00f, 1.00f, 1.00f);
        style.Colors[(int)ImGuiCol.TextDisabled] = new(0.50f, 0.50f, 0.50f, 1.00f);
        style.Colors[(int)ImGuiCol.WindowBg] = new(0.17f, 0.17f, 0.18f, 1f);
        style.Colors[(int)ImGuiCol.ChildBg] = new(0.17f, 0.17f, 0.18f, 0.00f);
        style.Colors[(int)ImGuiCol.PopupBg] = new(0.17f, 0.17f, 0.18f, 1f);
        style.Colors[(int)ImGuiCol.Border] = new(0.15f, 0.16f, 0.17f, 1.00f);
        style.Colors[(int)ImGuiCol.BorderShadow] = new(0.10f, 0.11f, 0.11f, 1.00f);
        style.Colors[(int)ImGuiCol.FrameBg] = new(0.10f, 0.11f, 0.11f, 1.00f);
        style.Colors[(int)ImGuiCol.FrameBgHovered] = HoveredColor.ToSystem();
        style.Colors[(int)ImGuiCol.FrameBgActive] = new(0.10f, 0.11f, 0.11f, 1.00f);
        style.Colors[(int)ImGuiCol.TitleBg] = new(0.08f, 0.08f, 0.09f, 1.00f);
        style.Colors[(int)ImGuiCol.TitleBgActive] = new(0.08f, 0.08f, 0.09f, 1.00f);
        style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new(0.08f, 0.08f, 0.09f, 1.00f);
        style.Colors[(int)ImGuiCol.MenuBarBg] = new(0.08f, 0.08f, 0.09f, 1.00f);
        style.Colors[(int)ImGuiCol.ScrollbarBg] = new(0.10f, 0.11f, 0.11f, 1.00f);
        style.Colors[(int)ImGuiCol.ScrollbarGrab] = new(0.31f, 0.31f, 0.31f, 1.00f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = HoveredColor.ToSystem();
        style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = SelectedColor.ToSystem();
        style.Colors[(int)ImGuiCol.CheckMark] = new(0.26f, 0.59f, 0.98f, 1.00f);
        style.Colors[(int)ImGuiCol.SliderGrab] = new(0.24f, 0.24f, 0.25f, 1.00f);
        style.Colors[(int)ImGuiCol.SliderGrabActive] = SelectedColor.ToSystem();
        style.Colors[(int)ImGuiCol.Button] = new(0.24f, 0.24f, 0.25f, 1.00f);
        style.Colors[(int)ImGuiCol.ButtonHovered] = HoveredColor.ToSystem();
        style.Colors[(int)ImGuiCol.ButtonActive] = SelectedColor.ToSystem();
        style.Colors[(int)ImGuiCol.Header] = new(0.10f, 0.11f, 0.11f, 1.00f);
        style.Colors[(int)ImGuiCol.HeaderHovered] = HoveredColor.ToSystem();
        style.Colors[(int)ImGuiCol.HeaderActive] = SelectedColor.ToSystem();
        style.Colors[(int)ImGuiCol.Separator] = new(0.43f, 0.43f, 0.50f, 0.50f);
        style.Colors[(int)ImGuiCol.SeparatorHovered] = HoveredColor.ToSystem();
        style.Colors[(int)ImGuiCol.SeparatorActive] = SelectedColor.ToSystem();
        style.Colors[(int)ImGuiCol.ResizeGrip] = new(0.26f, 0.59f, 0.98f, 0.20f);
        style.Colors[(int)ImGuiCol.ResizeGripHovered] = HoveredColor.ToSystem();
        style.Colors[(int)ImGuiCol.ResizeGripActive] = SelectedColor.ToSystem();
        style.Colors[(int)ImGuiCol.Tab] = new(0.08f, 0.08f, 0.09f, 1.00f);
        style.Colors[(int)ImGuiCol.TabHovered] = HoveredColor.ToSystem();
        style.Colors[(int)ImGuiCol.TabActive] = new(0.17f, 0.17f, 0.18f, 1.00f);
        style.Colors[(int)ImGuiCol.TabUnfocused] = new(0.08f, 0.08f, 0.09f, 1.00f);
        style.Colors[(int)ImGuiCol.TabUnfocusedActive] = new(0.17f, 0.17f, 0.18f, 1.00f);
        style.Colors[(int)ImGuiCol.DockingPreview] = new(0.26f, 0.59f, 0.98f, 0.70f);
        style.Colors[(int)ImGuiCol.DockingEmptyBg] = new(0.20f, 0.20f, 0.20f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotLines] = new(0.61f, 0.61f, 0.61f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotLinesHovered] = HoveredColor.ToSystem();
        style.Colors[(int)ImGuiCol.PlotHistogram] = new(0.90f, 0.70f, 0.00f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotHistogramHovered] = HoveredColor.ToSystem();
        style.Colors[(int)ImGuiCol.TableHeaderBg] = new(0.19f, 0.19f, 0.20f, 1.00f);
        style.Colors[(int)ImGuiCol.TableBorderStrong] = new(0.31f, 0.31f, 0.35f, 1.00f);
        style.Colors[(int)ImGuiCol.TableBorderLight] = new(0.23f, 0.23f, 0.25f, 1.00f);
        style.Colors[(int)ImGuiCol.TableRowBg] = new(0.00f, 0.00f, 0.00f, 0.00f);
        style.Colors[(int)ImGuiCol.TableRowBgAlt] = new(1.00f, 1.00f, 1.00f, 0.06f);
        style.Colors[(int)ImGuiCol.TextSelectedBg] = new(0.26f, 0.59f, 0.98f, 0.35f);
        style.Colors[(int)ImGuiCol.DragDropTarget] = new(1.00f, 1.00f, 0.00f, 0.90f);
        style.Colors[(int)ImGuiCol.NavHighlight] = new(0.26f, 0.59f, 0.98f, 1.00f);
        style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new(1.00f, 1.00f, 1.00f, 0.70f);
        style.Colors[(int)ImGuiCol.NavWindowingDimBg] = new(0.80f, 0.80f, 0.80f, 0.20f);
        style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new(0.80f, 0.80f, 0.80f, 0.35f);

        style.WindowPadding = new Vector2(3.0f, 3.0f);
        style.FramePadding = new Vector2(6.0f, 2.0f);
        style.CellPadding = new Vector2(4.0f, 0.0f);
        style.ItemSpacing = new Vector2(4.0f, 3.0f);
        style.ItemInnerSpacing = new Vector2(4.0f, 4.0f);
        style.IndentSpacing = 10.0f;
        style.ScrollbarSize = 10.0f;
        style.GrabMinSize = 10.0f;

        style.WindowBorderSize = 0.0f;
        style.ChildBorderSize = 0.0f;
        style.PopupBorderSize = 0.0f;
        style.FrameBorderSize = 0.0f;
        style.TabBorderSize = 0.0f;

        style.WindowRounding = 3.0f;
        style.ChildRounding = 3.0f;
        style.PopupRounding = 3.0f;
        style.FrameRounding = 3.0f;
        style.GrabRounding = 6.0f;
        style.TabRounding = 3.0f;
        style.ScrollbarRounding = 6.0f;

        style.Alpha = 1.0f;
        style.DisabledAlpha = 0.5f;
        style.WindowMinSize = new Vector2(32.0f, 32.0f);
        style.WindowTitleAlign = new Vector2(0.5f, 0.5f);
        style.WindowMenuButtonPosition = ImGuiDir.None;
        style.ColumnsMinSpacing = 6.0f;
        style.ColorButtonPosition = ImGuiDir.Right;
        style.ButtonTextAlign = new Vector2(0.5f, 0.5f);
        style.SelectableTextAlign = new Vector2(0.0f, 0.0f);
    }

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

    public static void Frame(TrFrame? frame)
    {
        Vector2D<float> size = Vector2D<float>.Zero;
        if (frame != null)
        {
            size.X = frame.Width;
            size.Y = frame.Height;
        }

        Frame(frame, ImGui.GetContentRegionAvail().ToGeneric(), size);
    }

    public static void Frame(TrFrame? frame, Vector2D<float> area, Vector2D<float> size, TrHorizontalAlignment horizontalAlignment = TrHorizontalAlignment.Center, TrVerticalAlignment verticalAlignment = TrVerticalAlignment.Center)
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
        ImGui.Image(frame != null ? (nint)frame.Texture.Handle : 0, size.ToSystem(), new Vector2(0.0f, 1.0f), new Vector2(1.0f, 0.0f));
    }

    public static void DragFloat(string label, ref float value, float speed = 1.0f, float min = float.MinValue, float max = float.MaxValue)
    {
        BeginLeftTextRightContent(label);

        ImGui.DragFloat($"##{label}", ref value, speed, min, max);

        EndLeftTextRightContent();
    }

    public static void DragFloat2(string label, ref Vector2D<float> value, float speed = 1.0f, float min = float.MinValue, float max = float.MaxValue)
    {
        BeginLeftTextRightContent(label);

        Vector2 sys = value.ToSystem();
        ImGui.DragFloat2($"##{label}", ref sys, speed, min, max);
        value = sys.ToGeneric();

        EndLeftTextRightContent();
    }

    public static void DragFloat3(string label, ref Vector3D<float> value, float speed = 1.0f, float min = float.MinValue, float max = float.MaxValue)
    {
        BeginLeftTextRightContent(label);

        Vector3 sys = value.ToSystem();
        ImGui.DragFloat3($"##{label}", ref sys, speed, min, max);
        value = sys.ToGeneric();

        EndLeftTextRightContent();
    }

    public static void DragFloat4(string label, ref Vector4D<float> value, float speed = 1.0f, float min = float.MinValue, float max = float.MaxValue)
    {
        BeginLeftTextRightContent(label);

        Vector4 sys = value.ToSystem();
        ImGui.DragFloat4($"##{label}", ref sys, speed, min, max);
        value = sys.ToGeneric();

        EndLeftTextRightContent();
    }

    public static void SliderFloat(string label, ref float value, float min, float max)
    {
        BeginLeftTextRightContent(label);

        ImGui.SliderFloat($"##{label}", ref value, min, max);

        EndLeftTextRightContent();
    }

    public static void SliderFloat2(string label, ref Vector2D<float> value, float min, float max)
    {
        BeginLeftTextRightContent(label);

        Vector2 sys = value.ToSystem();
        ImGui.SliderFloat2($"##{label}", ref sys, min, max);
        value = sys.ToGeneric();

        EndLeftTextRightContent();
    }

    public static void SliderFloat3(string label, ref Vector3D<float> value, float min, float max)
    {
        BeginLeftTextRightContent(label);

        Vector3 sys = value.ToSystem();
        ImGui.SliderFloat3($"##{label}", ref sys, min, max);
        value = sys.ToGeneric();

        EndLeftTextRightContent();
    }

    public static void SliderFloat4(string label, ref Vector4D<float> value, float min, float max)
    {
        BeginLeftTextRightContent(label);

        Vector4 sys = value.ToSystem();
        ImGui.SliderFloat4($"##{label}", ref sys, min, max);
        value = sys.ToGeneric();

        EndLeftTextRightContent();
    }

    public static void ColorEdit3(string label, ref Vector3D<float> value)
    {
        BeginLeftTextRightContent(label);

        Vector3 sys = value.ToSystem();
        ImGui.ColorEdit3($"##{label}", ref sys);
        value = sys.ToGeneric();

        EndLeftTextRightContent();
    }

    public static void ColorEdit4(string label, ref Vector4D<float> value)
    {
        BeginLeftTextRightContent(label);

        Vector4 sys = value.ToSystem();
        ImGui.ColorEdit4($"##{label}", ref sys);
        value = sys.ToGeneric();

        EndLeftTextRightContent();
    }

    private static void BeginLeftTextRightContent(string label)
    {
        float width = ImGui.GetContentRegionAvail().X - ImGui.GetCursorPosX();

        ImGui.Columns(2, false);
        ImGui.SetColumnWidth(0, width * 0.3f);

        ImGui.Text(label);

        ImGui.NextColumn();

        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 1.0f);

        ImGui.PushItemWidth(-1.0f);
    }

    private static void EndLeftTextRightContent()
    {
        ImGui.PopItemWidth();
        ImGui.Columns(1);
    }
}
