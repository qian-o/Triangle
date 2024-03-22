#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtils.glsl"

layout(location = 0) in VertexData
{
    vec3 WorldPos;
    vec3 WorldNormal;
    vec4 LightSpacePos;
}
In;

layout(location = 0) out vec4 Out_Color;

float ShadowCalculation()
{
    vec3 projCoords = In.LightSpacePos.xyz / In.LightSpacePos.w;
    projCoords = projCoords * 0.5 + 0.5;

    float closestDepth = SampleTexture(Channel0, projCoords.xy).x;
    float currentDepth = projCoords.z;

    vec3 normal = normalize(In.WorldNormal);
    vec3 lightDir = normalize(Uni_DirectionalLight.Position);
    float cosTheta = clamp(dot(normal, lightDir), 0.0, 1.0);
    float bias = 0.00001 * tan(acos(cosTheta));
    bias = clamp(bias, 0.0, 0.01);
    float shadow = currentDepth - bias > closestDepth ? 0.5 : 0.0;

    return shadow;
}

void main()
{
    float shadow = ShadowCalculation();

    Out_Color = vec4(0.0, 0.0, 0.0, shadow);
}