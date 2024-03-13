using Hexa.NET.ImGui;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.GameObjects;

namespace Triangle.Core.Graphics;

public abstract class TrMaterial(TrContext context, string name) : TrGraphics<TrContext>(context)
{
    private TrRenderPass? renderPass;

    public string Name => name;

    public TrRenderPass RenderPass => renderPass ??= CreateRenderPass();

    protected abstract TrRenderPass CreateRenderPass();

    public abstract void Draw(IEnumerable<TrMesh> meshes, params object[] args);

    public abstract void Draw(IEnumerable<TrModel> models, params object[] args);

    public void Controller(string name = "")
    {
        if (string.IsNullOrEmpty(name))
        {
            name = $"{Name} Material";
        }

        ImGui.PushID(name);
        {
            if (ImGui.TreeNode(name))
            {
                ControllerCore();

                ImGui.TreePop();
            }
        }
        ImGui.PopID();
    }

    protected abstract void ControllerCore();
}
