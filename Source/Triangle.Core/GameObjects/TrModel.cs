using System.Collections.ObjectModel;
using Hexa.NET.ImGui;
using Triangle.Core.Contracts.GameObjects;
using Triangle.Core.Graphics;

namespace Triangle.Core.GameObjects;

public class TrModel(string name) : TrGameObject(name)
{
    private readonly TrMesh[] _meshes = [];
    private readonly TrMaterial[] _materials = [];
    private readonly Dictionary<TrMesh, TrMaterial> _indexer = [];

    public TrModel(string name, TrMesh[] meshes, TrMaterial materials) : this(name)
    {
        _meshes = meshes;
        _materials = new TrMaterial[meshes.Length];

        for (int i = 0; i < _meshes.Length; i++)
        {
            _materials[i] = materials;
            _indexer.Add(_meshes[i], materials);
        }
    }

    public TrModel(string name, TrMesh[] meshes, TrMaterial[] materials) : this(name)
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

    public TrModel(string name, TrMesh[] meshes, TrMaterial[] materials, Dictionary<TrMesh, TrMaterial> indexer) : this(name)
    {
        if (meshes.Length != materials.Length && meshes.Length != indexer.Count)
        {
            throw new ArgumentException("The number of meshes and materials must be the same.");
        }

        _meshes = meshes;
        _materials = materials;
        _indexer = indexer;
    }

    public ReadOnlyCollection<TrMesh> Meshes => new(_meshes);

    protected override void OtherPropertyEditor()
    {
        ImGui.Text("Materials");
        ImGui.Separator();

        for (int i = 0; i < _materials.Length; i++)
        {
            if (i == 0)
            {
                ImGui.SetNextItemOpen(true, ImGuiCond.Once);
            }

            _materials[i].Controller($"Material {i}");
        }
    }

    /// <summary>
    /// This method is empty and should be overridden by the user.
    /// </summary>
    public virtual void Render() { }

    /// <summary>
    /// Renders the model with the given arguments.
    /// </summary>
    /// <param name="args">Before passing to the material, the model's transformation matrix will be added to the list of arguments.</param>
    public void Render(params object[] args)
    {
        foreach (TrMesh mesh in Meshes)
        {
            _indexer[mesh].Draw(mesh, [Transform.Model, .. args]);
        }
    }

    /// <summary>
    /// Renders the model with the given material and arguments.
    /// </summary>
    /// <param name="material">material</param>
    /// <param name="args">Before passing to the material, the model's transformation matrix will be added to the list of arguments.</param>
    public void Render(TrMaterial material, params object[] args)
    {
        foreach (TrMesh mesh in Meshes)
        {
            material.Draw(mesh, [Transform.Model, .. args]);
        }
    }
}
