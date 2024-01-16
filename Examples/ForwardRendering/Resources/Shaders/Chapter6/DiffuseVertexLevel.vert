#version 460

layout(location = 0) in vec3 In_Position;
layout(location = 1) in vec3 In_Normal;
layout(location = 2) in vec2 In_TexCoord;

layout(location = 0) out VertexData
{
    vec4 Color;
}
Out;

layout(std140, binding = 0) uniform Matrix
{
    mat4 ObjectToWorld;
    mat4 ObjectToClip;
    mat4 WorldToObject;
}
Uni_Matrix;

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
    vec3 worldNormal = normalize(mat3(Uni_Matrix.WorldToObject) * In_Normal);
    vec3 worldLightDir = normalize(Uni_DirectionalLight.Position);

    vec3 diffuse = Uni_DirectionalLight.Color * Uni_Material.Diffuse.rgb * saturate(dot(worldNormal, worldLightDir));

    Out.Color = vec4(Uni_AmbientLight.Color + diffuse, 1.0);

    gl_Position = Uni_Matrix.ObjectToClip * vec4(In_Position, 1.0);
}