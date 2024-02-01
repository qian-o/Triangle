using System.Numerics;
using ImGuiNET;
using Silk.NET.Maths;
using Triangle.Core.Helpers;

namespace Triangle.Core.Models;

public class TransformController
{
    private readonly Dictionary<string, (Vector3D<float> Translation, Vector3D<float> Rotation, Vector3D<float> Scale)> _cache;

    public TransformController()
    {
        _cache = [];
    }

    public Matrix4X4<float> this[string name] => GetModelMatrix(name);

    public void Add(string name)
    {
        _cache.Add(name, (new(0.0f, 0.0f, 0.0f), new(0.0f, 0.0f, 0.0f), new(1.0f, 1.0f, 1.0f)));
    }

    public void Remove(string name)
    {
        _cache.Remove(name);
    }

    public void Clear()
    {
        _cache.Clear();
    }

    public void Controller()
    {
        if (ImGui.TreeNode("Transform"))
        {
            foreach ((string name, (Vector3D<float> translation, Vector3D<float> rotation, Vector3D<float> scale)) in _cache)
            {
                ImGui.PushID(name);
                {
                    Vector3 t = translation.ToSystem();
                    Vector3 r = rotation.RadianToDegree().ToSystem();
                    Vector3 s = scale.ToSystem();

                    ImGui.Text(name);
                    ImGui.DragFloat3("Translation", ref t, 0.01f);
                    ImGui.DragFloat3("Rotation", ref r, 0.1f);
                    ImGui.DragFloat3("Scale", ref s, 0.01f);

                    _cache[name] = (t.ToGeneric(), r.ToGeneric().DegreeToRadian(), s.ToGeneric());
                }
                ImGui.PopID();
            }

            ImGui.TreePop();
        }
    }

    private Matrix4X4<float> GetModelMatrix(string name)
    {
        (Vector3D<float> translation, Vector3D<float> rotation, Vector3D<float> scale) = _cache[name];

        return Matrix4X4.CreateScale(scale) * Matrix4X4.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix4X4.CreateTranslation(translation);
    }
}
