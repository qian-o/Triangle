using System.Numerics;
using ImGuiNET;
using Triangle.Core.Graphics;

namespace Triangle.Core.Helpers;

public static class TrTextureManager
{
    private static readonly Dictionary<string, TrTexture> _textures;

    static TrTextureManager()
    {
        _textures = [];
    }

    public static void InitializeImages(TrContext context, string folder)
    {
        CleanResources();

        LoadTextures(context, folder, "bmp");
        LoadTextures(context, folder, "png");
        LoadTextures(context, folder, "jpg");
        LoadTextures(context, folder, "jpeg");

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
    }

    public static TrTexture GetTexture(string file)
    {
        return _textures[file];
    }

    public static void Manager()
    {
        foreach (var gorup in _textures.GroupBy(item => Path.DirectorySeparatorChar + Path.GetDirectoryName(item.Key)))
        {
            if (ImGui.TreeNode(gorup.Key))
            {
                foreach (KeyValuePair<string, TrTexture> texture in gorup)
                {
                    string name = Path.GetFileName(texture.Key);

                    ImGui.ImageButton(name, (nint)texture.Value.Handle, new Vector2(64, 64));
                    ImGui.SameLine();
                }

                ImGui.TreePop();
            }
        }
    }
}
