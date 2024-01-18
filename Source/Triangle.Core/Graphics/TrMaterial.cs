using System.Diagnostics.CodeAnalysis;
using ImGuiNET;
using Triangle.Core.Contracts.Graphics;

namespace Triangle.Core.Graphics;

public abstract class TrMaterial<TParameters> : TrGraphics<TrContext>
{
    private TrRenderPass? renderPass;

    protected TrMaterial(TrContext context, string name) : base(context)
    {
        Name = name;
    }

    public string Name { get; }

    public TrRenderPass RenderPass => renderPass ??= CreateRenderPass();

    public abstract TrRenderPass CreateRenderPass();

    public abstract void Draw([NotNull] TrMesh mesh, [NotNull] TParameters parameters);

    public void AdjustImGuiProperties()
    {
        ImGui.PushID(GetHashCode());

        ImGui.SeparatorText($"{Name} Material");

        AdjustImGuiPropertiesCore();

        ImGui.PopID();
    }

    protected abstract void AdjustImGuiPropertiesCore();
}
