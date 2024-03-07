using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials;

namespace Triangle.Render.Tutorials;

[DisplayName("PBR - KTX Skybox")]
[Description("使用 KTX 文件加载天空盒")]
public class Tutorial08(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    private class Map(TrCubeMap skybox, TrCubeMap radiance, TrCubeMap irradiance)
    {
        public TrCubeMap Skybox { get; } = skybox;

        public TrCubeMap Radiance { get; } = radiance;

        public TrCubeMap Irradiance { get; } = irradiance;
    }

    private readonly List<Map> _maps = [];

    #region Meshes
    private TrMesh cubeMesh = null!;
    #endregion

    #region Materials
    private EquirectangularToCubemapMat equirectangularToCubemapMat = null!;
    private SingleCubeMapMat singleCubeMapMat = null!;
    #endregion

    #region Capture Skybox framebuffer
    private TrFrame skyPositiveX = null!;
    private TrFrame skyNegativeX = null!;
    private TrFrame skyPositiveY = null!;
    private TrFrame skyNegativeY = null!;
    private TrFrame skyPositiveZ = null!;
    private TrFrame skyNegativeZ = null!;
    #endregion

    protected override void Loaded()
    {
        cubeMesh = Context.CreateCube(1.0f);

        equirectangularToCubemapMat = new(Context);
        singleCubeMapMat = new(Context);

        skyPositiveX = new(Context);
        skyNegativeX = new(Context);
        skyPositiveY = new(Context);
        skyNegativeY = new(Context);
        skyPositiveZ = new(Context);
        skyNegativeZ = new(Context);

        DirectoryInfo skyDir = new("Resources/Textures/Skies".Path());
        DirectoryInfo[] skyTextures = skyDir.GetDirectories();

        foreach (DirectoryInfo skyTexture in skyTextures)
        {
            string? skybox = skyTexture.GetFiles("Skybox.hdr").FirstOrDefault()?.FullName;
            string? radiance = skyTexture.GetFiles("Radiance.hdr").FirstOrDefault()?.FullName;
            string? irradiance = skyTexture.GetFiles("Irradiance.hdr").FirstOrDefault()?.FullName;

            if (skybox is not null && radiance is not null && irradiance is not null)
            {
                TrCubeMap skyboxMap = new(Context);
                TrCubeMap radianceMap = new(Context);
                TrCubeMap irradianceMap = new(Context);

                GenerateCubeMap(skyboxMap, skybox);
                GenerateCubeMap(radianceMap, radiance);
                GenerateCubeMap(irradianceMap, irradiance);

                _maps.Add(new Map(skyboxMap, radianceMap, irradianceMap));
            }
        }
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        singleCubeMapMat.Map0 = _maps[0].Skybox;
        singleCubeMapMat.Draw(cubeMesh, GetSceneParameters());
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


    /// <summary>
    /// PBR: Convert equirectangular map to cubemap
    /// </summary>
    private void GenerateCubeMap(TrCubeMap cubeMap, string map)
    {
        using TrTexture mapTex = new(Context);
        mapTex.Write(map, true);

        const int width = 1024;
        const int height = 1024;

        equirectangularToCubemapMat.Channel0 = mapTex;
        equirectangularToCubemapMat.Projection = Matrix4X4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), 1.0f, 0.1f, 10.0f);
        equirectangularToCubemapMat.Exposure = SkyMat.Exposure;

        skyPositiveX.Update(width, height, pixelFormat: TrPixelFormat.RGB16F);
        skyPositiveX.Bind();
        {
            Context.Clear();

            equirectangularToCubemapMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, Vector3D<float>.UnitX, -Vector3D<float>.UnitY);

            equirectangularToCubemapMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyPositiveX.Unbind();

        skyNegativeX.Update(width, height, pixelFormat: TrPixelFormat.RGB16F);
        skyNegativeX.Bind();
        {
            Context.Clear();

            equirectangularToCubemapMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, -Vector3D<float>.UnitX, -Vector3D<float>.UnitY);

            equirectangularToCubemapMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyNegativeX.Unbind();

        skyPositiveY.Update(width, height, pixelFormat: TrPixelFormat.RGB16F);
        skyPositiveY.Bind();
        {
            Context.Clear();

            equirectangularToCubemapMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, Vector3D<float>.UnitY, Vector3D<float>.UnitZ);

            equirectangularToCubemapMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyPositiveY.Unbind();

        skyNegativeY.Update(width, height, pixelFormat: TrPixelFormat.RGB16F);
        skyNegativeY.Bind();
        {
            Context.Clear();

            equirectangularToCubemapMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, -Vector3D<float>.UnitY, -Vector3D<float>.UnitZ);

            equirectangularToCubemapMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyNegativeY.Unbind();

        skyPositiveZ.Update(width, height, pixelFormat: TrPixelFormat.RGB16F);
        skyPositiveZ.Bind();
        {
            Context.Clear();

            equirectangularToCubemapMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, Vector3D<float>.UnitZ, -Vector3D<float>.UnitY);

            equirectangularToCubemapMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyPositiveZ.Unbind();

        skyNegativeZ.Update(width, height, pixelFormat: TrPixelFormat.RGB16F);
        skyNegativeZ.Bind();
        {
            Context.Clear();

            equirectangularToCubemapMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, -Vector3D<float>.UnitZ, -Vector3D<float>.UnitY);

            equirectangularToCubemapMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyNegativeZ.Unbind();

        cubeMap.Write(skyPositiveX.Texture, TrCubeMapFace.PositiveX);
        cubeMap.Write(skyNegativeX.Texture, TrCubeMapFace.NegativeX);
        cubeMap.Write(skyPositiveY.Texture, TrCubeMapFace.PositiveY);
        cubeMap.Write(skyNegativeY.Texture, TrCubeMapFace.NegativeY);
        cubeMap.Write(skyPositiveZ.Texture, TrCubeMapFace.PositiveZ);
        cubeMap.Write(skyNegativeZ.Texture, TrCubeMapFace.NegativeZ);
        cubeMap.GenerateMipmap();

        equirectangularToCubemapMat.Channel0 = null;
    }
}
