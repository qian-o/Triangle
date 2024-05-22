using System.Collections.Concurrent;
using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter6;

namespace Triangle.Render.Tutorials;

[DisplayName("Instancing Rendering")]
[Description("This tutorial demonstrates how to render multiple instances of the same object using instancing.")]
public class Tutorial11(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Meshes
    private TrMesh cubeMesh = null!;
    #endregion

    #region Transforms
    private Matrix4X4<float>[] transforms = null!;
    #endregion

    #region Materials
    private DiffusePixelLevelInstancedMat diffusePixelLevelInstancedMat = null!;
    #endregion

    protected override void Loaded()
    {
        cubeMesh = Context.GetCube();

        const int columns = 2000;
        const int rows = 2000;

        int length = columns * rows;

        transforms = new Matrix4X4<float>[length];
        Vector4D<float>[] colors = new Vector4D<float>[length];

        Parallel.ForEach(Partitioner.Create(0, length), range =>
        {
            for (int i = range.Item1; i < range.Item2; i++)
            {
                int c = i % columns;
                int r = i / rows;

                transforms[i] = Matrix4X4.CreateTranslation(new Vector3D<float>(c * 2.0f, 0.0f, -(r * 2.0f)));
                colors[i] = GenerateColor();
            }
        });

        diffusePixelLevelInstancedMat = new DiffusePixelLevelInstancedMat(Context)
        {
            Diffuse = colors
        };
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        diffusePixelLevelInstancedMat.Draw(cubeMesh, transforms, Parameters);
    }

    protected override void Destroy(bool disposing = false)
    {
        diffusePixelLevelInstancedMat.Dispose();
    }

    private static Vector4D<float> GenerateColor()
    {
        byte[] bytes = Guid.NewGuid().ToByteArray();

        byte r = (byte)(bytes[0] ^ bytes[4] ^ bytes[8] ^ bytes[12]);
        byte g = (byte)(bytes[1] ^ bytes[5] ^ bytes[9] ^ bytes[13]);
        byte b = (byte)(bytes[2] ^ bytes[6] ^ bytes[10] ^ bytes[14]);

        return new Vector4D<byte>(r, g, b, 255).ToSingle();
    }
}
