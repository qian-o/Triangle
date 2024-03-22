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
    float NearPlane;
    float FarPlane;
}
Uni_Parameters;

void main()
{
    float depthValue = SampleTexture(Channel0, In.UV).r;

    float depth = LinearizeDepth(depthValue, Uni_Parameters.NearPlane, Uni_Parameters.FarPlane);

    Out_Color = vec4(vec3(depth), 1.0);
}