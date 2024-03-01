#version 460

layout(location = 0) in vec3 In_Position;

layout(std140, binding = 0) uniform Transforms
{
    mat4 Model;
    mat4 View;
    mat4 Projection;
    mat4 ObjectToWorld;
    mat4 ObjectToClip;
    mat4 WorldToObject;
}
Uni_Transforms;

void main()
{
    gl_Position = Uni_Transforms.ObjectToClip * vec4(In_Position, 1.0);
}