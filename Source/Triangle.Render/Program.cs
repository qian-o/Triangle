using Triangle.Core.Helpers;
using Triangle.Render.Applications;
using Triangle.Render.Models;

Dictionary<string,string> includes = new()
{
    { "TrVertex.glsl", "Resources/Shaders/TrVertex.glsl".Path() },
    { "TrUtils.glsl", "Resources/Shaders/TrUtils.glsl".Path() },
    { "TrUtilInstanced.glsl", "Resources/Shaders/TrUtilInstanced.glsl".Path() }
};

ShadercHelper.CompileSpirv("Resources/Shaders".Path(), includes);

using RenderHost<TutorialApplication> renderHost = new("Triangle Render");
renderHost.Run();
