using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Enums;
using Triangle.Core.GameObjects;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials;

namespace Triangle.Render.Tutorials;

[DisplayName("PBR - Skybox")]
[Description("使用预生成的 HDR 贴图创建 PBR 材质。")]
public class Tutorial08(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    private sealed class Map(TrCubeMap skybox, TrCubeMap radiance, TrCubeMap irradiance, TrCubeMap prefiltered)
    {
        public TrCubeMap Skybox { get; } = skybox;

        public TrCubeMap Radiance { get; } = radiance;

        public TrCubeMap Irradiance { get; } = irradiance;

        public TrCubeMap Prefiltered { get; } = prefiltered;
    }

    // PBR: Maximum number of mip levels for prefiltered map (0 to 4)
    private const int MaxMipLevels = 4;

    private readonly List<Map> _maps = [];

    #region Meshes
    private TrMesh cubeMesh = null!;
    private TrMesh canvasMesh = null!;
    #endregion

    #region Materials
    private EquirectangularToCubemapMat equirectangularToCubemapMat = null!;
    private PrefilterMat prefilterMat = null!;
    private BRDFMat brdfMat = null!;
    private PBRMat[] pbrMats = null!;
    #endregion

    #region Capture Skybox framebuffer
    private TrFrame skyPositiveX = null!;
    private TrFrame skyNegativeX = null!;
    private TrFrame skyPositiveY = null!;
    private TrFrame skyNegativeY = null!;
    private TrFrame skyPositiveZ = null!;
    private TrFrame skyNegativeZ = null!;
    #endregion

    #region Textures
    private TrTexture brdfLUTT = null!;
    #endregion

    #region Models
    private TrModel[] spheres = null!;
    #endregion

    protected override void Loaded()
    {
        cubeMesh = Context.CreateCube(1.0f);
        canvasMesh = Context.CreateCanvas();

        equirectangularToCubemapMat = new(Context);
        prefilterMat = new(Context);
        brdfMat = new(Context);

        skyPositiveX = new(Context);
        skyNegativeX = new(Context);
        skyPositiveY = new(Context);
        skyNegativeY = new(Context);
        skyPositiveZ = new(Context);
        skyNegativeZ = new(Context);

        brdfLUTT = new(Context);

        GenerateBRDFLUT();

        // PBR: Load skyboxes
        {
            DirectoryInfo skyDir = new("Resources/Textures/Skies".Path());
            DirectoryInfo[] skyTextures = skyDir.GetDirectories();

            foreach (DirectoryInfo skyTexture in skyTextures)
            {
                string? skybox = skyTexture.GetFiles("Skybox.hdr").FirstOrDefault()?.FullName;
                string? radiance = skyTexture.GetFiles("Radiance.hdr").FirstOrDefault()?.FullName;
                string? irradiance = skyTexture.GetFiles("Irradiance.hdr").FirstOrDefault()?.FullName;

                if (skybox is not null && radiance is not null && irradiance is not null)
                {
                    TrCubeMap skyboxMap = new(Context)
                    {
                        TextureMinFilter = TrTextureFilter.LinearMipmapLinear,
                        TextureWrap = TrTextureWrap.ClampToEdge
                    };
                    skyboxMap.UpdateParameters();

                    TrCubeMap radianceMap = new(Context)
                    {
                        TextureMinFilter = TrTextureFilter.LinearMipmapLinear,
                        TextureWrap = TrTextureWrap.ClampToEdge
                    };
                    skyboxMap.UpdateParameters();

                    TrCubeMap irradianceMap = new(Context)
                    {
                        TextureMinFilter = TrTextureFilter.LinearMipmapLinear,
                        TextureWrap = TrTextureWrap.ClampToEdge
                    };
                    skyboxMap.UpdateParameters();

                    TrCubeMap prefilteredMap = new(Context)
                    {
                        TextureMinFilter = TrTextureFilter.LinearMipmapLinear,
                        TextureWrap = TrTextureWrap.ClampToEdge
                    };
                    prefilteredMap.UpdateParameters();

                    GenerateCubeMap(skyboxMap, skybox);
                    GenerateCubeMap(radianceMap, radiance);
                    GenerateCubeMap(irradianceMap, irradiance);
                    GeneratePrefilteredMap(prefilteredMap, radianceMap);

                    _maps.Add(new Map(skyboxMap, radianceMap, irradianceMap, prefilteredMap));
                }
            }
        }

        // PBR: Create materials and models
        {
            DirectoryInfo pbrDir = new("Resources/Textures/PBR".Path());
            DirectoryInfo[] pbrMaterials = pbrDir.GetDirectories();

            pbrMats = new PBRMat[pbrMaterials.Length];
            spheres = new TrModel[pbrMaterials.Length];
            for (int i = 0; i < pbrMaterials.Length; i++)
            {
                DirectoryInfo directory = pbrMaterials[i];

                PBRMat mat = new(Context)
                {
                    Channel0 = TrTextureManager.Texture($"Resources/Textures/PBR/{directory.Name}/Albedo.png".Path()),
                    Channel1 = TrTextureManager.Texture($"Resources/Textures/PBR/{directory.Name}/Normal.png".Path()),
                    Channel2 = TrTextureManager.Texture($"Resources/Textures/PBR/{directory.Name}/Metallic.png".Path()),
                    Channel3 = TrTextureManager.Texture($"Resources/Textures/PBR/{directory.Name}/Roughness.png".Path()),
                    Channel4 = TrTextureManager.Texture($"Resources/Textures/PBR/{directory.Name}/AmbientOcclusion.png".Path()),
                    MaxMipLevels = MaxMipLevels,
                    BRDF = brdfLUTT
                };

                pbrMats[i] = mat;
                spheres[i] = new($"Sphere [{directory.Name}]", [Context.CreateSphere()], mat);
                spheres[i].Transform.Translate(new Vector3D<float>(-2.0f + i, 0.0f, 0.0f));
                spheres[i].Transform.Scaled(new Vector3D<float>(0.4f));

                SceneController.Add(spheres[i]);
            }
        }

        AddPointLight("Point Light [0]", out TrPointLight pointLight0);
        pointLight0.Transform.Translate(new Vector3D<float>(-1.0f, 1.0f, 2.0f));

        AddPointLight("Point Light [1]", out TrPointLight pointLight1);
        pointLight1.Transform.Translate(new Vector3D<float>(1.0f, 1.0f, 2.0f));

        AddPointLight("Point Light [2]", out TrPointLight pointLight2);
        pointLight2.Transform.Translate(new Vector3D<float>(-1.0f, -1.0f, 2.0f));

        AddPointLight("Point Light [3]", out TrPointLight pointLight3);
        pointLight3.Transform.Translate(new Vector3D<float>(1.0f, -1.0f, 2.0f));
    }

    protected override void UpdateScene(double deltaSeconds)
    {
        foreach (PBRMat mat in pbrMats)
        {
            mat.Map0 = _maps[0].Irradiance;
            mat.Map1 = _maps[0].Prefiltered;
        }
    }

    protected override void RenderScene(double deltaSeconds)
    {
        foreach (var sphere in spheres)
        {
            sphere.Render(GetSceneParameters());
        }
    }

    protected override void Destroy(bool disposing = false)
    {
        _maps.ForEach(map =>
        {
            map.Skybox.Dispose();
            map.Irradiance.Dispose();
        });

        _maps.Clear();

        cubeMesh.Dispose();
        canvasMesh.Dispose();

        equirectangularToCubemapMat.Dispose();
        prefilterMat.Dispose();
        brdfMat.Dispose();

        skyPositiveX.Dispose();
        skyNegativeX.Dispose();
        skyPositiveY.Dispose();
        skyNegativeY.Dispose();
        skyPositiveZ.Dispose();
        skyNegativeZ.Dispose();

        brdfLUTT.Dispose();

        foreach (TrModel sphere in spheres)
        {
            sphere.Dispose();
        }
    }

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

    private void GeneratePrefilteredMap(TrCubeMap cubeMap, TrCubeMap radiance)
    {
        const int width = 256;
        const int height = 256;

        cubeMap.Initialize(width, height, TrPixelFormat.RGB16F);
        cubeMap.GenerateMipmap();

        prefilterMat.Map0 = radiance;
        prefilterMat.Projection = Matrix4X4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), 1.0f, 0.1f, 10.0f);

        for (int i = 0; i <= MaxMipLevels; i++)
        {
            prefilterMat.Roughness = (float)i / (MaxMipLevels - 1);

            int mipWidth = (int)(width * MathF.Pow(0.5f, i));
            int mipHeight = (int)(height * MathF.Pow(0.5f, i));

            GenerateMipMap(i, mipWidth, mipHeight);
        }

        void GenerateMipMap(int mipLevel, int mipWidth, int mipHeight)
        {
            skyPositiveX.Update(mipWidth, mipHeight, pixelFormat: TrPixelFormat.RGB16F);
            skyPositiveX.Bind();
            {
                Context.Clear();

                prefilterMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, Vector3D<float>.UnitX, -Vector3D<float>.UnitY);

                prefilterMat.Draw(cubeMesh, GetSceneParameters());
            }
            skyPositiveX.Unbind();

            skyNegativeX.Update(mipWidth, mipHeight, pixelFormat: TrPixelFormat.RGB16F);
            skyNegativeX.Bind();
            {
                Context.Clear();

                prefilterMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, -Vector3D<float>.UnitX, -Vector3D<float>.UnitY);

                prefilterMat.Draw(cubeMesh, GetSceneParameters());
            }
            skyNegativeX.Unbind();

            skyPositiveY.Update(mipWidth, mipHeight, pixelFormat: TrPixelFormat.RGB16F);
            skyPositiveY.Bind();
            {
                Context.Clear();

                prefilterMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, Vector3D<float>.UnitY, Vector3D<float>.UnitZ);

                prefilterMat.Draw(cubeMesh, GetSceneParameters());
            }
            skyPositiveY.Unbind();

            skyNegativeY.Update(mipWidth, mipHeight, pixelFormat: TrPixelFormat.RGB16F);
            skyNegativeY.Bind();
            {
                Context.Clear();

                prefilterMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, -Vector3D<float>.UnitY, -Vector3D<float>.UnitZ);

                prefilterMat.Draw(cubeMesh, GetSceneParameters());
            }
            skyNegativeY.Unbind();

            skyPositiveZ.Update(mipWidth, mipHeight, pixelFormat: TrPixelFormat.RGB16F);
            skyPositiveZ.Bind();
            {
                Context.Clear();

                prefilterMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, Vector3D<float>.UnitZ, -Vector3D<float>.UnitY);

                prefilterMat.Draw(cubeMesh, GetSceneParameters());
            }
            skyPositiveZ.Unbind();

            skyNegativeZ.Update(mipWidth, mipHeight, pixelFormat: TrPixelFormat.RGB16F);
            skyNegativeZ.Bind();
            {
                Context.Clear();

                prefilterMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, -Vector3D<float>.UnitZ, -Vector3D<float>.UnitY);

                prefilterMat.Draw(cubeMesh, GetSceneParameters());
            }
            skyNegativeZ.Unbind();

            cubeMap.Write(skyPositiveX.Texture, TrCubeMapFace.PositiveX, mipLevel);
            cubeMap.Write(skyNegativeX.Texture, TrCubeMapFace.NegativeX, mipLevel);
            cubeMap.Write(skyPositiveY.Texture, TrCubeMapFace.PositiveY, mipLevel);
            cubeMap.Write(skyNegativeY.Texture, TrCubeMapFace.NegativeY, mipLevel);
            cubeMap.Write(skyPositiveZ.Texture, TrCubeMapFace.PositiveZ, mipLevel);
            cubeMap.Write(skyNegativeZ.Texture, TrCubeMapFace.NegativeZ, mipLevel);
        }
    }

    /// <summary>
    /// PBR: Generate a 2D LUT from the BRDF equations used.
    /// </summary>
    private void GenerateBRDFLUT()
    {
        const int width = 512;
        const int height = 512;

        skyPositiveX.Update(width, height, pixelFormat: TrPixelFormat.RG16F);
        skyPositiveX.Bind();
        {
            Context.Clear();

            brdfMat.Draw(canvasMesh, GetSceneParameters());
        }
        skyPositiveX.Unbind();

        brdfLUTT.Write(skyPositiveX);
    }
}
