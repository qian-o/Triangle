#version 460 core

layout(location = 0) in vec3 In_Position;
layout(location = 1) in vec3 In_Normal;

layout(location = 0) out VertexData
{
    vec4 Color;
}
Out;

uniform mat4 Uni_Model;
uniform mat4 Uni_View;
uniform mat4 Uni_Projection;

void main()
{
    Out.Color = vec4(In_Normal * 0.5 + vec3(0.5), 1.0);

    gl_Position = Uni_Projection * Uni_View * Uni_Model * vec4(In_Position, 1.0);
}