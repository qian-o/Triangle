using System.ComponentModel;
using Silk.NET.Input;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;

namespace Triangle.Render.Tutorials;

[DisplayName("PBR - KTX Skybox")]
[Description("使用 KTX 文件加载天空盒")]
public class Tutorial08(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    private class Map(TrContext context, string skybox, string radiance, string irradiance)
    {
        public TrCubeMap Skybox { get; } = context.Load(skybox, TrPixelFormat.RGB16F);

        public TrCubeMap Radiance { get; } = context.Load(radiance, TrPixelFormat.RGB16F);

        public TrCubeMap Irradiance { get; } = context.Load(irradiance, TrPixelFormat.RGB16F);
    }

    private readonly List<Map> _maps = [];

    protected override void Loaded()
    {
        DirectoryInfo skyDir = new("Resources/Textures/Skies".Path());
        DirectoryInfo[] skyTextures = skyDir.GetDirectories();

        foreach (DirectoryInfo skyTexture in skyTextures)
        {
            string? skybox = skyTexture.GetFiles("Skybox.ktx").FirstOrDefault()?.FullName;
            string? radiance = skyTexture.GetFiles("Radiance.ktx").FirstOrDefault()?.FullName;
            string? irradiance = skyTexture.GetFiles("Irradiance.ktx").FirstOrDefault()?.FullName;

            if (skybox is not null && radiance is not null && irradiance is not null)
            {
                _maps.Add(new Map(Context, skybox, radiance, irradiance));
            }
        }
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
    }

    protected override void Destroy(bool disposing = false)
    {
        _maps.ForEach(map =>
        {
            map.Skybox.Dispose();
            map.Radiance.Dispose();
            map.Irradiance.Dispose();
        });

        _maps.Clear();
    }
}
