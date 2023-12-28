using Common;
using Common.Contracts;
using ForwardRendering.Materials;
using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.OpenGLES;
using Silk.NET.Windowing;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Triangle.Core;
using Triangle.Core.Graphics;
using Triangle.Render.Graphics;
using Triangle.Render.Helpers;

namespace ForwardRendering.Applications;

public class Application1 : IApplication
{
    #region IDisposable
    private bool disposedValue;
    #endregion

    #region Graphics
    private SimpleMat simpleMat = null!;

    private TrMesh cube = null!;
    #endregion

    ~Application1()
    {
        Dispose(disposing: false);
    }

    public IWindow Window { get; private set; } = null!;

    public TrContext Context { get; private set; } = null!;

    public void Initialize(IWindow window, TrContext context)
    {
        Window = window;
        Context = context;

        simpleMat = new(Context);

        cube = TrMeshFactory.CreateCube(Context, 0.5f);
    }

    public void Update(double deltaSeconds)
    {
    }

    public void Render([NotNull] Camera camera, [NotNull] TrFrame frame, double deltaSeconds)
    {
        GL gl = Context.GL;

        frame.Bind();

        gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);

        simpleMat.Draw(cube, camera);

        frame.Unbind();
    }

    public void DrawImGui()
    {
        ImGui.Begin("Application1");

        Vector4 color = simpleMat.Color.ToSystem();
        ImGui.ColorEdit4("SimpleMat.Color", ref color);
        simpleMat.Color = color.ToGeneric();

        ImGui.End();
    }

    public void Resize(Vector2D<int> size)
    {
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                cube.Dispose();
                simpleMat.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
