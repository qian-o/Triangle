#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtils.glsl"
#include "../../TrVertex.glsl"

layout(location = 0) out VertexData
{
    vec2 UV;
    vec3 LightDir;
    vec3 ViewDir;
}
Out;

void main()
{
    Out.UV = TransformUV(In_TexCoord, Uni_TexScaleOffset.Channel0ST);

    TANGENT_SPACE_ROTATION;

    Out.LightDir = rotation * ObjSpaceDirectionalLightDir();
    Out.ViewDir = rotation * ObjSpaceViewDir(In_Position);

    gl_Position = ObjectToClipPos(In_Position);
}