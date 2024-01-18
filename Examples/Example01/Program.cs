using Common;
using Example01.Applications;
using Triangle.Core.Helpers;

ShadercHelper.CompileSpirv("Resources/Shaders".PathFormatter());

using RenderHost<TutorialApplication> renderHost = new("Forward Rendering");
renderHost.Run();
