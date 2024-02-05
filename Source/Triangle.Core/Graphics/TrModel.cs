using System.Collections.ObjectModel;
using ImGuiNET;
using Silk.NET.Maths;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Models;

namespace Triangle.Core.Graphics;

public class TrModel : TrGraphics<TrContext>
{
    private readonly TransformController _transformController;
    private readonly TrMesh[] _meshes;
    private readonly TrMaterial[] _materials;
    private readonly Dictionary<int, int> _meshMaterialMap;

    public TrModel(TrContext context, string name, TransformController transformController) : base(context)
    {
        Name = name;
        _transformController = transformController;
        _meshes = [];
        _materials = [];
        _meshMaterialMap = [];
    }

    public TrModel(TrContext context, string name, TransformController transformController, TrMesh[] meshes, TrMaterial material) : base(context)
    {
        Name = name;

        _transformController = transformController;
        _meshes = meshes;
        _materials = [material];
        _meshMaterialMap = [];

        for (int i = 0; i < meshes.Length; i++)
        {
            _meshMaterialMap.Add(i, 0);
        }
    }

    public TrModel(TrContext context, string name, TransformController transformController, TrMesh[] meshes, TrMaterial[] materials) : base(context)
    {
        Name = name;

        _transformController = transformController;
        _meshes = meshes;
        _materials = materials;
        _meshMaterialMap = [];

        for (int i = 0; i < meshes.Length; i++)
        {
            _meshMaterialMap.Add(i, i);
        }
    }

    public TrModel(TrContext context, string name, TransformController transformController, TrMesh[] meshes, TrMaterial[] materials, Dictionary<int, int> meshMaterialMap) : base(context)
    {
        Name = name;

        _transformController = transformController;
        _meshes = meshes;
        _materials = materials;
        _meshMaterialMap = meshMaterialMap;
    }

    public string Name { get; }

    public ReadOnlyCollection<TrMesh> Meshes => new(_meshes);

    public ReadOnlyCollection<TrMaterial> Materials => new(_materials);

    public Matrix4X4<float> Transform => _transformController[Name];

    public void AdjustProperties()
    {
        ImGui.PushID(GetHashCode());
        {
            _transformController.Controller(Name);

            for (int i = 0; i < _materials.Length; i++)
            {
                _materials[i].AdjustProperties();
            }

            ImGui.TreePop();
        }
        ImGui.PopID();
    }

    protected override void Destroy(bool disposing = false)
    {
        if (disposing)
        {
            foreach (var mesh in _meshes)
            {
                mesh.Dispose();
            }

            foreach (var material in _materials)
            {
                material.Dispose();
            }
        }
    }
}
