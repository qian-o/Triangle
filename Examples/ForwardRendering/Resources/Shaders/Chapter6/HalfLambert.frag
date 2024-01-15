#version 320 es

precision highp float;

in VertexData
{
    vec3 Normal;
}
In;

out vec4 Out_Color;

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

void main()
{
    vec3 worldNormal = normalize(In.Normal);
    vec3 worldLightDir = normalize(Uni_DirectionalLight.Position);

    float halfLambert = dot(worldNormal, worldLightDir) * 0.5 + 0.5;

    vec3 diffuse = Uni_DirectionalLight.Color * Uni_Material.Diffuse.rgb * halfLambert;

    Out_Color = vec4(Uni_AmbientLight.Color + diffuse, 1.0);
}