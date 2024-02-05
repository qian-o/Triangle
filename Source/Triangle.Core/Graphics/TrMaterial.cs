﻿using ImGuiNET;
using Triangle.Core.Contracts.Graphics;

namespace Triangle.Core.Graphics;

public abstract class TrMaterial : TrGraphics<TrContext>
{
    private TrRenderPass? renderPass;

    protected TrMaterial(TrContext context, string name) : base(context)
    {
        Name = name;
    }

    public string Name { get; }

    public TrRenderPass RenderPass => renderPass ??= CreateRenderPass();

    public abstract TrRenderPass CreateRenderPass();

    public abstract void Draw(TrMesh mesh, object[] args);

    public void AdjustProperties()
    {
        ImGui.PushID(GetHashCode());
        {
            if (ImGui.TreeNode($"{Name} Material"))
            {
                AdjustPropertiesCore();

                ImGui.TreePop();
            }
        }
        ImGui.PopID();
    }

    protected abstract void AdjustPropertiesCore();
}

public abstract class TrMaterial<TParameters>(TrContext context, string name) : TrMaterial(context, name)
{
    public override void Draw(TrMesh mesh, object[] args)
    {
        if (args.Length == 1 && args[0] is TParameters parameters)
        {
            Draw(mesh, parameters);
        }
    }

    public abstract void Draw(TrMesh mesh, TParameters parameters);
}
