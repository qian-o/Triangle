using System.Collections.ObjectModel;
using System.Numerics;
using Hexa.NET.ImGui;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core.Graphics;

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
                ImGui.PushID(gameObject.GetHashCode());
                {
                    ImGui.Text(gameObject.Name);
                    ImGui.Separator();

                    TrTransform transform = gameObject.Transform;

                    Vector3 t = transform.Position.ToSystem();
                    Vector3 r = transform.EulerAngles.ToSystem();
                    Vector3 s = transform.Scale.ToSystem();

                    ImGui.DragFloat3("Translation", ref t, 0.01f);
                    ImGui.SliderFloat3("Rotation", ref r, -360.0f, 360.0f);
                    ImGui.DragFloat3("Scale", ref s, 0.01f);

                    transform.Position = t.ToGeneric();
                    transform.EulerAngles = r.ToGeneric();
                    transform.Scale = s.ToGeneric();

                    if (gameObject is TrCamera camera)
                    {
                        ImGui.Text("Camera");
                        ImGui.Separator();

                        float cameraSpeed = camera.Speed;
                        ImGui.SliderFloat("Speed", ref cameraSpeed, 0.1f, 10.0f);
                        camera.Speed = cameraSpeed;

                        float cameraSensitivity = camera.Sensitivity;
                        ImGui.SliderFloat("Sensitivity", ref cameraSensitivity, 0.1f, 1.0f);
                        camera.Sensitivity = cameraSensitivity;
                    }
                }
                ImGui.PopID();
            }

            ImGui.End();
        }
    }
}
