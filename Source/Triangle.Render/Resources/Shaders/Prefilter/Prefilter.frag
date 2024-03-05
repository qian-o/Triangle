#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtils.glsl"

layout(location = 0) in VertexData
{
    vec3 WorldPos;
}
In;

layout(location = 0) out vec4 Out_Color;

layout(std140, binding = UNIFORM_BUFFER_BINDING_START + 0) uniform Parameters
{
    mat4 View;
    mat4 Projection;
    float Roughness;
}
Uni_Parameters;

const uint SAMPLE_COUNT = 1024;

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

float RadicalInverse_VdC(uint bits)
{
    bits = (bits << 16u) | (bits >> 16u);
    bits = ((bits & 0x55555555u) << 1u) | ((bits & 0xAAAAAAAAu) >> 1u);
    bits = ((bits & 0x33333333u) << 2u) | ((bits & 0xCCCCCCCCu) >> 2u);
    bits = ((bits & 0x0F0F0F0Fu) << 4u) | ((bits & 0xF0F0F0F0u) >> 4u);
    bits = ((bits & 0x00FF00FFu) << 8u) | ((bits & 0xFF00FF00u) >> 8u);

    return float(bits) * 2.3283064365386963e-10; // / 0x100000000
}

vec2 Hammersley(uint i, uint N)
{
    return vec2(float(i) / float(N), RadicalInverse_VdC(i));
}

vec3 ImportanceSampleGGX(vec2 Xi, vec3 N, float roughness)
{
    float a = roughness * roughness;

    float phi = 2.0 * PI * Xi.x;
    float cosTheta = sqrt((1.0 - Xi.y) / (1.0 + (a * a - 1.0) * Xi.y));
    float sinTheta = sqrt(1.0 - cosTheta * cosTheta);

    vec3 H;
    H.x = cos(phi) * sinTheta;
    H.y = sin(phi) * sinTheta;
    H.z = cosTheta;

    vec3 up = abs(N.z) < 0.999 ? vec3(0.0, 0.0, 1.0) : vec3(1.0, 0.0, 0.0);
    vec3 tangentX = normalize(cross(up, N));
    vec3 tangentY = cross(N, tangentX);

    return normalize(tangentX * H.x + tangentY * H.y + N * H.z);
}

void main()
{
    vec3 N = normalize(In.WorldPos);

    // make the simplyfied assumption that V equals R equals the normal
    vec3 R = N;
    vec3 V = R;

    vec3 prefilteredColor = vec3(0.0);
    float totalWeight = 0.0;

    for (uint i = 0; i < SAMPLE_COUNT; i++)
    {
        // generate a sample vector that's biased towards the normal direction (importance sampling).
        vec2 Xi = Hammersley(i, SAMPLE_COUNT);
        vec3 H = ImportanceSampleGGX(Xi, N, Uni_Parameters.Roughness);
        vec3 L = normalize(2.0 * dot(V, H) * H - V);

        float NdotL = max(dot(N, L), 0.0);
        if (NdotL > 0.0)
        {
            // sample from the environment's mip level based on roughness/pdf
            float D = DistributionGGX(N, H, Uni_Parameters.Roughness);
            float NdotH = max(dot(N, H), 0.0);
            float HdotV = max(dot(H, V), 0.0);
            float pdf = D * NdotH / (4.0 * HdotV) + 0.0001;

            float resolution = 512.0; // resolution of source cubemap (per face)
            float saTexel = 4.0 * PI / (6.0 * resolution * resolution);
            float saSample = 1.0 / (float(SAMPLE_COUNT) * pdf + 0.0001);

            float mipLevel = Uni_Parameters.Roughness == 0.0 ? 0.0 : 0.5 * log2(saSample / saTexel);

            prefilteredColor += SampleTexture(Map0, L, mipLevel).rgb * NdotL;
            totalWeight += NdotL;
        }
    }

    prefilteredColor = prefilteredColor / totalWeight;

    Out_Color = vec4(prefilteredColor, 1.0);
}