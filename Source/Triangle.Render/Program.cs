using Triangle.Core.Helpers;
using Triangle.Render.Applications;
using Triangle.Render.Models;

ShadercHelper.CompileSpirv("Resources/Shaders".PathFormatter());

using RenderHost<TutorialApplication> renderHost = new("Forward Rendering");
renderHost.Run();
