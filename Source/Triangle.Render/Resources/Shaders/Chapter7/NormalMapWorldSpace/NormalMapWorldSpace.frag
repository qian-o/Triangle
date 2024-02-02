#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtils.glsl"

layout(location = 0) in VertexData
{
    vec4 UV;
    vec4 TtoW0;
    vec4 TtoW1;
    vec4 TtoW2;
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
    vec3 worldPos = vec3(In.TtoW0.w, In.TtoW1.w, In.TtoW2.w);
    vec3 lightDir = normalize(WorldSpaceDirectionalLightDir());
    vec3 viewDir = normalize(WorldSpaceViewDir(worldPos));

    vec3 bump = UnpackNormal(texture(Channel1, In.UV.zw));
    bump.xy *= Uni_Material.BumpScale;
    bump.z = sqrt(1.0 - Saturate(dot(bump.xy, bump.xy)));
    bump = normalize(vec3(dot(In.TtoW0.xyz, bump), dot(In.TtoW1.xyz, bump), dot(In.TtoW2.xyz, bump)));

    vec3 albedo = texture(Channel0, In.UV.xy).rgb * Uni_Material.Color.rgb;

    vec3 ambient = Uni_AmbientLight.Color * albedo;

    vec3 diffent = Uni_DirectionalLight.Color * albedo * Saturate(dot(lightDir, bump));

    vec3 halfDir = normalize(lightDir + viewDir);
    vec3 specent =
        Uni_DirectionalLight.Color * Uni_Material.Specular.rgb * pow(Saturate(dot(halfDir, bump)), Uni_Material.Gloss);

    Out_Color = vec4(ambient + diffent + specent, 1.0);
}