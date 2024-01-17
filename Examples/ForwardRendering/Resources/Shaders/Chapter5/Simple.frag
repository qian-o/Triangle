#version 460

#include "../TrShaderUtilities.glsl"

layout(location = 0) in VertexData
{
    vec4 Color;
}
In;

layout(location = 0) out vec4 Out_Color;

layout(std140, binding = UNIFORM_BUFFER_BINDING_START + 0) uniform Parameters
{
    vec4 Color;
}
Uni_Parameters;

void main()
{
    Out_Color = In.Color * Uni_Parameters.Color;
}