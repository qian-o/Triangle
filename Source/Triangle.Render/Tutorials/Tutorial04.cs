﻿using System.ComponentModel;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.GameObjects;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter7;

namespace Triangle.Render.Tutorials;

[DisplayName("2D 纹理")]
[Description("使用 Texture 相关材质渲染胶囊体。")]
public class Tutorial04(IInputContext input, TrContext context) : BaseTutorial(input, context)
{
    #region Models
    private TrModel capsule1 = null!;
    private TrModel capsule2 = null!;
    private TrModel capsule3 = null!;
    private TrModel capsule4 = null!;
    #endregion

    protected override void Loaded()
    {
        capsule1 = new("Capsule 1", [Context.CreateCapsule()], new SingleTextureMat(Context));
        capsule1.Transform.Translate(new Vector3D<float>(-4.5f, 0.0f, 0.0f));

        capsule2 = new("Capsule 2", [Context.CreateCapsule()], new NormalMapWorldSpaceMat(Context));
        capsule2.Transform.Translate(new Vector3D<float>(-1.5f, 0.0f, 0.0f));

        capsule3 = new("Capsule 3", [Context.CreateCapsule()], new NormalMapTangentSpaceMat(Context));
        capsule3.Transform.Translate(new Vector3D<float>(1.5f, 0.0f, 0.0f));

        capsule4 = new("Capsule 4", [Context.CreateCapsule()], new MaskTextureMat(Context));
        capsule4.Transform.Translate(new Vector3D<float>(4.5f, 0.0f, 0.0f));

        SceneController.Add(capsule1);
        SceneController.Add(capsule2);
        SceneController.Add(capsule3);
        SceneController.Add(capsule4);
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        capsule1.Render(GetSceneParameters());
        capsule2.Render(GetSceneParameters());
        capsule3.Render(GetSceneParameters());
        capsule4.Render(GetSceneParameters());
    }

    protected override void Destroy(bool disposing = false)
    {
        capsule1.Dispose();
        capsule2.Dispose();
        capsule3.Dispose();
        capsule4.Dispose();
    }
}
