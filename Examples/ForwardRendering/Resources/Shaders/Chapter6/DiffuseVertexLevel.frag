#version 460 core

layout(location = 0) in VertexData
{
    vec4 Color;
}
In;

layout(location = 0) out vec4 Out_Color;

void main()
{
    Out_Color = In.Color;
}