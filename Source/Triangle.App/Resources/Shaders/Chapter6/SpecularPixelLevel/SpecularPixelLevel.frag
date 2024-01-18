#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtils.glsl"

layout(location = 0) in VertexData
{
    vec3 WorldNormal;
    vec3 WorldPos;
}
In;

layout(location = 0) out vec4 Out_Color;

layout(std140, binding = UNIFORM_BUFFER_BINDING_START + 0) uniform Material
{
    vec4 Diffuse;
    vec4 Specular;
    float Gloss;
}
Uni_Material;

void main()
{
    vec3 worldNormal = normalize(In.WorldNormal);
    vec3 worldLightDir = normalize(Uni_DirectionalLight.Position);

    vec3 diffuse = Uni_DirectionalLight.Color * Uni_Material.Diffuse.rgb * Saturate(dot(worldNormal, worldLightDir));

    vec3 reflectDir = normalize(reflect(-worldLightDir, worldNormal));

    vec3 viewDir = normalize(WorldSpaceViewDirection(In.WorldPos));

    vec3 specular = Uni_DirectionalLight.Color * Uni_Material.Specular.rgb *
                    pow(Saturate(dot(reflectDir, viewDir)), Uni_Material.Gloss);

    Out_Color = vec4(Uni_AmbientLight.Color + diffuse + specular, 1.0);
}