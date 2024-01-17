#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtils.glsl"
#include "../../TrVertex.glsl"

layout(location = 0) out VertexData
{
    vec3 Color;
}
Out;

layout(std140, binding = UNIFORM_BUFFER_BINDING_START + 0) uniform Material
{
    vec4 Diffuse;
    vec4 Specular;
    float Gloss;
}
Uni_Material;

void main()
{
    vec3 worldNormal = normalize(mat3(Uni_Transforms.WorldToObject) * In_Normal);
    vec3 worldLightDir = normalize(Uni_DirectionalLight.Position);

    vec3 diffuse = Uni_DirectionalLight.Color * Uni_Material.Diffuse.rgb * Saturate(dot(worldNormal, worldLightDir));

    vec3 reflectDir = normalize(reflect(-worldLightDir, worldNormal));

    vec3 viewDir = normalize(WorldSpaceViewDirection(vec3(Uni_Transforms.ObjectToWorld * vec4(In_Position, 1.0))));

    vec3 specular = Uni_DirectionalLight.Color * Uni_Material.Specular.rgb *
                    pow(Saturate(dot(reflectDir, viewDir)), Uni_Material.Gloss);

    Out.Color = Uni_AmbientLight.Color + diffuse + specular;

    gl_Position = Uni_Transforms.ObjectToClip * vec4(In_Position, 1.0);
}
