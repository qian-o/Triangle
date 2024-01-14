#version 320 es

precision highp float;

in VertexData
{
    vec4 Color;
}
In;

out vec4 Out_Color;

uniform vec4 Uni_Color;

void main()
{
    Out_Color = In.Color * Uni_Color;
}