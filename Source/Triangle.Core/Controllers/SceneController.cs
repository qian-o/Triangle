using System.Collections.ObjectModel;
using Hexa.NET.ImGui;
using Hexa.NET.ImGuizmo;
using Silk.NET.Input;
using Silk.NET.Maths;
using Triangle.Core.Contracts.GameObjects;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;

namespace Triangle.Core.Controllers;

public delegate void GameObjectsChanged();

public class SceneController
{
    private readonly TrScene _scene;
    private readonly Dictionary<string, TrGameObject> _cache;
    private readonly Dictionary<string, List<TrGameObject>> _groupCache;
    private readonly List<string> _selectedObjects;

    private string currentGroup;

    public event GameObjectsChanged? ObjectsChanged;
    public event GameObjectsChanged? SelectedObjectsChanged;

    public SceneController(TrScene scene)
    {
        _scene = scene;
        _cache = [];
        _groupCache = new()
        {
            { string.Empty, [] }
        };
        _selectedObjects = [];

        currentGroup = string.Empty;

        Add(_scene.Camera);

        _scene.DrawContentInWindow += Scene_DrawContentInWindow;
    }

    ~SceneController()
    {
        _scene.DrawContentInWindow -= Scene_DrawContentInWindow;
    }

    public TrGameObject this[string name] => _cache[name];

    public ReadOnlyCollection<TrGameObject> Objects => _cache.Values.ToArray().AsReadOnly();

    public ReadOnlyCollection<TrGameObject> SelectedObjects => _selectedObjects.Select(name => _cache[name]).ToArray().AsReadOnly();

    public bool IsTransformObject { get; private set; }

    public void BeginGroup(string name)
    {
        if (!_groupCache.ContainsKey(name))
        {
            _groupCache.Add(name, []);
        }

        currentGroup = name;
    }

    public void EndGroup()
    {
        currentGroup = string.Empty;
    }

    public void Add(TrGameObject gameObject)
    {
        _cache.Add(gameObject.Name, gameObject);
        _groupCache[currentGroup].Add(gameObject);

        ObjectsChanged?.Invoke();
    }

    public void Remove(TrGameObject gameObject)
    {
        _cache.Remove(gameObject.Name);
        _groupCache[currentGroup].Remove(gameObject);

        ObjectsChanged?.Invoke();
    }

    public void SelectObjects(TrGameObject[] gameObjects)
    {
        if (gameObjects.Except(Objects).Any())
        {
            throw new InvalidOperationException("One or more game objects are not in the scene.");
        }

        _selectedObjects.Clear();
        _selectedObjects.AddRange(gameObjects.Select(x => x.Name));

        SelectedObjectsChanged?.Invoke();
    }

    public void Clear()
    {
        _cache.Clear();
    }

    public void Controller()
    {
        if (ImGui.Begin("Scene Collection"))
        {
            if (ImGui.TreeNode("Properties"))
            {
                int samples = _scene.Samples;
                ImGuiHelper.DragInt("Samples", ref samples, 1, 1, 16);
                _scene.Samples = samples;

                ImGui.TreePop();
            }

            ImGui.SetNextItemOpen(true, ImGuiCond.Once);

            if (ImGui.TreeNode("Collection"))
            {
                bool isMultiSelect = _scene.KeyPressed(Key.ControlLeft) || _scene.KeyPressed(Key.ControlRight);

                foreach (string group in _groupCache.Keys)
                {
                    ImGui.PushID(group);
                    {
                        bool isTreeNode = group != string.Empty && _groupCache[group].Count > 0;

                        if (isTreeNode)
                        {
                            if (ImGui.TreeNode(group))
                            {
                                _groupCache[group].ForEach(AddGameObjectNode);

                                ImGui.TreePop();
                            }
                        }
                        else
                        {
                            _groupCache[group].ForEach(AddGameObjectNode);
                        }

                        void AddGameObjectNode(TrGameObject gameObject)
                        {
                            ImGui.PushID(gameObject.Name);
                            {
                                bool isSelected = _selectedObjects.Contains(gameObject.Name);

                                if (ImGui.Selectable(gameObject.Name, isSelected))
                                {
                                    if (isMultiSelect)
                                    {
                                        _selectedObjects.Remove(gameObject.Name);
                                    }
                                    else
                                    {
                                        _selectedObjects.Clear();
                                    }

                                    if (!isMultiSelect || isMultiSelect && !isSelected)
                                    {
                                        _selectedObjects.Add(gameObject.Name);
                                    }

                                    SelectedObjectsChanged?.Invoke();
                                }
                            }
                            ImGui.PopID();
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
        if (IsTransformObject = ImGuizmo.Manipulate(ref viewArray[0], ref projectionArray[0], _scene.GizmosOperation, _scene.GizmosSpace, ref showMatrixArray[0]))
        {
            Matrix4X4.Decompose(showMatrixArray.ToMatrix(), out Vector3D<float> scale, out Quaternion<float> rotation, out Vector3D<float> translation);

            if (selectedObjects.Count == 1)
            {
                TrGameObject gameObject = selectedObjects.First();

                gameObject.Transform.Position = translation;
                gameObject.Transform.Rotation = rotation;
                gameObject.Transform.Scale = scale;
            }
            else
            {
                Vector3D<float> deltaScale = scale - Vector3D<float>.One;

                foreach (TrGameObject gameObject in selectedObjects)
                {
                    Vector3D<float> offset = gameObject.Transform.Position - center;
                    offset = Vector3D.Transform(offset, rotation);

                    gameObject.Transform.Position = translation + (offset * scale);
                    gameObject.Transform.Rotation *= rotation;
                    gameObject.Transform.Scale += deltaScale;
                }
            }
        }
    }
}
