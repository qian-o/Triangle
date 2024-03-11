#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtils.glsl"
#include "../TrVertex.glsl"

layout(location = 0) out VertexData
{
    vec3 WorldPos;
}
Out;

layout(std140, binding = UNIFORM_BUFFER_BINDING_START + 0) uniform Parameters
{
    mat4 View;
    mat4 Projection;
    bool GammaCorrection;
    float Gamma;
    float Exposure;
}
Uni_Parameters;

void main()
{
    Out.WorldPos = In_Position;

    gl_Position = Uni_Parameters.Projection * Uni_Parameters.View * vec4(In_Position, 1.0);
}