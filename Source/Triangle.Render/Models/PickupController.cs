using ImGuiNET;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Triangle.Core;
using Triangle.Core.Contracts;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Materials;

namespace Triangle.Render.Models;

public class PickupController(TrContext context, TrScene scene) : Disposable
{
    private readonly Random _random = new();
    private readonly List<(MeshModel Model, Vector4D<byte> Color)> _cache = [];

    private readonly TrScene _scene = scene;
    private readonly TrFrame _frame = new(context);
    private readonly SolidColorMat _solidColorMat = new(context);

    public MeshModel? PickupModel { get; private set; }

    public void Add(MeshModel model)
    {
        Vector4D<byte> color = new((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256), 255);

        _cache.Add((model, color));
    }

    public void Remove(MeshModel model)
    {
        int index = _cache.FindIndex(item => item.Model == model);

        if (index != -1)
        {
            _cache.RemoveAt(index);
        }
    }

    public void Render(GlobalParameters baseParameters)
    {
        _frame.Update(_scene.Width, _scene.Height, _scene.Samples);

        _frame.Bind();

        GL gl = context.GL;

        gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);

        foreach ((MeshModel model, Vector4D<byte> color) in _cache)
        {
            _solidColorMat.Color = new Vector4D<float>(color.X / 255.0f, color.Y / 255.0f, color.Z / 255.0f, color.W / 255.0f);

            model.Render(_solidColorMat, baseParameters);
        }

        _frame.Unbind();
    }

    public void Update()
    {
        if (_scene.IsFocused)
        {
            Vector4D<float> mouse = _scene.Mouse;

            if (mouse.Z == 1)
            {
                if (mouse.X >= 0 && mouse.Y >= 0 && mouse.X <= _scene.Width && mouse.Y <= _scene.Height)
                {
                    PickupModel = null;

                    Vector4D<byte> pickColor = _frame.GetPixel(Convert.ToInt32(mouse.X), Convert.ToInt32(mouse.Y));

                    foreach ((MeshModel model, Vector4D<byte> color) in _cache)
                    {
                        if (color == pickColor)
                        {
                            PickupModel = model;

                            break;
                        }
                    }
                }
            }
        }
    }

    public void ShowPickupFrame()
    {
        if (ImGui.Begin("Pickup"))
        {
            ImGuiHelper.Frame(_frame);

            ImGui.End();
        }
    }

    protected override void Destroy(bool disposing = false)
    {
        _frame.Dispose();
        _solidColorMat.Dispose();
    }
}
