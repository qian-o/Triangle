#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtilInstanced.glsl"

layout(location = 0) in VertexData
{
    flat int InstanceID;
    vec3 WorldNormal;
}
In;

layout(location = 0) out vec4 Out_Color;

layout(std140, binding = BUFFEER_BINDING_START + 0) buffer Diffuse
{
    vec4 Data[];
}
Buffer_Diffuse;

void main()
{
    vec3 worldNormal = normalize(In.WorldNormal);
    vec3 worldLightDir = normalize(Uni_DirectionalLight.Position);

    vec3 diffuse =
        Uni_DirectionalLight.Color * Buffer_Diffuse.Data[In.InstanceID].rgb * Saturate(dot(worldNormal, worldLightDir));

    Out_Color = vec4(Uni_AmbientLight.Color + diffuse, 1.0);
}