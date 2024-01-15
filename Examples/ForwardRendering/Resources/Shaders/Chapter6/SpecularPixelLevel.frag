#version 320 es

precision highp float;

in VertexData
{
    vec3 WorldNormal;
    vec3 WorldPos;
}
In;

out vec4 Out_Color;

layout(std140, binding = 1) uniform Vectors
{
    vec4 WorldSpaceCameraPos;
}

layout(std140, binding = 2) uniform Material
{
    vec4 Diffuse;
    vec4 Specular;
    float Gloss;
}
Uni_Material;

layout(std140, binding = 3) uniform AmbientLight
{
    vec3 Color;
}
Uni_AmbientLight;

layout(std140, binding = 4) uniform DirectionalLight
{
    vec3 Direction;
    vec3 Color;
}
Uni_DirectionalLight;

float saturate(float value)
{
    return clamp(value, 0.0, 1.0);
}

void main()
{
    vec3 worldNormal = normalize(In.worldNormal);
    vec3 worldLightDir = normalize(-Uni_DirectionalLight.Direction);

    vec3 diffuse = Uni_DirectionalLight.Color * Uni_Material.Diffuse.rgb * saturate(dot(worldNormal, worldLightDir));

    vec3 reflectDir = normalize(reflect(-worldLightDir, worldNormal));

    vec3 viewDir = normalize(WorldSpaceCameraPos.xyz - In.WorldPos);

    vec3 specular = Uni_DirectionalLight.Color * Uni_Material.Specular.rgb *
                    pow(saturate(dot(reflectDir, viewDir)), Uni_Material.Gloss);

    Out_Color = vec4(Uni_AmbientLight.Color + diffuse + specular, 1.0);
}