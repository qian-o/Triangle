using ImGuiNET;

namespace Triangle.Core.Structs;

public struct ImGuiFontConfig
{
    public string FontPath;

    public int FontSize;

    public Func<ImGuiIOPtr, nint>? GetGlyphRange;

    public ImGuiFontConfig(string fontPath, int fontSize, Func<ImGuiIOPtr, nint>? getGlyphRange = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(fontSize);

        FontPath = fontPath ?? throw new ArgumentNullException(nameof(fontPath));
        FontSize = fontSize;
        GetGlyphRange = getGlyphRange;
    }
}
