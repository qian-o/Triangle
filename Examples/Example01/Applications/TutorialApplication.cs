using System.ComponentModel;
using System.Reflection;
using Common.Contracts.Applications;
using Common.Models;
using Example01.Contracts.Tutorials;
using ImGuiNET;
using Silk.NET.Maths;
using Triangle.Core.Helpers;

namespace Example01.Applications;

public class TutorialApplication : BaseApplication
{
    private readonly List<ITutorial> _tutorials = [];
    private readonly List<(string DisplayName, string Description, Type Type)> _allTutorials = [];

    private ITutorial? _lastFocusedTutorial;

    public override void Loaded()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetInterfaces().Contains(typeof(ITutorial)) && !x.IsAbstract).ToArray();

        _allTutorials.Clear();
        foreach (Type type in types)
        {
            string displayName = type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? type.Name;
            string description = type.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "";

            _allTutorials.Add((displayName, description, type));
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
            foreach ((string DisplayName, string Description, Type Type) in _allTutorials)
            {
                ImGuiHelper.Button(DisplayName, () =>
                {
                    _tutorials.Add((ITutorial)Activator.CreateInstance(Type, Input, Context, DisplayName)!);

                }, height: 40.0f);
                ImGuiHelper.ShowHelpMarker(Description);

                ImGui.SameLine();
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
