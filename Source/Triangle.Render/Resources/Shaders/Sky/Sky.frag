#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtils.glsl"

layout(location = 0) in VertexData
{
    vec2 UV;
}
In;

layout(location = 0) out vec4 Out_Color;

void main()
{
    vec3 color = SampleTexture(Channel0, In.UV).rgb;

    Out_Color = vec4(GammaCorrection(color), 1.0);
}