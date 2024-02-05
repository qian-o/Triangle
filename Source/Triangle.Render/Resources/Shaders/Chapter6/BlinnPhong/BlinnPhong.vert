#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtils.glsl"
#include "../../TrVertex.glsl"

layout(location = 0) out VertexData
{
    vec3 WorldNormal;
    vec3 WorldPos;
}
Out;

void main()
{
    Out.WorldNormal = ObjectToWorldNormal(In_Normal);
    Out.WorldPos = (Uni_Transforms.ObjectToWorld * vec4(In_Position, 1.0)).xyz;

    gl_Position = Uni_Transforms.ObjectToClip * vec4(In_Position, 1.0);
}