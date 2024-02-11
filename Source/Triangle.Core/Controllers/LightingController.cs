using System.Numerics;
using Hexa.NET.ImGui;
using Silk.NET.Maths;
using Triangle.Core.Helpers;
using Triangle.Core.Structs;

namespace Triangle.Core.Controllers;

public class LightingController
{
    #region Ambient Light
    private Vector3D<float> ambientColor;
    #endregion

    #region Directional Light
    private Vector3D<float> directionalColor;
    private Vector3D<float> directionalRotation;
    #endregion

    public LightingController()
    {
        ambientColor = new(0.21176471f, 0.22745098f, 0.25882354f);

        directionalColor = new(1.0f, 0.9569f, 0.8392f);
        directionalRotation = new(-0.87266463f, 0.5235988f, 0.0f);
    }

    public TrAmbientLight AmbientLight => new(ambientColor);

    public TrDirectionalLight DirectionalLight => new(directionalColor, Vector3D.Transform(new Vector3D<float>(0.0f, 0.0f, -1.0f), Matrix4X4.CreateFromYawPitchRoll(directionalRotation.Y, directionalRotation.X, directionalRotation.Z)));

    public void Controller()
    {
        if (ImGui.TreeNode("Lighting"))
        {
            ImGui.PushID("Ambient Light");
            {
                Vector3 color = ambientColor.ToSystem();

                ImGui.Text("Ambient Light");
                ImGui.ColorEdit3("Color", ref color);

                ambientColor = color.ToGeneric();
            }
            ImGui.PopID();

            ImGui.PushID("Directional Light");
            {
                Vector3 color = directionalColor.ToSystem();
                Vector3 rotation = directionalRotation.RadianToDegree().ToSystem();

                ImGui.Text("Directional Light");
                ImGui.ColorEdit3("Color", ref color);
                ImGui.DragFloat3("Rotation", ref rotation, 0.1f);

                directionalColor = color.ToGeneric();
                directionalRotation = rotation.ToGeneric().DegreeToRadian();
            }
            ImGui.PopID();

            ImGui.TreePop();
        }
    }
}
