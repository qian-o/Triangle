using Triangle.Core.Helpers;
using Triangle.Render.Applications;
using Triangle.Render.Models;

ShadercHelper.CompileSpirv("Resources/Shaders".Path());

using RenderHost<TutorialApplication> renderHost = new("Triangle Render");
renderHost.Run();
