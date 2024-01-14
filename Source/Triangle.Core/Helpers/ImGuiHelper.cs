using System.Diagnostics.CodeAnalysis;
using ImGuiNET;

namespace Triangle.Core.Helpers;

public static class ImGuiHelper
{
    public static void Button(string label, [NotNull] Action action)
    {
        if (ImGui.Button(label))
        {
            action();
        }
    }
}
