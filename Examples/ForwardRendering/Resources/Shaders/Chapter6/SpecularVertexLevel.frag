#version 320 es

precision highp float;

in VertexData
{
    vec3 Color;
}
In;

out vec4 Out_Color;

void main()
{
    Out_Color = vec4(In.Color, 1.0);
}