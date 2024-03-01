using System.Collections.ObjectModel;
using System.Numerics;
using Hexa.NET.ImGui;
using Silk.NET.Maths;
using Triangle.Core.Graphics;
using Triangle.Core.Models;
using Triangle.Core.Structs;
using Triangle.Core.Widgets;
using Triangle.Core.Widgets.Layouts;

namespace Triangle.Core.Helpers;

public static class TrTextureManager
{
    private static readonly Dictionary<string, TrTexture> _textures;
    private static readonly Dictionary<string, TrWrapPanel> _treeNodes;

    private static TrTexture? currentTexture;

    static TrTextureManager()
    {
        _textures = [];
        _treeNodes = [];
    }

    public static ReadOnlyDictionary<string, TrTexture> Textures => new(_textures);

    public static void InitializeImages(TrContext context, string folder)
    {
        CleanResources();

        Dictionary<string, TrTexture> temp = [];
        LoadTextures(context, folder, "bmp");
        LoadTextures(context, folder, "png");
        LoadTextures(context, folder, "jpg");
        LoadTextures(context, folder, "jpeg");
        LoadTextures(context, folder, "psd");
        LoadTextures(context, folder, "tga");
        LoadTextures(context, folder, "hdr");

        Dictionary<string, TrTexture> sortedFiles = temp.OrderBy(item => item.Key, new FilePathComparer()).ToDictionary();

        foreach (KeyValuePair<string, TrTexture> file in sortedFiles)
        {
            _textures.Add(file.Key, file.Value);
        }

        foreach (IGrouping<string, KeyValuePair<string, TrTexture>> gorup in _textures.GroupBy(item => Path.DirectorySeparatorChar + Path.GetDirectoryName(item.Key)))
        {
            TrWrapPanel wrapPanel = new();
            foreach (KeyValuePair<string, TrTexture> pair in gorup)
            {
                string name = Path.GetFileName(pair.Key);

                wrapPanel.Add(new TrControl(64.0f, 46.0f)
                {
                    Margin = new TrThickness(2.0f),
                    Tag = (pair.Key, pair.Value)
                });
            }

            _treeNodes.Add(gorup.Key, wrapPanel);
        }

        void LoadTextures(TrContext context, string folder, string extension)
        {
            string[] files = Directory.GetFiles(folder, $"*.{extension}", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                TrTexture texture = new(context);
                texture.Write(file);

                temp.Add(file, texture);
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

    public static TrTexture Texture(string path)
    {
        if (!_textures.TryGetValue(path, out TrTexture? texture))
        {
            throw new ArgumentException($"Texture {path} not found");
        }

        return texture;
    }

    public static void Manager()
    {
        foreach (KeyValuePair<string, TrWrapPanel> treeNode in _treeNodes)
        {
            if (ImGui.TreeNode(treeNode.Key))
            {
                Vector2 windowSize = ImGui.GetWindowSize();
                Vector2 contentSize = ImGui.GetContentRegionAvail();

                Vector2 leftTop = ImGui.GetCursorPos();
                Vector2 rightBottom = windowSize - (leftTop + contentSize);
                TrThickness windowPadding = new(leftTop.X, leftTop.Y, rightBottom.X, rightBottom.Y);

                treeNode.Value.Width = contentSize.X;
                treeNode.Value.Height = contentSize.Y;
                treeNode.Value.Measure(windowSize.ToGeneric(), windowPadding);

                foreach (TrControl control in treeNode.Value.Children)
                {
                    control.Render((r) =>
                    {
                        (string path, TrTexture texture) = (ValueTuple<string, TrTexture>)control.Tag!;

                        ImGui.SetCursorPos(r.Position.ToSystem());

                        ImGuiHelper.ImageButtonSelected(texture,
                                                        () => { currentTexture = texture; },
                                                        texture == currentTexture,
                                                        r.Size);

                        ImGuiHelper.ShowHelpMarker(path);
                    });
                }

                ImGui.TreePop();
            }
        }

        if (currentTexture != null)
        {
            ImGui.Begin("Texture Properties");

            currentTexture.AdjustImGuiProperties();

            ImGui.End();
        }
    }

    public static void TextureSelection(string label, Vector2D<float> size, ref TrTexture? texture)
    {
        ImGuiHelper.ImageButton(label, texture, () => ImGui.OpenPopup(label), size);

        ImGui.SetNextWindowSizeConstraints(new Vector2(100.0f, 200.0f), new Vector2(300.0f, 200.0f));
        if (ImGui.BeginPopup(label, ImGuiWindowFlags.AlwaysAutoResize))
        {
            foreach (KeyValuePair<string, TrTexture> item in _textures)
            {
                if (ImGui.Selectable(item.Key, item.Value == texture))
                {
                    texture = item.Value;
                }

                ImGuiHelper.ShowHelpMarker(item.Value, new Vector2D<float>(64.0f, 64.0f));
            }

            ImGui.EndPopup();
        }
    }
}
