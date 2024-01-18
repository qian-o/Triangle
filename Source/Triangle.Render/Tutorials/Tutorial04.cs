﻿using System.ComponentModel;
using System.Numerics;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Core.Structs;
using Triangle.Render.Contracts.Tutorials;
using Triangle.Render.Materials.Chapter7;
using Triangle.Render.Models;

namespace Triangle.Render.Tutorials;

[DisplayName("2D 纹理")]
[Description("使用 Texture 相关材质渲染胶囊体。")]
public class Tutorial04(IInputContext input, TrContext context, string name) : BaseTutorial(input, context, name)
{
    #region Meshes
    private TrMesh capsule = null!;
    #endregion

    #region Transforms
    private Vector3D<float> translation = new(0.0f, 0.0f, 0.0f);
    private Vector3D<float> rotation = new(0.0f, 0.0f, 0.0f);
    private Vector3D<float> scale = new(1.0f, 1.0f, 1.0f);
    private Vector3D<float> directionalRotation = new(-0.87266463f, 0.5235988f, 0.0f);
    #endregion

    #region Materials
    private SingleTextureMat singleTextureMat = null!;
    #endregion

    #region Lights
    private TrAmbientLight ambientLight = new(new Vector3D<float>(0.21176471f, 0.22745098f, 0.25882354f));
    private TrDirectionalLight directionalLight = new(new Vector3D<float>(1.0f, 0.9569f, 0.8392f), new Vector3D<float>(0.0f, 0.0f, -1.0f));
    #endregion

    protected override void Loaded()
    {
        capsule = Context.AssimpParsing("Resources/Models/Capsule.glb".PathFormatter())[0];

        singleTextureMat = new(Context);
    }

    protected override void UpdateScene(double deltaSeconds)
    {
    }

    protected override void RenderScene(double deltaSeconds)
    {
        Matrix4X4<float> model = Matrix4X4.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix4X4.CreateScale(scale) * Matrix4X4.CreateTranslation(translation);

        GlobalParameters parameters = new(Scene.Camera, model, Scene.SceneData, ambientLight, directionalLight);

        singleTextureMat.Draw(capsule, parameters);
    }

    protected override void EditProperties()
    {
        singleTextureMat.AdjustImGuiProperties();

        ImGui.SeparatorText("Transforms");

        Vector3 v1 = translation.ToSystem();
        ImGui.DragFloat3("Translation", ref v1, 0.01f);
        translation = v1.ToGeneric();

        Vector3 v2 = rotation.ToSystem();
        ImGui.DragFloat3("Rotation", ref v2, 0.01f);
        rotation = v2.ToGeneric();

        Vector3 v3 = scale.ToSystem();
        ImGui.DragFloat3("Scale", ref v3, 0.01f);
        scale = v3.ToGeneric();

        ImGui.SeparatorText("Lights");

        Vector3 v4 = ambientLight.Color.ToSystem();
        ImGui.ColorEdit3("Ambient Light", ref v4);
        ambientLight.Color = v4.ToGeneric();

        Vector3 v5 = directionalLight.Color.ToSystem();
        ImGui.ColorEdit3("Directional Light", ref v5);
        directionalLight.Color = v5.ToGeneric();

        Vector3 v6 = directionalRotation.RadianToDegree().ToSystem();
        ImGui.DragFloat3("Directional Rotation", ref v6, 0.5f, -360.0f, 360.0f);
        directionalRotation = v6.ToGeneric().DegreeToRadian();

        directionalLight.Direction = Vector3D.Transform(new Vector3D<float>(0.0f, 0.0f, -1.0f), Matrix4X4.CreateFromYawPitchRoll(directionalRotation.Y, directionalRotation.X, directionalRotation.Z));
    }

    protected override void Destroy(bool disposing = false)
    {
        singleTextureMat.Dispose();
        capsule.Dispose();
    }
}
