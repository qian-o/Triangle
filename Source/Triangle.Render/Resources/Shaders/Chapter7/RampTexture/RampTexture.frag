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
    vec4 Color;
    vec4 Specular;
    float Gloss;
}
Uni_Material;

void main()
{
    vec3 worldNormal = normalize(In.WorldNormal);
    vec3 worldLightDir = normalize(WorldSpaceDirectionalLightDir());

    vec3 ambient = Uni_AmbientLight.Color;

    float halfLambert = 0.5 * dot(worldNormal, worldLightDir) + 0.5;
    vec3 diffuseColor = SampleTexture(Channel0, vec2(halfLambert, halfLambert)).rgb * Uni_Material.Color.rgb;

    vec3 diffuse = Uni_DirectionalLight.Color * diffuseColor;

    vec3 viewDir = normalize(WorldSpaceViewDir(In.WorldPos));
    vec3 halfDir = normalize(worldLightDir + viewDir);
    vec3 specular = Uni_DirectionalLight.Color * Uni_Material.Specular.rgb *
                    pow(Saturate(dot(worldNormal, halfDir)), Uni_Material.Gloss);

    Out_Color = vec4(ambient + diffuse + specular, 1.0);
}