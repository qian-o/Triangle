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

    #region Sky Map
    private TrCubeMap envCubemap = null!;
    #endregion

    #region Models
    private TrModel[] spheres = null!;
    #endregion

    protected override void Loaded()
    {
        cubeMesh = Context.CreateCube(1.0f);

        equirectangularToCubemapMat = new(Context);
        singleCubeMapMat = new(Context);

        envCubemap = new(Context)
        {
            TextureWrap = TrTextureWrap.ClampToEdge
        };
        envCubemap.UpdateParameters();

        skyPositiveX = new(Context);
        skyNegativeX = new(Context);
        skyPositiveY = new(Context);
        skyNegativeY = new(Context);
        skyPositiveZ = new(Context);
        skyNegativeZ = new(Context);

        GenerateCubeMap("Resources/Textures/Skies/cloudy_puresky_4k.hdr".Path());
        GenerateIrradianceMap();

        const int rows = 7;
        const int cols = 7;
        const float spacing = 1.2f;

        spheres = new TrModel[rows * cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                int index = i * rows + j;

                spheres[index] = new($"Sphere [{index}]", [Context.CreateSphere()], new PhysicallyBasedRenderingMat(Context));
                spheres[index].Transform.Translate(new Vector3D<float>(i * spacing - rows * spacing / 2.0f, j * spacing - cols * spacing / 2.0f, 0.0f));
                spheres[index].Transform.Scaled(new Vector3D<float>(0.5f));
                SceneController.Add(spheres[index]);
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
        singleCubeMapMat.Map0 = envCubemap;
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
        equirectangularToCubemapMat.Dispose();

        cubeMesh.Dispose();

        skyPositiveX.Dispose();
        skyNegativeX.Dispose();
        skyPositiveY.Dispose();
        skyNegativeY.Dispose();
        skyPositiveZ.Dispose();
        skyNegativeZ.Dispose();

        envCubemap.Dispose();

        foreach (TrModel sphere in spheres)
        {
            sphere.Dispose();
        }
    }

    private void GenerateCubeMap(string hdr)
    {
        equirectangularToCubemapMat.Channel0?.Dispose();
        equirectangularToCubemapMat.Channel0 = new TrTexture(Context);
        equirectangularToCubemapMat.Channel0.Write(hdr, true);

        equirectangularToCubemapMat.Projection = Matrix4X4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), 1.0f, 0.1f, 10.0f);

        skyPositiveX.Update(1024, 1024, pixelFormat: TrPixelFormat.RGB16F);
        skyPositiveX.Bind();
        {
            Context.Clear();

            equirectangularToCubemapMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, Vector3D<float>.UnitX, -Vector3D<float>.UnitY);

            equirectangularToCubemapMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyPositiveX.Unbind();

        skyNegativeX.Update(1024, 1024, pixelFormat: TrPixelFormat.RGB16F);
        skyNegativeX.Bind();
        {
            Context.Clear();

            equirectangularToCubemapMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, -Vector3D<float>.UnitX, -Vector3D<float>.UnitY);

            equirectangularToCubemapMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyNegativeX.Unbind();

        skyPositiveY.Update(1024, 1024, pixelFormat: TrPixelFormat.RGB16F);
        skyPositiveY.Bind();
        {
            Context.Clear();

            equirectangularToCubemapMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, Vector3D<float>.UnitY, Vector3D<float>.UnitZ);

            equirectangularToCubemapMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyPositiveY.Unbind();

        skyNegativeY.Update(1024, 1024, pixelFormat: TrPixelFormat.RGB16F);
        skyNegativeY.Bind();
        {
            Context.Clear();

            equirectangularToCubemapMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, -Vector3D<float>.UnitY, -Vector3D<float>.UnitZ);

            equirectangularToCubemapMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyNegativeY.Unbind();

        skyPositiveZ.Update(1024, 1024, pixelFormat: TrPixelFormat.RGB16F);
        skyPositiveZ.Bind();
        {
            Context.Clear();

            equirectangularToCubemapMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, Vector3D<float>.UnitZ, -Vector3D<float>.UnitY);

            equirectangularToCubemapMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyPositiveZ.Unbind();

        skyNegativeZ.Update(1024, 1024, pixelFormat: TrPixelFormat.RGB16F);
        skyNegativeZ.Bind();
        {
            Context.Clear();

            equirectangularToCubemapMat.View = Matrix4X4.CreateLookAt(Vector3D<float>.Zero, -Vector3D<float>.UnitZ, -Vector3D<float>.UnitY);

            equirectangularToCubemapMat.Draw(cubeMesh, GetSceneParameters());
        }
        skyNegativeZ.Unbind();

        envCubemap.Write(skyPositiveX.Texture, TrCubeMapFace.PositiveX);
        envCubemap.Write(skyNegativeX.Texture, TrCubeMapFace.NegativeX);
        envCubemap.Write(skyPositiveY.Texture, TrCubeMapFace.PositiveY);
        envCubemap.Write(skyNegativeY.Texture, TrCubeMapFace.NegativeY);
        envCubemap.Write(skyPositiveZ.Texture, TrCubeMapFace.PositiveZ);
        envCubemap.Write(skyNegativeZ.Texture, TrCubeMapFace.NegativeZ);
    }

    private void GenerateIrradianceMap()
    {
        // TODO: Implement this method
    }
}
