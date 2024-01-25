using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using Triangle.Core.Graphics;
using Triangle.Core.Widgets;
using Triangle.Core.Widgets.Layouts;

namespace Triangle.Core.Helpers;

public static class TrTextureManager
{
    private static readonly Dictionary<string, TrTexture> _textures;
    private static readonly Dictionary<string, WrapPanel> _treeNodes;

    static TrTextureManager()
    {
        _textures = [];
        _treeNodes = [];
    }

    public static void InitializeImages(TrContext context, string folder)
    {
        CleanResources();

        LoadTextures(context, folder, "bmp");
        LoadTextures(context, folder, "png");
        LoadTextures(context, folder, "jpg");
        LoadTextures(context, folder, "jpeg");

        foreach (IGrouping<string, KeyValuePair<string, TrTexture>> gorup in _textures.OrderByDescending(item => item.Key)
                                                                                      .GroupBy(item => Path.DirectorySeparatorChar + Path.GetDirectoryName(item.Key)))
        {
            WrapPanel wrapPanel = new();
            foreach (KeyValuePair<string, TrTexture> pair in gorup)
            {
                string name = Path.GetFileName(pair.Key);

                wrapPanel.Add(new Control(64.0f, 46.0f)
                {
                    Margin = new Thickness(2.0f),
                    Tag = (pair.Key, pair.Value)
                });
            }

            _treeNodes.Add(gorup.Key, wrapPanel);
        }

        static void LoadTextures(TrContext context, string folder, string extension)
        {
            string[] files = Directory.GetFiles(folder, $"*.{extension}", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                TrTexture texture = new(context);
                texture.LoadImage(file);

                _textures.Add(file, texture);
            }
        }
    }

    public static void CleanResources()
    {
        foreach (TrTexture texture in _textures.Values)
        {
            texture.Dispose();
        }

        _textures.Clear();
        _treeNodes.Clear();
    }

    public static TrTexture GetTexture(string file)
    {
        return _textures[file];
    }

    public static void Manager()
    {
        foreach (KeyValuePair<string, WrapPanel> treeNode in _treeNodes)
        {
            if (ImGui.TreeNode(treeNode.Key))
            {
                Vector2 windowSize = ImGui.GetWindowSize();
                Vector2 contentSize = ImGui.GetContentRegionAvail();

                Vector2 leftTop = ImGui.GetCursorPos();
                Vector2 rightBottom = windowSize - (leftTop + contentSize);
                Thickness windowPadding = new(leftTop.X, leftTop.Y, rightBottom.X, rightBottom.Y);

                treeNode.Value.Width = contentSize.X;
                treeNode.Value.Height = contentSize.Y;
                treeNode.Value.Measure(windowSize.ToGeneric(), windowPadding);

                foreach (Control control in treeNode.Value.Children)
                {
                    control.Render((r) =>
                    {
                        (string path, TrTexture texture) = (ValueTuple<string, TrTexture>)control.Tag!;

                        ImGui.SetCursorPos(r.Position.ToSystem());

                        ImGuiHelper.ImageButton(texture, () =>
                        {

                            // TODO: Preview image and edit properties.

                        }, r.Size.X, r.Size.Y);

                        ImGuiHelper.ShowHelpMarker(path);
                    });
                }

                ImGui.TreePop();
            }
        }
    }
}
