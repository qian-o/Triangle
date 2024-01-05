#version 320 es

in vec3 In_Position;
in vec3 In_Normal;
in vec2 In_TexCoord;

out VertexData
{
    vec4 Color;
}
Out;

layout(std140, binding = 0) uniform Camera
{
    mat4 Model;
    mat4 View;
    mat4 Projection;
}
Uni_Camera;

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
    vec3 worldNormal = normalize(mat3(Uni_Camera.Model) * In_Normal);
    vec3 worldLightDir = normalize(Uni_DirectionalLight.Direction);

    vec3 diffuse = Uni_DirectionalLight.Color * Uni_Material.Diffuse.rgb * saturate(dot(worldNormal, worldLightDir));

    Out.Color = vec4(Uni_AmbientLight.Color + diffuse, 1.0);

    gl_Position = Uni_Camera.Projection * Uni_Camera.View * Uni_Camera.Model * vec4(In_Position, 1.0);
}