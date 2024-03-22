using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Triangle.Core.Enums;
using Triangle.Core.GameObjects;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using AttribLocation = uint;

namespace Triangle.Core.Materials;

internal sealed class PointLightMat(TrContext context) : TrMaterial(context, "PointLight")
{
    public const AttribLocation InPosition = 0;

    #region Uniforms
    [StructLayout(LayoutKind.Explicit)]
    private struct UniTransforms
    {
        [FieldOffset(0)]
        public Matrix4X4<float> Model;

        [FieldOffset(64)]
        public Matrix4X4<float> View;

        [FieldOffset(128)]
        public Matrix4X4<float> Projection;

        [FieldOffset(192)]
        public Matrix4X4<float> ObjectToWorld;

        [FieldOffset(256)]
        public Matrix4X4<float> ObjectToClip;

        [FieldOffset(320)]
        public Matrix4X4<float> WorldToObject;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct UniParameters
    {
        [FieldOffset(0)]
        public Vector4D<float> Color;

        [FieldOffset(16)]
        public float Intensity;

        [FieldOffset(20)]
        public float Range;
    }
    #endregion

    private TrBuffer<UniTransforms> uboTransforms = null!;
    private TrBuffer<UniParameters> uboParameters = null!;

    protected override TrRenderPass CreateRenderPass()
    {
        uboTransforms = new(Context);
        uboParameters = new(Context);

        using TrShader vert = new(Context, TrShaderType.Vertex, "Resources/Shaders/PointLight/PointLight.vert.spv".Path());
        using TrShader frag = new(Context, TrShaderType.Fragment, "Resources/Shaders/PointLight/PointLight.frag.spv".Path());

        TrRenderPipeline renderPipeline = new(Context, [vert, frag]);
        renderPipeline.SetRenderLayer(TrRenderLayer.Opaque);

        return new TrRenderPass(Context, [renderPipeline]);
    }

    /// <summary>
    /// Draw point light mesh.
    /// </summary>
    /// <param name="meshes">meshes</param>
    /// <param name="args">
    /// args:
    /// args[0] - Transform matrix
    /// args[1] - Camera
    /// args[2] - Color vec3
    /// args[3] - Intensity float
    /// args[4] - Range float
    /// </param>
    public override void Draw(TrMesh[] meshes, params object[] args)
    {
        if (args.Length != 5
            || args[0] is not Matrix4X4<float> model
            || args[1] is not TrCamera camera
            || args[2] is not Vector3D<float> color
            || args[3] is not float intensity
            || args[4] is not float range)
        {
            throw new ArgumentException("Invalid arguments.");
        }

        foreach (TrRenderPipeline renderPipeline in RenderPass.RenderPipelines)
        {
            renderPipeline.Bind();

            uboTransforms.SetData(new UniTransforms()
            {
                Model = model,
                View = camera.Transform.View,
                Projection = camera.Projection,
                ObjectToWorld = model,
                ObjectToClip = model * camera.Transform.View * camera.Projection,
                WorldToObject = model.Invert()
            });
            uboParameters.SetData(new UniParameters()
            {
                Color = new(color.X, color.Y, color.Z, 1.0f),
                Intensity = intensity,
                Range = range
            });

            renderPipeline.BindUniformBlock(0, uboTransforms);
            renderPipeline.BindUniformBlock(1, uboParameters);

            foreach (TrMesh mesh in meshes)
            {
                mesh.Draw();
            }

            renderPipeline.Unbind();
        }
    }

    /// <summary>
    /// Draw point light model.
    /// </summary>
    /// <param name="models">meshes</param>
    /// <param name="args">
    /// args:
    /// args[0] - Camera
    /// args[1] - Color vec3
    /// args[2] - Intensity float
    /// args[3] - Range float
    /// </param>
    public override void Draw(TrModel[] models, params object[] args)
    {
        foreach (TrModel model in models)
        {
            Draw([.. model.Meshes], [model.Transform.Model, .. args]);
        }
    }

    protected override void ControllerCore()
    {
    }

    protected override void Destroy(bool disposing = false)
    {
        RenderPass.Dispose();

        uboTransforms.Dispose();
        uboParameters.Dispose();
    }
}
