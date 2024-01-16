#version 460

layout(location = 0) in vec3 In_Position;
layout(location = 1) in vec3 In_Normal;
layout(location = 2) in vec2 In_TexCoord;

layout(location = 0) out VertexData
{
    vec3 WorldNormal;
    vec3 WorldPos;
}
Out;

layout(std140, binding = 0) uniform Transforms
{
    mat4 ObjectToWorld;
    mat4 ObjectToClip;
    mat4 WorldToObject;
}
Uni_Transforms;

void main()
{
    Out.WorldNormal = normalize(mat3(Uni_Transforms.WorldToObject) * In_Normal);
    Out.WorldPos = (Uni_Transforms.ObjectToWorld * vec4(In_Position, 1.0)).xyz;

    gl_Position = Uni_Transforms.ObjectToClip * vec4(In_Position, 1.0);
}