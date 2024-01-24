using System.Numerics;
using ImGuiNET;

namespace Triangle.Core.Helpers;

public static class ImGuiHelper
{
    public static void Button(string label, Action action, float width = 0.0f, float height = 0.0f)
    {
        if (ImGui.Button(label, new Vector2(width, height)))
        {
            action();
        }
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
}
