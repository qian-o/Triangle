#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtils.glsl"

layout(location = 0) in VertexData
{
    vec3 Color;
}
In;

layout(location = 0) out vec4 Out_Color;

void main()
{
    Out_Color = vec4(In.Color, 1.0);
}