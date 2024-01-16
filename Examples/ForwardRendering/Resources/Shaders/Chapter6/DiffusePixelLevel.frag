#version 460

layout(location = 0) in VertexData
{
    vec3 Normal;
}
In;

layout(location = 0) out vec4 Out_Color;

layout(std140, binding = 1) uniform Material
{
    vec4 Diffuse;
}
Uni_Material;

layout(std140, binding = 2) uniform AmbientLight
{
    vec3 Color;
}
Uni_AmbientLight;

layout(std140, binding = 3) uniform DirectionalLight
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
    vec3 worldNormal = normalize(In.Normal);
    vec3 worldLightDir = normalize(Uni_DirectionalLight.Position);

    vec3 diffuse = Uni_DirectionalLight.Color * Uni_Material.Diffuse.rgb * saturate(dot(worldNormal, worldLightDir));

    Out_Color = vec4(Uni_AmbientLight.Color + diffuse, 1.0);
}