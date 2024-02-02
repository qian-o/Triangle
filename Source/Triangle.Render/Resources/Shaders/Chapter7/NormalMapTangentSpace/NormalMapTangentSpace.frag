#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtils.glsl"

layout(location = 0) in VertexData
{
    vec4 UV;
    vec3 LightDir;
    vec3 ViewDir;
}
In;

layout(location = 0) out vec4 Out_Color;

layout(std140, binding = UNIFORM_BUFFER_BINDING_START + 0) uniform Material
{
    vec4 Color;
    float BumpScale;
    vec4 Specular;
    float Gloss;
}
Uni_Material;

void main()
{
    vec3 tangentLightDir = normalize(In.LightDir);
    vec3 tangentViewDir = normalize(In.ViewDir);

    vec4 packedNormal = texture(Channel1, In.UV.xy);
    vec3 tangentNormal = UnpackNormal(packedNormal);
    tangentNormal.xy *= Uni_Material.BumpScale;
    tangentNormal.z = sqrt(1.0 - Saturate(dot(tangentNormal.xy, tangentNormal.xy)));

    vec3 albedo = texture(Channel0, In.UV.xy).rgb * Uni_Material.Color.rgb;

    vec3 ambient = Uni_AmbientLight.Color * albedo;

    vec3 diffent = Uni_DirectionalLight.Color * albedo * Saturate(dot(tangentLightDir, tangentNormal));

    vec3 halfDir = normalize(tangentLightDir + tangentViewDir);
    vec3 specent = Uni_DirectionalLight.Color * Uni_Material.Specular.rgb *
                   pow(Saturate(dot(halfDir, tangentNormal)), Uni_Material.Gloss);

    Out_Color = vec4(ambient + diffent + specent, 1.0);
}