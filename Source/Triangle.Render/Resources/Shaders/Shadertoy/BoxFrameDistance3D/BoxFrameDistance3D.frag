// Original author: https://www.shadertoy.com/user/iq
// Project link: https://www.shadertoy.com/view/3ljcRh

#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtils.glsl"

layout(location = 0) out vec4 Out_Color;

float sdBoxFrame(vec3 p, vec3 b, float e)
{
    p = abs(p) - b;
    vec3 q = abs(p + e) - e;

    return min(min(length(max(vec3(p.x, q.y, q.z), 0.0)) + min(max(p.x, max(q.y, q.z)), 0.0),
                   length(max(vec3(q.x, p.y, q.z), 0.0)) + min(max(q.x, max(p.y, q.z)), 0.0)),
               length(max(vec3(q.x, q.y, p.z), 0.0)) + min(max(q.x, max(q.y, p.z)), 0.0));
}

float map(vec3 pos)
{
    return sdBoxFrame(pos, vec3(0.5, 0.3, 0.5), 0.025);
}

vec3 calcNormal(in vec3 pos)
{
    const float eps = 0.0005;
    vec2 e = vec2(1.0, -1.0) * 0.5773;

    return normalize(e.xyy * map(pos + e.xyy * eps) + e.yyx * map(pos + e.yyx * eps) + e.yxy * map(pos + e.yxy * eps) +
                     e.xxx * map(pos + e.xxx * eps));
}

void main()
{
    float an = (Uni_Constants.Time - 10.0) * 0.5;
    vec3 ro = vec3(1.0 * cos(an), 0.0, 1.0 * sin(an)) * 1.2;
    vec3 ta = vec3(0.0, -0.0, 0.0);

    vec3 ww = normalize(ta - ro);
    vec3 uu = normalize(cross(ww, vec3(0.0, 1.0, 0.0)));
    vec3 vv = normalize(cross(uu, ww));

    vec3 tot = vec3(0.0);
    for (int m = 0; m < ANTI_ALIASING; m++)
    {
        for (int n = 0; n < ANTI_ALIASING; n++)
        {
            vec2 o = vec2(float(m), float(n)) / float(ANTI_ALIASING) - 0.5;
            vec2 p = (2.0 * (gl_FragCoord.xy + o) - Uni_Vectors.Resolution.xy) / Uni_Vectors.Resolution.y;

            vec3 rd = normalize(p.x * uu + p.y * vv + 1.5 * ww);

            const float tmax = 5.0;
            float t = 0.0;
            for (int i = 0; i < 256; i++)
            {
                vec3 pos = ro + t * rd;
                float h = map(pos);
                if (h < 0.0001 || t > tmax)
                {
                    break;
                }

                t += h;
            }

            vec3 col = vec3(0.0);
            if (t < tmax)
            {
                vec3 pos = ro + t * rd;
                vec3 nor = calcNormal(pos) * ro;
                float dif = clamp(dot(nor, vec3(0.57703)), 0.0, 1.0);
                float amb = 0.5 + 0.5 * dot(nor, vec3(0.0, 1.0, 0.0));
                col = vec3(0.2, 0.3, 0.4) * amb + vec3(0.85, 0.75, 0.65) * dif;
            }

            col = sqrt(col);
            tot += col;
        }
    }

    tot /= float(ANTI_ALIASING * ANTI_ALIASING);

    Out_Color = vec4(tot, 1.0);
}