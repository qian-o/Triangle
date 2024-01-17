#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtils.glsl"

layout(location = 0) in VertexData
{
    vec3 WorldNormal;
    vec3 WorldPos;
    vec2 UV;
}
In;

layout(location = 0) out vec4 Out_Color;

layout(std140, binding = UNIFORM_BUFFER_BINDING_START + 0) uniform Material
{
    vec4 Color;
    vec4 Specular;
    float Gloss;
    vec4 MainTexST;
}
Uni_Material;

layout(binding = 0) uniform sampler2D MainTex;

void main()
{
    vec3 worldNormal = normalize(In.WorldNormal);
    vec3 worldLightDir = normalize(Uni_DirectionalLight.Position);

    vec3 albedo = texture(MainTex, TransformUV(In.UV, Uni_Material.MainTexST)).rgb * Uni_Material.Color.rgb;

    vec3 ambient = albedo * Uni_AmbientLight.Color;

    vec3 diffuse = albedo * Uni_DirectionalLight.Color * Saturate(dot(worldNormal, worldLightDir));

    vec3 viewDir = normalize(Uni_Vectors.CameraPosition - In.WorldPos);
    vec3 halfDir = normalize(worldLightDir + viewDir);
    vec3 specular = Uni_Material.Specular.rgb * Uni_DirectionalLight.Color *
                    pow(Saturate(dot(worldNormal, halfDir)), Uni_Material.Gloss);

    Out_Color = vec4(ambient + diffuse + specular, 1.0);
}