#version 320 es

precision highp float;

in vec3 In_Position;
in vec3 In_Normal;
in vec2 In_UV;

out vec4 Out_Color;

uniform vec4 Uni_Color;

void main()
{
    Out_Color = Uni_Color;
}