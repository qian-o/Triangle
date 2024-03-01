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
    Out_Color = SampleTexture(Channel0, In.UV);
}