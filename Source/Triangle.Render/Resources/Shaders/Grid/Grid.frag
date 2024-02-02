#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtils.glsl"

layout(location = 0) in VertexData
{
    vec3 NearPos;
    vec3 FarPos;
}
In;

layout(location = 0) out vec4 Out_Color;

layout(std140, binding = UNIFORM_BUFFER_BINDING_START + 0) uniform Parameters
{
    float Near;
    float Far;
    float PrimaryScale;
    float SecondaryScale;
    float GridIntensity;
    float AxisIntensity;
    float Fade;
}
Uni_Parameters;

vec4 grid(vec3 fragPos3D, float scale, float fade)
{
    vec2 coord = fragPos3D.xz * scale;
    vec2 derivative = fwidth(coord);
    vec2 grid = abs(fract(coord - 0.5) - 0.5) / derivative;
    float line = min(grid.x, grid.y);

    vec4 color = vec4(Uni_Parameters.GridIntensity, Uni_Parameters.GridIntensity, Uni_Parameters.GridIntensity,
                      fade * (1.0 - min(line, 1.0)));

    float minimumz = min(derivative.y, 1.0);
    float minimumx = min(derivative.x, 1.0);

    // z - axis
    if (fragPos3D.x > -Uni_Parameters.AxisIntensity * minimumx && fragPos3D.x < Uni_Parameters.AxisIntensity * minimumx)
    {
        color.b = 1.0;
    }

    // x - axis
    if (fragPos3D.z > -Uni_Parameters.AxisIntensity * minimumz && fragPos3D.z < Uni_Parameters.AxisIntensity * minimumz)
    {
        color.r = 1.0;
    }

    return color;
}

float computeLinearDepth(vec3 pos)
{
    vec4 clip_space_pos = Uni_Transforms.Projection * Uni_Transforms.View * vec4(pos, 1.0);
    float clip_space_depth = clip_space_pos.z / clip_space_pos.w;
    float linearDepth =
        (2.0 * Uni_Parameters.Near * Uni_Parameters.Far) /
        (Uni_Parameters.Far + Uni_Parameters.Near - clip_space_depth * (Uni_Parameters.Far - Uni_Parameters.Near));

    return linearDepth / Uni_Parameters.Far;
}

void main()
{
    float ty = -In.NearPos.y / (In.FarPos.y - In.NearPos.y);
    vec3 fragPos3D = In.NearPos + ty * (In.FarPos - In.NearPos);

    gl_FragDepth = ComputeDepth(fragPos3D);

    float linearDepth = computeLinearDepth(fragPos3D);
    float fading = max(0.0, (0.5 - linearDepth));

    Out_Color = grid(fragPos3D, Uni_Parameters.PrimaryScale, Uni_Parameters.Fade) +
                grid(fragPos3D, Uni_Parameters.SecondaryScale, 1.0 - Uni_Parameters.Fade);
    Out_Color.a *= fading;
}