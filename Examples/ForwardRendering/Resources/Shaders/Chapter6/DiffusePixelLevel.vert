#version 460

layout(location = 0) in vec3 In_Position;
layout(location = 1) in vec3 In_Normal;
layout(location = 2) in vec2 In_TexCoord;

layout(location = 0) out VertexData
{
    vec3 Normal;
}
Out;

layout(std140, binding = 0) uniform Matrix
{
    mat4 ObjectToWorld;
    mat4 ObjectToClip;
    mat4 WorldToObject;
}
Uni_Matrix;

void main()
{
    Out.Normal = normalize(mat3(Uni_Matrix.WorldToObject) * In_Normal);

    gl_Position = Uni_Matrix.ObjectToClip * vec4(In_Position, 1.0);
}