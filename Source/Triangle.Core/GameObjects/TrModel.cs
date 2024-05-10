using System.Collections.ObjectModel;
using Hexa.NET.ImGui;
using Triangle.Core.Contracts.GameObjects;
using Triangle.Core.Graphics;

namespace Triangle.Core.GameObjects;

public class TrModel(string name) : TrGameObject(name)
{
    private readonly TrMesh[] _meshes = [];
    private readonly TrMaterial[] _materials = [];

    public TrModel(string name, TrMesh[] meshes, TrMaterial materials) : this(name)
    {
        _meshes = meshes;
        _materials = [materials];
    }

    public TrModel(string name, TrMesh[] meshes, TrMaterial[] materials) : this(name)
    {
        if (meshes.Length == 0 || materials.Length == 0)
        {
            throw new ArgumentException("Meshes and materials must not be empty.");
        }

        if (meshes.Select(item => item.MaterialIndex).Max() >= materials.Length)
        {
            throw new ArgumentException("The material index of the mesh must be less than the number of materials.");
        }

        _meshes = meshes;
        _materials = materials;
    }

    public ReadOnlyCollection<TrMesh> Meshes => new(_meshes);

    public ReadOnlyCollection<TrMaterial> Materials => new(_materials);

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
            _materials[mesh.MaterialIndex >= _materials.Length ? 0 : mesh.MaterialIndex].Draw([mesh], [Transform.Model, .. args]);
        }
    }

    /// <summary>
    /// Renders the model with the given material and arguments.
    /// </summary>
    /// <param name="material">material</param>
    /// <param name="args">Before passing to the material, the model's transformation matrix will be added to the list of arguments.</param>
    public void Render(TrMaterial material, params object[] args)
    {
        material.Draw([.. Meshes], [Transform.Model, .. args]);
    }

    protected override void Destroy(bool disposing = false)
    {
    }
}
