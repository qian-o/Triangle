using System.Collections.ObjectModel;
using Hexa.NET.ImGui;
using Silk.NET.Maths;
using Triangle.Core.Controllers;
using Triangle.Core.Helpers;
using Triangle.Render.Contracts.Materials;
using Triangle.Render.Models;

namespace Triangle.Core.Graphics;

public class MeshModel
{
    private readonly TransformController _transformController;
    private readonly TrMesh[] _meshes;
    private readonly GlobalMat[] _materials;
    private readonly Dictionary<TrMesh, GlobalMat> _indexer;

    private MeshModel(TransformController transformController, string name)
    {
        _transformController = transformController;
        _meshes = [];
        _materials = [];
        _indexer = [];

        Name = name;

        _transformController.Add(Name);
    }

    public MeshModel(TransformController transformController, string name, TrMesh[] meshes, GlobalMat materials) : this(transformController, name)
    {
        _meshes = meshes;
        _materials = new GlobalMat[meshes.Length];

        for (int i = 0; i < _meshes.Length; i++)
        {
            _materials[i] = materials;
            _indexer.Add(_meshes[i], materials);
        }
    }

    public MeshModel(TransformController transformController, string name, TrMesh[] meshes, GlobalMat[] materials) : this(transformController, name)
    {
        if (meshes.Length != materials.Length)
        {
            throw new ArgumentException("The number of meshes and materials must be the same.");
        }

        _meshes = meshes;
        _materials = materials;

        for (int i = 0; i < _meshes.Length; i++)
        {
            _indexer.Add(_meshes[i], _materials[i]);
        }
    }

    public MeshModel(TransformController transformController, string name, TrMesh[] meshes, GlobalMat[] materials, Dictionary<TrMesh, GlobalMat> indexer) : this(transformController, name)
    {
        if (meshes.Length != materials.Length && meshes.Length != indexer.Count)
        {
            throw new ArgumentException("The number of meshes and materials must be the same.");
        }

        _meshes = meshes;
        _materials = materials;
        _indexer = indexer;
    }

    public string Name { get; }

    public ReadOnlyCollection<TrMesh> Meshes => new(_meshes);

    public Matrix4X4<float> Transform => _transformController[Name];

    public void SetTranslation(Vector3D<float> translation)
    {
        _transformController.SetTransform(Name, translation: translation);
    }

    public void SetRotation(Vector3D<float> rotation)
    {
        _transformController.SetTransform(Name, rotation: rotation);
    }

    public void SetRotationByDegree(Vector3D<float> rotationDegree)
    {
        _transformController.SetTransform(Name, rotation: rotationDegree.DegreeToRadian());
    }

    public void SetScale(Vector3D<float> scale)
    {
        _transformController.SetTransform(Name, scale: scale);
    }

    public void SetTransform(Matrix4X4<float> transform)
    {
        _transformController.SetTransform(Name, transform);
    }

    /// <summary>
    /// 绘制所有网格。
    /// 用于 GlobalParameters 为引用类型，所以内部在使用的时候会进行Copy，不需要担心参数被修改。
    /// </summary>
    /// <param name="baseParameters">baseParameters</param>
    public void Render(GlobalParameters baseParameters)
    {
        GlobalParameters temp = baseParameters.Copy();
        temp.Model = Transform;

        foreach (TrMesh mesh in Meshes)
        {
            _indexer[mesh].Draw(mesh, temp);
        }
    }

    /// <summary>
    /// 使用指定的材质绘制所有网格。
    /// 关于 GlobalParameters 的使用同上。
    /// </summary>
    /// <param name="material">material</param>
    /// <param name="baseParameters">baseParameters</param>
    public void Render(GlobalMat material, GlobalParameters baseParameters)
    {
        GlobalParameters temp = baseParameters.Copy();
        temp.Model = Transform;

        foreach (TrMesh mesh in Meshes)
        {
            material.Draw(mesh, temp);
        }
    }

    public void Controller()
    {
        ImGui.SetNextItemOpen(true, ImGuiCond.Once);
        if (ImGui.TreeNode($"{Name} - Properties"))
        {
            ImGui.PushID(Name);
            {
                _transformController.Controller(Name);

                for (int i = 0; i < _materials.Length; i++)
                {
                    _materials[i].Controller($"Material {i}");
                }
            }
            ImGui.PopID();

            ImGui.TreePop();
        }
    }
}
