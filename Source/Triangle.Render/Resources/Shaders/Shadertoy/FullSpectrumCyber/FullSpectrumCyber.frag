// Original author: https://www.shadertoy.com/user/Kali
// Project link: https://www.shadertoy.com/view/XcXXzS

#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtils.glsl"

layout(location = 0) out vec4 Out_Color;

int hexid;
vec3 hpos, point, pt;
float tcol, bcol, hitbol, hexpos, fparam = 0.0;

mat2 rot(float a)
{
    float s = sin(a), c = cos(a);

    return mat2(c, s, -s, c);
}

vec3 path(float t)
{
    return vec3(sin(t * 0.3 + cos(t * 0.2) * 0.5) * 4.0, cos(t * 0.2) * 3.0, t);
}

float hexagon(vec2 p, float r)
{
    const vec3 k = vec3(-0.866025404, 0.5, 0.577350269);

    p = abs(p);
    p -= 2.0 * min(dot(k.xy, p), 0.0) * k.xy;
    p -= vec2(clamp(p.x, -k.z * r, k.z * r), r);

    return length(p) * sign(p.y);
}

float hex(vec2 p)
{
    p.x *= 0.57735 * 2.0;
    p.y += mod(floor(p.x), 2.0) * 0.5;
    p = abs((mod(p, 1.0) - 0.5));

    return abs(max(p.x * 1.5 + p.y, p.y * 2.0) - 1.0);
}

mat3 lookat(vec3 dir)
{
    vec3 up = vec3(0.0, 1.0, 0.0);
    vec3 rt = normalize(cross(dir, up));

    return mat3(rt, cross(rt, dir), dir);
}

float hash12(vec2 p)
{
    p *= 1000.0;
    vec3 p3 = fract(vec3(p.xyx) * 0.1031);
    p3 += dot(p3, p3.yzx + 33.33);

    return fract((p3.x + p3.y) * p3.z);
}

float de(vec3 p)
{
    pt = vec3(p.xy - path(p.z).xy, p.z);

    float h = abs(hexagon(pt.xy, 3.0 + fparam));
    hexpos = hex(pt.yz);
    tcol = smoothstep(0.0, 0.15, hexpos);
    h -= tcol * 0.1;

    vec3 pp = p - hpos;
    pp = lookat(point) * pp;
    pp.y -= abs(sin(Uni_Constants.Time)) * 3.0 + (fparam - (2.0 - fparam));
    pp.yz *= rot(-Uni_Constants.Time);

    float bola = length(pp) - 1.0;
    bcol = smoothstep(0.0, 0.5, hex(pp.xy * 3.0));
    bola -= bcol * 0.1;

    vec3 pr = p;
    pr.z = mod(p.z, 6.0) - 3.0;

    float d = min(h, bola);
    if (d == bola)
    {
        tcol = 1.0;
        hitbol = 1.0;
    }
    else
    {
        hitbol = 0.0;
        bcol = 1.0;
    }

    return d * 0.5;
}

vec3 normal(vec3 p)
{
    vec2 e = vec2(0.0, 0.005);

    return normalize(vec3(de(p + e.yxx), de(p + e.xyx), de(p + e.xxy)) - de(p));
}

vec3 march(vec3 from, vec3 dir)
{
    vec3 odir = dir;
    vec3 p = from, col = vec3(0.0);
    float d, td = 0.0;
    vec3 g = vec3(0.0);

    for (int i = 0; i < 200; i++)
    {
        d = de(p);

        if (d < 0.001 || td > 200.0)
        {
            break;
        }

        p += dir * d;
        td += d;
        g += 0.1 / (0.1 + d) * hitbol * abs(normalize(point));
    }

    float hp = hexpos * (1.0 - hitbol);
    p -= dir * 0.01;
    vec3 n = normal(p);
    if (d < 0.001)
    {
        col = pow(max(0.0, dot(-dir, n)), 2.0) * vec3(0.6, 0.7, 0.8) * tcol * bcol;
    }
    col += float(hexid);

    vec3 pr = pt;
    dir = reflect(dir, n);
    td = 0.0;
    for (int i = 0; i < 200; i++)
    {
        d = de(p);

        if (d < 0.001 || td > 200.0)
        {
            break;
        }

        p += dir * d;
        td += d;
        g += 0.1 / (0.1 + d) * abs(normalize(point));
    }

    float zz = p.z;
    if (d < 0.001)
    {
        vec3 refcol = pow(max(0.0, dot(-odir, n)), 2.0) * vec3(0.6, 0.7, 0.8) * tcol * bcol;
        p = pr;
        p = abs(0.5 - fract(p * 0.1));

        float m = 100.0;
        for (int i = 0; i < 10; i++)
        {
            p = abs(p) / dot(p, p) - 0.8;
            m = min(m, length(p));
        }
        col = mix(col, refcol, m) - m * 0.3;
        col += step(0.3, hp) * step(0.9, fract(pr.z * 0.05 + Uni_Constants.Time * 0.5 + hp * 0.1)) * 0.7;
        col += step(0.3, hexpos) * step(0.9, fract(zz * 0.05 + Uni_Constants.Time + hexpos * 0.1)) * 0.3;
    }
    col += g * 0.03;
    col.rb *= rot(odir.y * 0.5);

    return col;
}

void main()
{
    vec2 uv = gl_FragCoord.xy / Uni_Vectors.Resolution - 0.5;
    uv.x *= Uni_Vectors.Resolution.x / Uni_Vectors.Resolution.y;

    float t = Uni_Constants.Time * 2.0;
    vec3 from = path(t);

    if (mod(Uni_Constants.Time - 10.0, 20.0) > 10.0)
    {
        from = path(floor(t / 20.0) * 20.0 + 10.0);
        from.x += 2.0;
    }

    hpos = path(t + 3.0);

    vec3 adv = path(t + 2.0);
    vec3 dir = normalize(vec3(uv, 0.7));
    vec3 dd = normalize(adv - from);

    point = normalize(adv - hpos);
    point.xz *= rot(sin(Uni_Constants.Time) * 0.2);

    dir = lookat(dd) * dir;

    vec3 col = march(from, dir);
    col *= vec3(1.0, 0.9, 0.8);

    Out_Color = vec4(col, 1.0);
}