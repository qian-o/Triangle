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
    bool GammaCorrection;
    float Gamma;
    float Exposure;
}
Uni_Parameters;

void main()
{
    vec3 color = SampleTexture(Channel0, In.UV).rgb;

    if (Uni_Parameters.GammaCorrection)
    {
        color = ApplyGammaCorrection(color, Uni_Parameters.Gamma, Uni_Parameters.Exposure);
    }

    Out_Color = vec4(color, 1.0);
}