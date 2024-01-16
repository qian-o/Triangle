#version 460

layout(location = 0) in VertexData
{
    vec3 WorldNormal;
    vec3 WorldPos;
}
In;

layout(location = 0) out vec4 Out_Color;

layout(std140, binding = 1) uniform Vectors
{
    vec3 CameraPosition;
}
Uni_Vectors;

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
    vec3 Position;
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
    vec3 worldNormal = normalize(In.WorldNormal);
    vec3 worldLightDir = normalize(Uni_DirectionalLight.Position);

    vec3 diffuse = Uni_DirectionalLight.Color * Uni_Material.Diffuse.rgb * saturate(dot(worldNormal, worldLightDir));

    vec3 reflectDir = normalize(reflect(-worldLightDir, worldNormal));

    vec3 viewDir = normalize(Uni_Vectors.CameraPosition - In.WorldPos);

    vec3 specular = Uni_DirectionalLight.Color * Uni_Material.Specular.rgb *
                    pow(saturate(dot(reflectDir, viewDir)), Uni_Material.Gloss);

    Out_Color = vec4(Uni_AmbientLight.Color + diffuse + specular, 1.0);
}