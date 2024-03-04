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
    vec2 uv = SampleSphericalMap(normalize(In.WorldPos));

    Out_Color = vec4(SampleTexture(Channel0, uv).rgb, 1.0);
}