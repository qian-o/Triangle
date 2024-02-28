using Hexa.NET.ImGui;
using Triangle.Core.Contracts.Graphics;

namespace Triangle.Core.Graphics;

public abstract class TrMaterial(TrContext context, string name) : TrGraphics<TrContext>(context)
{
    private TrRenderPass? renderPass;

    public string Name => name;

    public TrRenderPass RenderPass => renderPass ??= CreateRenderPass();

    public abstract TrRenderPass CreateRenderPass();

    public abstract void Draw(TrMesh mesh, params object[] args);

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
