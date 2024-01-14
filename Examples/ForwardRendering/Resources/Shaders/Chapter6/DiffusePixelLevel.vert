#version 320 es

in vec3 In_Position;
in vec3 In_Normal;
in vec2 In_TexCoord;

out VertexData
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