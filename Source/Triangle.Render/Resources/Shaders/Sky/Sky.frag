#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtils.glsl"

layout(location = 0) in VertexData
{
    vec2 UV;
}
In;

layout(location = 0) out vec4 Out_Color;

layout(std140, binding = UNIFORM_BUFFER_BINDING_START + 0) uniform Parameters
{
    float Exposure;
}
Uni_Parameters;

void main()
{
    vec3 color = SampleTexture(Channel0, In.UV).rgb;

    Out_Color = vec4(ApplyGammaCorrection(color, Uni_Parameters.Exposure), 1.0);
}