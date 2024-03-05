#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtils.glsl"
#include "../TrVertex.glsl"

layout(location = 0) out VertexData
{
    vec2 UV;
}
Out;

void main()
{
    Out.UV = In_TexCoord;

    gl_Position = vec4(In_Position, 1.0);
}