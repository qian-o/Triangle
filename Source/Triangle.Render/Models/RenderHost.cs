﻿using System.Runtime.InteropServices;
using Hexa.NET.ImGui;
using Hexa.NET.ImGuizmo;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Triangle.Core;
using Triangle.Core.Contracts;
using Triangle.Core.Controllers;
using Triangle.Core.Helpers;
using Triangle.Core.Structs;
using Triangle.Render.Contracts.Applications;

namespace Triangle.Render.Models;

public unsafe class RenderHost<TApplication> : Disposable where TApplication : IApplication, new()
{
    private readonly IWindow _window;
    private readonly IApplication _application;
    private readonly HashSet<string> _persistentMenuItems;
    private readonly Dictionary<string, bool> _persistentMenuStates;
    private readonly Dictionary<string, Action> _persistentMenuActions;

    #region Contexts
    private GL gl = null!;
    private IInputContext inputContext = null!;
    private TrContext trContext = null!;
    private ImGuiController imGuiController = null!;
    #endregion

    #region Parameters
    private bool firstFrame = true;
    #endregion

    public RenderHost(string title)
    {
        WindowOptions windowOptions = WindowOptions.Default;
        windowOptions.Title = title;
        windowOptions.API = new GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.ForwardCompatible, new APIVersion(4, 6));
        windowOptions.Size = new Vector2D<int>(1280, 720);
        windowOptions.WindowState = WindowState.Maximized;
        windowOptions.Samples = 8;
        windowOptions.VSync = false;
        windowOptions.PreferredDepthBufferBits = 32;
        windowOptions.PreferredStencilBufferBits = 32;
        windowOptions.PreferredBitDepth = new Vector4D<int>(8);

        _window = Window.Create(windowOptions);
        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
        _window.Resize += OnResize;
        _window.Closing += OnClosing;

        _application = new TApplication();

        _persistentMenuItems = [];
        _persistentMenuStates = [];
        _persistentMenuActions = [];
    }

    protected override void Destroy(bool disposing = false)
    {
        imGuiController.Dispose();
        trContext.Dispose();
        inputContext.Dispose();

        _application.Dispose();
    }

    private void OnLoad()
    {
        gl = _window.CreateOpenGL();
        inputContext = _window.CreateInput();
        trContext = new TrContext(gl);
        imGuiController = new ImGuiController(trContext,
                                              _window,
                                              inputContext,
                                              new ImGuiFontConfig("Resources/Fonts/MSYH.TTC", 14, (a) => (nint)a.Fonts.GetGlyphRangesChineseFull()));

        TrTextureManager.InitializeImages(trContext, "Resources/Textures".Path());

        _application.Initialize(_window, inputContext, trContext);
    }

    private void OnUpdate(double deltaSeconds)
    {
        _application.Update(deltaSeconds);
    }

    private void OnRender(double deltaSeconds)
    {
        // Clear the screen
        {
            gl.ColorMask(true, true, true, true);
            gl.DepthMask(true);
            gl.StencilMask(0xFF);
            gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit | (uint)GLEnum.StencilBufferBit);
        }

        _application.Render(deltaSeconds);

        gl.Viewport(0, 0, (uint)_window.Size.X, (uint)_window.Size.Y);
        gl.BindFramebuffer(GLEnum.Framebuffer, 0);

        imGuiController.Update((float)deltaSeconds);

        if (firstFrame)
        {
            ImGuiHelper.SetThemeByProwl();

            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            if (File.Exists($"{_application.GetType().Name}.ini"))
            {
                ImGui.LoadIniSettingsFromDisk($"{_application.GetType().Name}.ini");
            }

            ImGuizmo.AllowAxisFlip(false);

            firstFrame = false;
        }

        ImGui.DockSpaceOverViewport();

        _application.ImGuiRender();

        if (ImGui.Begin("Info"))
        {
            ImGui.Text($"Renderer : {Marshal.PtrToStringAnsi((nint)gl.GetString(GLEnum.Renderer))}");
            ImGui.Text($"Version : OpenGL {Marshal.PtrToStringAnsi((nint)gl.GetString(GLEnum.Version))}");
            ImGui.Text($"Vendor : {Marshal.PtrToStringAnsi((nint)gl.GetString(GLEnum.Vendor))}");
            ImGui.Text($"FPS : {ImGui.GetIO().Framerate}");

            ImGui.End();
        }

        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Settings"))
            {
                PersistentMenuItem("Style Editor", ImGui.ShowStyleEditor);

                if (ImGui.MenuItem("Save Layout"))
                {
                    ImGui.SaveIniSettingsToDisk($"{_application.GetType().Name}.ini");
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Tools"))
            {
                PersistentMenuItem("Texture Manager", TrTextureManager.Manager);

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }

        ExecutePersistentMenu();

        imGuiController.Render();
    }

    private void OnResize(Vector2D<int> size)
    {
        _application.WindowResize(size);
    }

    private void OnClosing()
    {
        Dispose(true);
    }

    public void Run() => _window.Run();

    private void PersistentMenuItem(string label, Action action)
    {
        _persistentMenuItems.Add(label);
        _persistentMenuStates.TryAdd(label, false);
        _persistentMenuActions.TryAdd(label, action);

        if (ImGui.MenuItem(label))
        {
            _persistentMenuStates[label] = true;
        }
    }

    private void ExecutePersistentMenu()
    {
        foreach (string label in _persistentMenuItems)
        {
            if (_persistentMenuStates[label])
            {
                _persistentMenuStates[label] = ImGuiHelper.ShowDialog(label, _persistentMenuActions[label]);
            }
        }
    }
}
