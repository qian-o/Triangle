using System.Diagnostics.CodeAnalysis;
using ImGuiNET;
using Triangle.Core;
using Triangle.Core.Contracts.Graphics;
using Triangle.Core.Graphics;

namespace Triangle.Render.Graphics;

public abstract class TrMaterial<TParameter> : TrGraphics<TrContext>
{
    private TrRenderPass? renderPass;

    protected TrMaterial(TrContext context, string name) : base(context)
    {
        Name = name;
    }

    public string Name { get; }

    public TrRenderPass RenderPass => renderPass ??= CreateRenderPass();

    public abstract TrRenderPass CreateRenderPass();

    public abstract void Draw([NotNull] TrMesh mesh, [NotNull] TParameter parameter);

    public void AdjustImGuiProperties()
    {
        ImGui.PushID(GetHashCode());

        ImGui.SeparatorText($"{Name} Material");

        AdjustImGuiPropertiesCore();

        ImGui.PopID();
    }

    protected abstract void AdjustImGuiPropertiesCore();
}
