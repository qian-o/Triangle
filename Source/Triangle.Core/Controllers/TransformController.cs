using System.Numerics;
using Hexa.NET.ImGui;
using Silk.NET.Maths;
using Triangle.Core.Helpers;

namespace Triangle.Core.Controllers;

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

    public void SetTransform(string name, Vector3D<float>? translation = null, Vector3D<float>? rotation = null, Vector3D<float>? scale = null)
    {
        if (translation is null && rotation is null && scale is null)
        {
            return;
        }

        (Vector3D<float> t, Vector3D<float> r, Vector3D<float> s) = _cache[name];

        _cache[name] = (translation ?? t, rotation ?? r, scale ?? s);
    }

    public void SetTransform(string name, Matrix4X4<float> matrix)
    {
        Matrix4X4.Decompose(matrix, out Vector3D<float> scale, out Quaternion<float> rotation, out Vector3D<float> translation);

        _cache[name] = (translation, rotation.ToRotation(), scale);
    }

    public void Controller(string name)
    {
        ImGui.PushID(name);
        {
            ImGui.SetNextItemOpen(true, ImGuiCond.Once);
            if (ImGui.TreeNode("Transform"))
            {
                (Vector3D<float> translation, Vector3D<float> rotation, Vector3D<float> scale) = _cache[name];

                Vector3 t = translation.ToSystem();
                Vector3 r = rotation.RadianToDegree().ToSystem();
                Vector3 s = scale.ToSystem();

                ImGui.DragFloat3("Translation", ref t, 0.01f);
                ImGui.DragFloat3("Rotation", ref r, 0.1f);
                ImGui.DragFloat3("Scale", ref s, 0.01f);

                _cache[name] = (t.ToGeneric(), r.ToGeneric().DegreeToRadian(), s.ToGeneric());

                ImGui.TreePop();
            }
        }
        ImGui.PopID();
    }

    private Matrix4X4<float> GetModelMatrix(string name)
    {
        (Vector3D<float> translation, Vector3D<float> rotation, Vector3D<float> scale) = _cache[name];

        return Matrix4X4.CreateScale(scale) * Matrix4X4.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix4X4.CreateTranslation(translation);
    }
}
