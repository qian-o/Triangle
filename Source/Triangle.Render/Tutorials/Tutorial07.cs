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

[DisplayName("PBR 渲染")]
[Description("Physically Based Rendering (PBR) 渲染光照模型。")]
public class Tutorial07(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Meshes
    private TrMesh cubeMesh = null!;
    private TrMesh canvasMesh = null!;
    #endregion

    #region Materials
    private EquirectangularToCubemapMat equirectangularToCubemapMat = null!;
    private IrradianceConvolutionMat irradianceConvolutionMat = null!;
    private PrefilterMat prefilterMat = null!;
    private BRDFMat brdfMat = null!;
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

    #region Textures And CubeMaps
    private TrTexture flipSky = null!;
    private TrCubeMap envCubeMap = null!;
    private TrCubeMap irradianceMap = null!;
    private TrCubeMap prefilteredMap = null!;
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
        irradianceConvolutionMat = new(Context);
        prefilterMat = new(Context);
        brdfMat = new(Context);
        singleCubeMapMat = new(Context);

        flipSky = new(Context);
        flipSky.Write("Resources/Textures/Skies/newport_loft.hdr".Path(), true);

        envCubeMap = new(Context)
        {
            TextureMinFilter = TrTextureFilter.LinearMipmapLinear,
            TextureWrap = TrTextureWrap.ClampToEdge
        };
        envCubeMap.UpdateParameters();

        irradianceMap = new(Context)
        {
            TextureWrap = TrTextureWrap.ClampToEdge
        };
        irradianceMap.UpdateParameters();

        prefilteredMap = new(Context)
        {
            TextureMinFilter = TrTextureFilter.LinearMipmapLinear,
            TextureWrap = TrTextureWrap.ClampToEdge
        };
        irradianceMap.UpdateParameters();

        brdfLUTT = new(Context);

        skyPositiveX = new(Context);
        skyNegativeX = new(Context);
        skyPositiveY = new(Context);
        skyNegativeY = new(Context);
        skyPositiveZ = new(Context);
        skyNegativeZ = new(Context);

        GenerateCubeMap();
        GenerateIrradianceMap();
        GeneratePrefilteredMap();
        GenerateBRDFLUT();

        const int rows = 7;
        const int cols = 7;
        const float spacing = 1.2f;

        spheres = new TrModel[rows * cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int index = i * rows + j;

                PBRMat mat = new(Context)
                {
                    Map0 = irradianceMap
                };

                spheres[index] = new($"Sphere [{index}]", [Context.CreateSphere()], mat);
                spheres[index].Transform.Translate(new Vector3D<float>(i * spacing - rows * spacing / 2.0f, j * spacing - cols * spacing / 2.0f, 0.0f));
                spheres[index].Transform.Scaled(new Vector3D<float>(0.5f));
                SceneController.Add(spheres[index]);
            }
        }

        SetSky(TrTextureManager.Texture("Resources/Textures/Skies/newport_loft.hdr".Path()));

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
        singleCubeMapMat.Map0 = prefilteredMap;
    }

    protected override void RenderScene(double deltaSeconds)
    {
        foreach (var sphere in spheres)
        {
            sphere.Render(GetSceneParameters());
        }

        singleCubeMapMat.Draw(cubeMesh, GetSceneParameters());
    }

    protected override void Destroy(bool disposing = false)
    {
        cubeMesh.Dispose();

        equirectangularToCubemapMat.Dispose();
        irradianceConvolutionMat.Dispose();
        singleCubeMapMat.Dispose();

        skyPositiveX.Dispose();
        skyNegativeX.Dispose();
        skyPositiveY.Dispose();
        skyNegativeY.Dispose();
        skyPositiveZ.Dispose();
        skyNegativeZ.Dispose();

        flipSky.Dispose();
        envCubeMap.Dispose();
        irradianceMap.Dispose();

        foreach (TrModel sphere in spheres)
        {
            sphere.Dispose();
        }
    }

    /// <summary>
    /// PBR: Convert equirectangular map to cubemap
    /// </summary>
    private void GenerateCubeMap()
    {
        const int width = 1024;
        const int height = 1024;

        equirectangularToCubemapMat.Channel0 = flipSky;
        equirectangularToCubemapMat.Projection = Matrix4X4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), 1.0f, 0.1f, 10.0f);

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

        envCubeMap.Write(skyPositiveX.Texture, TrCubeMapFace.PositiveX);
        envCubeMap.Write(skyNegativeX.Texture, TrCubeMapFace.NegativeX);
        envCubeMap.Write(skyPositiveY.Texture, TrCubeMapFace.PositiveY);
        envCubeMap.Write(skyNegativeY.Texture, TrCubeMapFace.NegativeY);
        envCubeMap.Write(skyPositiveZ.Texture, TrCubeMapFace.PositiveZ);
        envCubeMap.Write(skyNegativeZ.Texture, TrCubeMapFace.NegativeZ);
        envCubeMap.GenerateMipmap();
    }

    /// <summary>
    /// PBR: Solve diffuse integral by convolution to create an irradiance (cube)map
    /// </summary>
    private void GenerateIrradianceMap()
    {
        const int width = 64;
        const int height = 64;

        irradianceConvolutionMat.Map0 = envCubeMap;
        irradianceConvolutionMat.Projection = Matrix4X4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), 1.0f, 0.1f, 10.0f);

        skyPositiveX.Update(width, height, pixelFormat: TrPixelFormat.RGB16F);
        skyPositiveX.Bind();
        {
            Context.Clear();

            irradianceConvolutionMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, Vector3D<float>.UnitX, -Vector3D<float>.UnitY);

            irradianceConvolutionMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyPositiveX.Unbind();

        skyNegativeX.Update(width, height, pixelFormat: TrPixelFormat.RGB16F);
        skyNegativeX.Bind();
        {
            Context.Clear();

            irradianceConvolutionMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, -Vector3D<float>.UnitX, -Vector3D<float>.UnitY);

            irradianceConvolutionMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyNegativeX.Unbind();

        skyPositiveY.Update(width, height, pixelFormat: TrPixelFormat.RGB16F);
        skyPositiveY.Bind();
        {
            Context.Clear();

            irradianceConvolutionMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, Vector3D<float>.UnitY, Vector3D<float>.UnitZ);

            irradianceConvolutionMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyPositiveY.Unbind();

        skyNegativeY.Update(width, height, pixelFormat: TrPixelFormat.RGB16F);
        skyNegativeY.Bind();
        {
            Context.Clear();

            irradianceConvolutionMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, -Vector3D<float>.UnitY, -Vector3D<float>.UnitZ);

            irradianceConvolutionMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyNegativeY.Unbind();

        skyPositiveZ.Update(width, height, pixelFormat: TrPixelFormat.RGB16F);
        skyPositiveZ.Bind();
        {
            Context.Clear();

            irradianceConvolutionMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, Vector3D<float>.UnitZ, -Vector3D<float>.UnitY);

            irradianceConvolutionMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyPositiveZ.Unbind();

        skyNegativeZ.Update(width, height, pixelFormat: TrPixelFormat.RGB16F);
        skyNegativeZ.Bind();
        {
            Context.Clear();

            irradianceConvolutionMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, -Vector3D<float>.UnitZ, -Vector3D<float>.UnitY);

            irradianceConvolutionMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyNegativeZ.Unbind();

        irradianceMap.Write(skyPositiveX.Texture, TrCubeMapFace.PositiveX);
        irradianceMap.Write(skyNegativeX.Texture, TrCubeMapFace.NegativeX);
        irradianceMap.Write(skyPositiveY.Texture, TrCubeMapFace.PositiveY);
        irradianceMap.Write(skyNegativeY.Texture, TrCubeMapFace.NegativeY);
        irradianceMap.Write(skyPositiveZ.Texture, TrCubeMapFace.PositiveZ);
        irradianceMap.Write(skyNegativeZ.Texture, TrCubeMapFace.NegativeZ);
    }

    /// <summary>
    /// PBR: Run a quasi monte-carlo simulation on the environment lighting to create a prefilter (cube)map.
    /// </summary>
    private void GeneratePrefilteredMap()
    {
        const int width = 256;
        const int height = 256;
        const int maxMipLevels = 5;

        prefilteredMap.Initialize(width, height, TrPixelFormat.RGB16F);
        prefilteredMap.GenerateMipmap();

        prefilterMat.Map0 = envCubeMap;
        prefilterMat.Projection = Matrix4X4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), 1.0f, 0.1f, 10.0f);

        for (int i = 0; i < maxMipLevels; i++)
        {
            prefilterMat.Roughness = (float)i / (maxMipLevels - 1);

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

            prefilteredMap.Write(skyPositiveX.Texture, TrCubeMapFace.PositiveX, mipLevel);
            prefilteredMap.Write(skyNegativeX.Texture, TrCubeMapFace.NegativeX, mipLevel);
            prefilteredMap.Write(skyPositiveY.Texture, TrCubeMapFace.PositiveY, mipLevel);
            prefilteredMap.Write(skyNegativeY.Texture, TrCubeMapFace.NegativeY, mipLevel);
            prefilteredMap.Write(skyPositiveZ.Texture, TrCubeMapFace.PositiveZ, mipLevel);
            prefilteredMap.Write(skyNegativeZ.Texture, TrCubeMapFace.NegativeZ, mipLevel);
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
