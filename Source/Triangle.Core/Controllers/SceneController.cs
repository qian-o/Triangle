using System.Collections.ObjectModel;
using Hexa.NET.ImGui;
using Hexa.NET.ImGuizmo;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;

namespace Triangle.Core.Controllers;

public class SceneController
{
    private readonly TrScene _scene;
    private readonly Dictionary<string, TrGameObject> _cache;
    private readonly List<string> _selectedObjects;

    public SceneController(TrScene scene)
    {
        _scene = scene;
        _cache = [];
        _selectedObjects = [];

        Add(_scene.Camera);

        _scene.DrawContentInWindow += Scene_DrawContentInWindow;
    }

    ~SceneController()
    {
        _scene.DrawContentInWindow -= Scene_DrawContentInWindow;
    }

    public TrGameObject this[string name] => _cache[name];

    public ReadOnlyCollection<TrGameObject> Objects => _cache.Values.ToList().AsReadOnly();

    public ReadOnlyCollection<TrGameObject> SelectedObjects => _selectedObjects.Select(name => _cache[name]).ToList().AsReadOnly();

    public void Add(TrGameObject gameObject)
    {
        _cache.Add(gameObject.Name, gameObject);
    }

    public void Remove(TrGameObject gameObject)
    {
        _cache.Remove(gameObject.Name);
    }

    public void SelectObjects(TrGameObject[] gameObjects)
    {
        if (gameObjects.Except(Objects).Any())
        {
            throw new InvalidOperationException("One or more game objects are not in the scene.");
        }

        _selectedObjects.Clear();
        _selectedObjects.AddRange(gameObjects.Select(x => x.Name));
    }

    public void Clear()
    {
        _cache.Clear();
    }

    public void Controller()
    {
        if (ImGui.Begin("Scene Collection"))
        {
            ImGui.SetNextItemOpen(true, ImGuiCond.Once);

            if (ImGui.TreeNode("Collection"))
            {
                bool isMultiSelect = _scene.KeyPressed(Key.ControlLeft) || _scene.KeyPressed(Key.ControlRight);

                foreach (string name in _cache.Keys)
                {
                    ImGui.PushID(name.GetHashCode());
                    {
                        bool isSelected = _selectedObjects.Contains(name);

                        if (ImGui.Selectable(name, isSelected))
                        {
                            if (isMultiSelect)
                            {
                                _selectedObjects.Remove(name);
                            }
                            else
                            {
                                _selectedObjects.Clear();
                            }

                            if (!isMultiSelect || isMultiSelect && !isSelected)
                            {
                                _selectedObjects.Add(name);
                            }
                        }
                    }
                    ImGui.PopID();
                }

                ImGui.TreePop();
            }

            ImGui.End();
        }
    }

    public void PropertyEditor()
    {
        if (ImGui.Begin("Property Editor"))
        {
            if (SelectedObjects.LastOrDefault() is TrGameObject gameObject)
            {
                gameObject.PropertyEditor();
            }

            ImGui.End();
        }
    }

    private void Scene_DrawContentInWindow()
    {
        ReadOnlyCollection<TrGameObject> selectedObjects = SelectedObjects;

        if (selectedObjects.Count == 0)
        {
            return;
        }

        Vector3D<float> center = selectedObjects.Select(x => x.Transform.Position).Aggregate((x, y) => x + y) / selectedObjects.Count;

        Matrix4X4<float> showMatrix = Matrix4X4.CreateTranslation(center);
        if (selectedObjects.Count == 1)
        {
            showMatrix = selectedObjects.First().Transform.Model;
        }

        float[] viewArray = _scene.Camera.View.ToArray();
        float[] projectionArray = _scene.Camera.Projection.ToArray();
        float[] showMatrixArray = showMatrix.ToArray();
        if (ImGuizmo.Manipulate(ref viewArray[0], ref projectionArray[0], _scene.GizmosOperation, _scene.GizmosSpace, ref showMatrixArray[0]))
        {

        }
    }
}
