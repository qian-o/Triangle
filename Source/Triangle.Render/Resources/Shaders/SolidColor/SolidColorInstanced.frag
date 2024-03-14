#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtils.glsl"

layout(location = 0) in VertexData
{
    flat int InstanceID;
}
In;

layout(location = 0) out vec4 Out_Color;

layout(binding = UNIFORM_SAMPLER_BINDING_START + 0) uniform sampler2D ColorSampler;

void main()
{
    Out_Color = SampleTexture(ColorSampler, ivec2(0, In.InstanceID), 0);
}