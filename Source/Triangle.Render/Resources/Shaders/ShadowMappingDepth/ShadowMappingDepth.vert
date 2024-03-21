#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtils.glsl"
#include "../TrVertex.glsl"

layout(std140, binding = UNIFORM_BUFFER_BINDING_START + 0) uniform Parameters
{
    mat4 LightSpaceMatrix;
}
Uni_Parameters;

void main()
{
    gl_Position = Uni_Parameters.LightSpaceMatrix * Uni_Transforms.ObjectToWorld * vec4(In_Position, 1.0);
}