using Hexa.NET.ImGui;

namespace Triangle.Core.Structs;

public readonly struct ImGuiFontConfig
{
    public string FontPath { get; }

    public int FontSize { get; }

    public Func<ImGuiIOPtr, nint>? GetGlyphRange { get; }

    public ImGuiFontConfig(string fontPath, int fontSize, Func<ImGuiIOPtr, nint>? getGlyphRange = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(fontSize);

        FontPath = fontPath ?? throw new ArgumentNullException(nameof(fontPath));
        FontSize = fontSize;
        GetGlyphRange = getGlyphRange;
    }
}
