#version 460

layout(location = 0) in vec3 In_Position;
layout(location = 1) in vec3 In_Normal;

layout(location = 0) out VertexData
{
    vec4 Color;
}
Out;

layout(std140, binding = 0) uniform Matrices
{
    mat4 Model;
    mat4 View;
    mat4 Projection;
}
Uni_Matrices;

void main()
{
    Out.Color = vec4(In_Normal * 0.5 + vec3(0.5), 1.0);
    
    gl_Position = Uni_Matrices.Projection * Uni_Matrices.View * Uni_Matrices.Model * vec4(In_Position, 1.0);
}