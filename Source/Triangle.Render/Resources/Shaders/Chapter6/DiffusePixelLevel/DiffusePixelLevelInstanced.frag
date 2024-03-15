#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtils.glsl"

layout(location = 0) in VertexData
{
    flat int InstanceID;
    vec3 WorldNormal;
}
In;

layout(location = 0) out vec4 Out_Color;

layout(binding = UNIFORM_SAMPLER_BINDING_START + 0) uniform sampler2D DiffuseSampler;

void main()
{
    vec3 worldNormal = normalize(In.WorldNormal);
    vec3 worldLightDir = normalize(Uni_DirectionalLight.Position);

    vec3 diffuse = Uni_DirectionalLight.Color * SampleTexture(DiffuseSampler, ivec2(0, In.InstanceID), 0).rgb *
                   Saturate(dot(worldNormal, worldLightDir));

    Out_Color = vec4(Uni_AmbientLight.Color + diffuse, 1.0);
}