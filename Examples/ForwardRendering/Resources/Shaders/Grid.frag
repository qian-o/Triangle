#version 460 core

layout(location = 0) in VertexData
{
    vec3 NearPos;
    vec3 FarPos;
    mat4 ViewMat;
    mat4 ProjectionMat;
}
In;

layout(location = 0) out vec4 Out_Color;

uniform float Uni_Near;
uniform float Uni_Far;
uniform float Uni_PrimaryScale;
uniform float Uni_SecondaryScale;
uniform float Uni_GridIntensity;
uniform float Uni_Fade;

vec4 grid(vec3 fragPos3D, float scale, float fade)
{
    vec2 coord = fragPos3D.xz * scale;
    vec2 derivative = fwidth(coord);
    vec2 grid = abs(fract(coord - 0.5) - 0.5) / derivative;
    float line = min(grid.x, grid.y);
    return vec4(Uni_GridIntensity, Uni_GridIntensity, Uni_GridIntensity, fade * (1.0 - min(line, 1.0)));
}

float computeDepth(vec3 pos)
{
    vec4 clip_space_pos = In.ProjectionMat * In.ViewMat * vec4(pos, 1.0);

    return (clip_space_pos.z / clip_space_pos.w) * 0.5 + 0.5;
}

float computeLinearDepth(vec3 pos)
{
    vec4 clip_space_pos = In.ProjectionMat * In.ViewMat * vec4(pos, 1.0);
    float clip_space_depth = clip_space_pos.z / clip_space_pos.w;
    float linearDepth = (2.0 * Uni_Near * Uni_Far) / (Uni_Far + Uni_Near - clip_space_depth * (Uni_Far - Uni_Near));

    return linearDepth / Uni_Far;
}

void main()
{
    float ty = -In.NearPos.y / (In.FarPos.y - In.NearPos.y);
    vec3 fragPos3D = In.NearPos + ty * (In.FarPos - In.NearPos);

    gl_FragDepth = computeDepth(fragPos3D);

    float linearDepth = computeLinearDepth(fragPos3D);
    float fading = max(0.0, (0.5 - linearDepth));

    Out_Color = grid(fragPos3D, Uni_PrimaryScale, Uni_Fade) + grid(fragPos3D, Uni_SecondaryScale, 1.0 - Uni_Fade);
    Out_Color.a *= fading;
}