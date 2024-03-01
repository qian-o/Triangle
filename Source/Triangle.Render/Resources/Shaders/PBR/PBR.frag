#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtils.glsl"

layout(location = 0) in VertexData
{
    vec3 WorldNormal;
    vec3 WorldPos;
    vec2 UV;
}
In;

layout(location = 0) out vec4 Out_Color;

vec3 GetNormalFromMap()
{
    vec3 normal = UnpackNormal(SampleTexture(Channel1, In.UV));

    vec3 Q1 = dFdx(In.WorldPos);
    vec3 Q2 = dFdy(In.WorldPos);
    vec2 st1 = dFdx(In.UV);
    vec2 st2 = dFdy(In.UV);

    vec3 N = normalize(In.WorldNormal);
    vec3 T = normalize(Q1 * st2.t - Q2 * st1.t);
    vec3 B = -normalize(cross(N, T));
    mat3 TBN = mat3(T, B, N);

    return normalize(TBN * normal);
}

float DistributionGGX(vec3 N, vec3 H, float roughness)
{
    float a = roughness * roughness;
    float a2 = a * a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;

    float num = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return num / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r * r) / 8.0;

    float num = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return num / denom;
}

float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness);

    return ggx1 * ggx2;
}

vec3 FresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}

void main()
{
    vec3 albedo = pow(SampleTexture(Channel0, In.UV).rgb, vec3(2.2));
    vec3 normal = GetNormalFromMap();
    float metallic = SampleTexture(Channel2, In.UV).r;
    float roughness = SampleTexture(Channel3, In.UV).r;
    float ao = SampleTexture(Channel4, In.UV).r;

    vec3 N = normalize(In.WorldNormal);
    vec3 V = normalize(WorldSpaceViewDir(In.WorldPos));

    vec3 F0 = vec3(0.04);
    F0 = mix(F0, albedo, metallic);

    // reflectance equation
    vec3 Lo = vec3(0.0);
    for (int i = 0; i < Uni_PointLights.Count; i++)
    {
        // reflectance equation
        vec3 L = normalize(Uni_PointLights.Lights[i].Position - In.WorldPos);
        vec3 H = normalize(V + L);
        float distance = length(Uni_PointLights.Lights[i].Position - In.WorldPos);
        float attenuation = 1.0 / (distance * distance);
        vec3 radiance = Uni_PointLights.Lights[i].Color * attenuation;

        // Cook-Torrance BRDF
        float NDF = DistributionGGX(N, H, roughness);
        float G = GeometrySmith(N, V, L, roughness);
        vec3 F = FresnelSchlick(max(dot(H, V), 0.0), F0);

        vec3 kS = F;
        vec3 kD = vec3(1.0) - kS;
        kD *= 1.0 - metallic;

        vec3 numerator = NDF * G * F;
        float denominator = 4 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0);
        vec3 specular = numerator / max(denominator, 0.001);

        // add to outgoing radiance Lo
        Lo += (kD * albedo / PI + specular) * radiance * max(dot(N, L), 0.0);
    }

    vec3 ambient = vec3(0.03) * albedo * ao;
    vec3 color = ambient + Lo;

    // HDR tonemapping
    color = color / (color + vec3(1.0));
    color = pow(color, vec3(1.0 / 2.2));

    Out_Color = vec4(color, 1.0);
}