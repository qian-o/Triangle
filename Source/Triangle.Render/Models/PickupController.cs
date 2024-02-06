using ImGuiNET;
using Silk.NET.Input;
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
    public static readonly Vector4D<byte> PickupColor = new(255, 255, 255, 255);

    private readonly Random _random = new();
    private readonly List<(MeshModel Model, Vector4D<byte> Color)> _cache = [];
    private readonly List<MeshModel> _selectedModels = [];

    private readonly TrScene _scene = scene;
    private readonly TrFrame _frame = new(context);
    private readonly SolidColorMat _solidColorMat = new(context);

    private readonly TrFrame _pickupFrame = new(context);
    private readonly TrMesh _pickupMesh = context.CreateCanvas();
    private readonly EdgeDetectionMat _edgeDetectionMat = new(context);

    public IReadOnlyCollection<MeshModel> PickupModels => _selectedModels;

    public void Add(MeshModel model)
    {
        Vector4D<byte> color = RandomColor();

        while (true)
        {
            if (color == PickupColor || _cache.Any(item => item.Color == color))
            {
                color = RandomColor();
            }
            else
            {
                break;
            }
        }

        _cache.Add((model, color));

        Vector4D<byte> RandomColor()
        {
            return new((byte)_random.Next(0, 256), (byte)_random.Next(0, 256), (byte)_random.Next(0, 256), 255);
        }
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
        GL gl = context.GL;

        _frame.Bind();
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);

            foreach ((MeshModel model, Vector4D<byte> color) in _cache)
            {
                _solidColorMat.Color = new(color.X / 255.0f, color.Y / 255.0f, color.Z / 255.0f, color.W / 255.0f);

                model.Render(_solidColorMat, baseParameters);
            }
        }
        _frame.Unbind();

        _pickupFrame.Bind();
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);

            foreach (MeshModel model in _selectedModels)
            {
                _solidColorMat.Color = new(PickupColor.X / 255.0f, PickupColor.Y / 255.0f, PickupColor.Z / 255.0f, PickupColor.W / 255.0f);

                model.Render(_solidColorMat, baseParameters);
            }
        }
        _pickupFrame.Unbind();
    }

    public void PostEffects(TrFrame frame, GlobalParameters baseParameters)
    {
        frame.Bind();
        {
            _edgeDetectionMat.Channel0 = _pickupFrame.Texture;

            _edgeDetectionMat.Draw(_pickupMesh, baseParameters);
        }
        frame.Unbind();
    }

    public void Update()
    {
        if (_scene.IsFocused && _scene.IsLeftClicked)
        {
            Vector2D<float> point = new(_scene.Mouse.X, _scene.Mouse.Y);

            Rectangle<float> rectangle = new(0.0f, 0.0f, _scene.Width, _scene.Height);

            if (rectangle.Contains(point))
            {
                bool anySelected = false;
                bool isMultiSelect = _scene.KeyPressed(Key.ControlLeft) || _scene.KeyPressed(Key.ControlRight);

                Vector4D<byte> pickColor = _frame.GetPixel(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));

                foreach ((MeshModel model, Vector4D<byte> color) in _cache)
                {
                    if (color == pickColor)
                    {
                        anySelected = true;

                        bool isSelected = _selectedModels.Contains(model);

                        if (isMultiSelect)
                        {
                            _selectedModels.Remove(model);
                        }
                        else
                        {
                            _selectedModels.Clear();
                        }

                        if (!isMultiSelect || (isMultiSelect && !isSelected))
                        {
                            _selectedModels.Add(model);
                        }

                        break;
                    }
                }

                if (!anySelected)
                {
                    _selectedModels.Clear();
                }
            }
        }

        _frame.Update(_scene.Width, _scene.Height, _scene.Samples);
        _pickupFrame.Update(_scene.Width, _scene.Height, _scene.Samples);
    }

    public void Controller()
    {
        bool isMultiSelect = _scene.KeyPressed(Key.ControlLeft) || _scene.KeyPressed(Key.ControlRight);

        if (ImGui.Begin("Scene Collection"))
        {
            ImGui.SetNextItemOpen(true, ImGuiCond.Once);
            if (ImGui.TreeNode("Collection"))
            {
                foreach ((MeshModel model, Vector4D<byte> _) in _cache)
                {
                    ImGui.PushID(model.GetHashCode());

                    bool isSelected = _selectedModels.Contains(model);
                    if (ImGui.Selectable(model.Name, isSelected))
                    {
                        if (isMultiSelect)
                        {
                            _selectedModels.Remove(model);
                        }
                        else
                        {
                            _selectedModels.Clear();
                        }

                        if (!isMultiSelect || (isMultiSelect && !isSelected))
                        {
                            _selectedModels.Add(model);
                        }
                    }

                    ImGui.PopID();
                }

                ImGui.TreePop();
            }

            ImGui.End();
        }
    }

    protected override void Destroy(bool disposing = false)
    {
        _frame.Dispose();
        _solidColorMat.Dispose();
    }
}
