using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core;
using Triangle.Core.Contracts;
using Triangle.Core.Controllers;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Render.Materials;
using Triangle.Render.Models;

namespace Triangle.Render.Controllers;

/// <summary>
/// 拾取控制器。
/// 基于场景控制器进行拾取。
/// </summary>
/// <param name="context">context</param>
/// <param name="scene">scene</param>
/// <param name="sceneController">sceneController</param>
public class PickupController(TrContext context, TrScene scene, SceneController sceneController) : Disposable
{
    // 选中的颜色。
    public static readonly Vector4D<byte> PickupColor = new(255, 255, 255, 255);

    private readonly TrContext _context = context;
    private readonly TrScene _scene = scene;
    private readonly SceneController _sceneController = sceneController;

    private readonly TrFrame _frame = new(context);
    private readonly SolidColorMat _solidColorMat = new(context);

    private readonly TrFrame _pickupFrame = new(context);
    private readonly TrMesh _pickupMesh = context.CreateCanvas();
    private readonly EdgeDetectionMat _edgeDetectionMat = new(context);

    /// <summary>
    /// 绘制描边效果。
    /// </summary>
    /// <param name="baseParameters">baseParameters</param>
    public void PostEffects(GlobalParameters baseParameters)
    {
        _edgeDetectionMat.Channel0 = _pickupFrame.Texture;

        _edgeDetectionMat.Draw(_pickupMesh, baseParameters);
    }

    /// <summary>
    /// 更新拾取。
    /// </summary>
    public void Update()
    {
        _frame.Update(_scene.Width, _scene.Height);
        _pickupFrame.Update(_scene.Width, _scene.Height, _scene.Samples);

        if (_scene.IsFocused && _scene.IsLeftClicked && !_scene.IsOperationTools)
        {
            Vector2D<float> point = new(_scene.Mouse.X, _scene.Mouse.Y);

            Rectangle<float> rectangle = new(0.0f, 0.0f, _scene.Width, _scene.Height);

            if (rectangle.Contains(point))
            {
                MeshModel[] meshModels = _sceneController.Objects.Where(x => x is MeshModel).Cast<MeshModel>().ToArray();
                List<MeshModel> selectedMeshModels = [.. _sceneController.SelectedObjects.Where(x => x is MeshModel).Cast<MeshModel>()];

                bool anySelected = false;
                bool isMultiSelect = _scene.KeyPressed(Key.ControlLeft) || _scene.KeyPressed(Key.ControlRight);

                Vector4D<byte> pickColor = _frame.GetPixel(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));

                foreach (MeshModel meshModel in meshModels)
                {
                    if (meshModel.ColorId == pickColor)
                    {
                        anySelected = true;

                        bool isSelected = selectedMeshModels.Contains(meshModel);

                        if (isMultiSelect)
                        {
                            selectedMeshModels.Remove(meshModel);
                        }
                        else
                        {
                            selectedMeshModels.Clear();
                        }

                        if (!isMultiSelect || isMultiSelect && !isSelected)
                        {
                            selectedMeshModels.Add(meshModel);
                        }

                        break;
                    }
                }

                if (!anySelected)
                {
                    selectedMeshModels.Clear();
                }

                _sceneController.SelectObjects(selectedMeshModels.ToArray());
            }
        }
    }

    /// <summary>
    /// 绘制可被拾取的模型和已拾取的模型。
    /// </summary>
    /// <param name="baseParameters"></param>
    public void Render(GlobalParameters baseParameters)
    {
        MeshModel[] meshModels = _sceneController.Objects.Where(x => x is MeshModel).Cast<MeshModel>().ToArray();
        MeshModel[] selectedMeshModels = _sceneController.SelectedObjects.Where(x => x is MeshModel).Cast<MeshModel>().ToArray();

        _frame.Bind();
        {
            _context.Clear();

            foreach (MeshModel model in meshModels)
            {
                _solidColorMat.Color = model.ColorId.ToSingle();

                model.Render(_solidColorMat, baseParameters);
            }
        }
        _frame.Unbind();

        _pickupFrame.Bind();
        {
            _context.Clear();

            foreach (MeshModel model in selectedMeshModels)
            {
                _solidColorMat.Color = PickupColor.ToSingle();

                model.Render(_solidColorMat, baseParameters);
            }
        }
        _pickupFrame.Unbind();
    }

    protected override void Destroy(bool disposing = false)
    {
        _frame.Dispose();
        _solidColorMat.Dispose();

        _pickupFrame.Dispose();
        _pickupMesh.Dispose();
        _edgeDetectionMat.Dispose();
    }
}
