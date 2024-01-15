#version 320 es

in vec3 In_Position;
in vec3 In_Normal;
in vec2 In_TexCoord;

out VertexData
{
    vec3 Color;
}
Out;

layout(std140, binding = 0) uniform Transforms
{
    mat4 ObjectToWorld;
    mat4 ObjectToClip;
    mat4 WorldToObject;
}
Uni_Transforms;

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
    vec3 worldNormal = normalize(mat3(Uni_Transforms.WorldToObject) * In_Normal);
    vec3 worldLightDir = normalize(Uni_DirectionalLight.Position);

    vec3 diffuse = Uni_DirectionalLight.Color * Uni_Material.Diffuse.rgb * saturate(dot(worldNormal, worldLightDir));

    vec3 reflectDir = normalize(reflect(-worldLightDir, worldNormal));

    vec3 viewDir = normalize(Uni_Vectors.CameraPosition - vec3(Uni_Transforms.ObjectToWorld * vec4(In_Position, 1.0)));

    vec3 specular = Uni_DirectionalLight.Color * Uni_Material.Specular.rgb *
                    pow(saturate(dot(reflectDir, viewDir)), Uni_Material.Gloss);

    Out.Color = Uni_AmbientLight.Color + diffuse + specular;

    gl_Position = Uni_Transforms.ObjectToClip * vec4(In_Position, 1.0);
}
