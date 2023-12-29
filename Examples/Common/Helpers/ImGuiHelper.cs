﻿using ImGuiNET;
using System.Diagnostics.CodeAnalysis;

namespace Common.Helpers;

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
