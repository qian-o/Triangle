#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtils.glsl"

layout(location = 0) in VertexData
{
    vec3 WorldPos;
}
In;

layout(location = 0) out vec4 Out_Color;

void main()
{
    vec3 color = SampleTexture(Map0, In.WorldPos).rgb;

    Out_Color = vec4(color, 1.0);
}