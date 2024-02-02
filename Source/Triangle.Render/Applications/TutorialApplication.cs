using System.ComponentModel;
using System.Numerics;
using System.Reflection;
using ImGuiNET;
using Silk.NET.Maths;
using Triangle.Core.Graphics;
using Triangle.Core.Helpers;
using Triangle.Core.Structs;
using Triangle.Core.Widgets;
using Triangle.Core.Widgets.Layouts;
using Triangle.Render.Contracts.Applications;
using Triangle.Render.Contracts.Tutorials;

namespace Triangle.Render.Applications;

public class TutorialApplication : BaseApplication
{
    private readonly List<ITutorial> _tutorials = [];
    private readonly TrWrapPanel _allTutorials = new();

    private ITutorial? _lastFocusedTutorial;

    public override void Loaded()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetInterfaces().Contains(typeof(ITutorial)) && !x.IsAbstract).ToArray();

        _allTutorials.Clear();
        foreach (Type type in types)
        {
            string displayName = type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? type.Name;
            string description = type.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "";

            _allTutorials.Add(new TrControl(64.0f, 46.0f)
            {
                Margin = new TrThickness(2.0f),
                Tag = (type, displayName, description)
            });
        }
    }

    public override void Update(double deltaSeconds)
    {
        foreach (ITutorial tutorial in _tutorials)
        {
            tutorial.Update(deltaSeconds);
        }
    }

    public override void Render(double deltaSeconds)
    {
        foreach (ITutorial tutorial in _tutorials)
        {
            tutorial.Render(deltaSeconds);
        }
    }

    public override void ImGuiRender()
    {
        if (ImGui.Begin("Tutorials"))
        {
            Vector2 windowSize = ImGui.GetWindowSize();
            Vector2 contentSize = ImGui.GetContentRegionAvail();

            Vector2 leftTop = ImGui.GetCursorPos();
            Vector2 rightBottom = windowSize - (leftTop + contentSize);
            TrThickness windowPadding = new(leftTop.X, leftTop.Y, rightBottom.X, rightBottom.Y);

            _allTutorials.Width = contentSize.X;
            _allTutorials.Height = contentSize.Y;
            _allTutorials.Measure(windowSize.ToGeneric(), windowPadding);

            foreach (TrControl control in _allTutorials.Children)
            {
                control.Render((r) =>
                {
                    (Type type, string displayName, string description) = (ValueTuple<Type, string, string>)control.Tag!;

                    ImGui.SetCursorPos(r.Position.ToSystem());

                    ImGuiHelper.Button(displayName,
                                       () => { _tutorials.Add((ITutorial)Activator.CreateInstance(type, Input, Context)!); },
                                       r.Size);

                    ImGuiHelper.ShowHelpMarker(description);
                });
            }

            ImGui.End();
        }

        _tutorials.RemoveAll(x =>
        {
            if (x.Scene.IsClosed)
            {
                if (_lastFocusedTutorial == x)
                {
                    _lastFocusedTutorial = null;
                }

                x.Dispose();
            }

            return x.Scene.IsClosed;
        });

        ITutorial? tut = _lastFocusedTutorial;
        foreach (ITutorial tutorial in _tutorials)
        {
            TrScene scene = tutorial.Scene;

            uint id = ImGui.DockSpaceOverViewport(ImGui.FindViewportByID(ImGui.GetID(scene.HostName)));

            ImGui.SetNextWindowDockID(id, ImGuiCond.Once);

            scene.DrawHost();

            if (scene.IsFocused)
            {
                tut = tutorial;
            }
        }

        _lastFocusedTutorial = tut;
        _lastFocusedTutorial?.ImGuiRender();
    }

    public override void WindowResize(Vector2D<int> size)
    {
    }

    protected override void Destroy(bool disposing = false)
    {
        _lastFocusedTutorial = null;

        foreach (ITutorial tutorial in _tutorials)
        {
            tutorial.Dispose();
        }
        _tutorials.Clear();
    }
}
