#version 460

#include "TrShaderUtilities.glsl"
#include "TrVertex.glsl"

layout(location = 0) out VertexData
{
    vec3 WorldNormal;
    vec3 WorldPos;
}
Out;

void main()
{
    Out.WorldNormal = normalize(mat3(Uni_Transforms.WorldToObject) * In_Normal);
    Out.WorldPos = (Uni_Transforms.ObjectToWorld * vec4(In_Position, 1.0)).xyz;

    gl_Position = Uni_Transforms.ObjectToClip * vec4(In_Position, 1.0);
}