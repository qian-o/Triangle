﻿using System.ComponentModel;
using Common.Models;
using ForwardRendering.Contracts.Tutorials;
using ForwardRendering.Materials.Chapter5;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Render.Graphics;
using Triangle.Render.Helpers;

namespace ForwardRendering.Tutorials;

[DisplayName("简单场景")]
[Description("使用 SimpleMat 材质渲染一个球体并显示法线方向。")]
public class Tutorial01(IInputContext input, TrContext context, string name) : BaseTutorial(input, context, name)
{
    #region Meshes
    private TrMesh sphere = null!;
    #endregion

    #region Materials
    private SimpleMat simpleMat = null!;
    #endregion

    #region Transform
    private Vector3D<float> translation = new(0.0f, 0.0f, 0.0f);
    private Vector3D<float> rotation = new(0.0f, 0.0f, 0.0f);
    private Vector3D<float> scale = new(1.0f, 1.0f, 1.0f);
    #endregion

    protected override void Loaded()
    {
        sphere = Context.AssimpParsing("Resources/Models/Sphere.glb")[0];

        simpleMat = new(Context);
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        Matrix4X4<float> model = Matrix4X4.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix4X4.CreateScale(scale) * Matrix4X4.CreateTranslation(translation);

        TrSceneParameters sceneParameters = new(Scene.Camera, model);

        simpleMat.Draw(sphere, sceneParameters);
    }

    protected override void EditProperties()
    {
        simpleMat.AdjustImGuiProperties();
    }

    protected override void Destroy(bool disposing = false)
    {
        simpleMat.Dispose();
        sphere.Dispose();
    }
}
