#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtils.glsl"
#include "../../TrVertex.glsl"

layout(location = 0) out VertexData
{
    vec3 WorldNormal;
}
Out;

void main()
{
    Out.WorldNormal = normalize(mat3(Uni_Transforms.WorldToObject) * In_Normal);

    gl_Position = Uni_Transforms.ObjectToClip * vec4(In_Position, 1.0);
}