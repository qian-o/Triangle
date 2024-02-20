using System.Numerics;
using Hexa.NET.ImGui;
using Silk.NET.Maths;
using Triangle.Core.Graphics;

namespace Triangle.Core.Controllers;

public class TransformController
{
    private readonly Dictionary<string, TrGameObject> _cache;

    public TransformController()
    {
        _cache = [];
    }

    public TrTransform this[string name] => _cache[name].Transform;

    public void Add(TrGameObject gameObject)
    {
        _cache.Add(gameObject.Name, gameObject);
    }

    public void Remove(TrGameObject gameObject)
    {
        _cache.Remove(gameObject.Name);
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

        TrTransform transform = this[name];

        if (translation != null)
        {
            transform.Position = translation.Value;
        }

        if (rotation != null)
        {
            transform.LocalEulerAngles = rotation.Value;
        }

        if (scale != null)
        {
            transform.Scale = scale.Value;
        }
    }

    public void SetTransform(string name, Matrix4X4<float> matrix)
    {
        Matrix4X4.Decompose(matrix, out Vector3D<float> scale, out Quaternion<float> rotation, out Vector3D<float> translation);

        TrTransform transform = this[name];

        transform.Position = translation;
        transform.LocalRotation = rotation;
        transform.Scale = scale;
    }

    public void Controller(string name)
    {
        ImGui.PushID(name);
        {
            ImGui.SetNextItemOpen(true, ImGuiCond.Once);
            if (ImGui.TreeNode("Transform"))
            {
                TrTransform transform = this[name];

                Vector3 t = transform.Position.ToSystem();
                Vector3 r = transform.LocalEulerAngles.ToSystem();
                Vector3 s = transform.Scale.ToSystem();

                ImGui.DragFloat3("Translation", ref t, 0.01f);
                ImGui.DragFloat3("Rotation", ref r, 0.01f);
                ImGui.DragFloat3("Scale", ref s, 0.01f);

                SetTransform(name, t.ToGeneric(), r.ToGeneric(), s.ToGeneric());

                ImGui.TreePop();
            }
        }
        ImGui.PopID();
    }
}
