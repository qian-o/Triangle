#version 460

layout(location = 0) in VertexData
{
    vec4 Color;
}
In;

layout(location = 0) out vec4 Out_Color;

uniform vec4 Uni_Color;

void main()
{
    Out_Color = In.Color * Uni_Color;
}