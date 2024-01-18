using Triangle.App.Applications;
using Triangle.App.Models;
using Triangle.Core.Helpers;

ShadercHelper.CompileSpirv("Resources/Shaders".PathFormatter());

using RenderHost<TutorialApplication> renderHost = new("Forward Rendering");
renderHost.Run();
