#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtils.glsl"
#include "../TrVertex.glsl"

layout(location = 0) out VertexData
{
    vec3 WorldPos;
    vec3 WorldNormal;
    vec4 LightSpacePos;
}
Out;

layout(std140, binding = UNIFORM_BUFFER_BINDING_START + 0) uniform Parameters
{
    mat4 LightSpace;
}
Uni_Parameters;

void main()
{
    Out.WorldPos = (Uni_Transforms.ObjectToWorld * vec4(In_Position, 1.0)).xyz;
    Out.WorldNormal = ObjectToWorldNormal(In_Normal);
    Out.LightSpacePos = Uni_Parameters.LightSpace * Uni_Transforms.ObjectToWorld * vec4(In_Position, 1.0);

    gl_Position = Uni_Transforms.ObjectToClip * vec4(In_Position, 1.0);
}