#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtils.glsl"

layout(location = 0) out vec4 Out_Color;

layout(std140, binding = UNIFORM_BUFFER_BINDING_START + 0) uniform Parameters
{
    vec4 Color;
}
Uni_Parameters;

void main()
{
    Out_Color = Uni_Parameters.Color;
}