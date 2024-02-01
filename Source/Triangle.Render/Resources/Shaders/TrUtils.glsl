#define UNIFORM_BUFFER_BINDING_START 6
#define UNIFORM_SAMPLER_2D_BINDING_START 4
#define ANTI_ALIASING 4

layout(std140, binding = 0) uniform Transforms
{
    mat4 Model;
    mat4 View;
    mat4 Projection;
    mat4 ObjectToWorld;
    mat4 ObjectToClip;
    mat4 WorldToObject;
}
Uni_Transforms;

layout(std140, binding = 1) uniform Vectors
{
    vec2 Resolution;
    vec3 CameraPosition;
    vec3 CameraUp;
    vec3 CameraRight;
    vec4 Mouse;
    vec4 Date;
}
Uni_Vectors;

layout(std140, binding = 2) uniform Constants
{
    float Time;
    float DeltaTime;
    float FrameRate;
    int Frame;
}
Uni_Constants;

layout(std140, binding = 3) uniform AmbientLight
{
    vec3 Color;
}
Uni_AmbientLight;

layout(std140, binding = 4) uniform DirectionalLight
{
    vec3 Color;
    vec3 Position;
    vec3 Direction;
}
Uni_DirectionalLight;

layout(std140, binding = 5) uniform TexScaleOffset
{
    vec4 Channel0ST;
    vec4 Channel1ST;
    vec4 Channel2ST;
    vec4 Channel3ST;
}
Uni_TexScaleOffset;

layout(binding = 0) uniform sampler2D Channel0;
layout(binding = 1) uniform sampler2D Channel1;
layout(binding = 2) uniform sampler2D Channel2;
layout(binding = 3) uniform sampler2D Channel3;

float ComputeDepth(vec3 pos)
{
    vec4 clip_space_pos = Uni_Transforms.Projection * Uni_Transforms.View * vec4(pos, 1.0);

    return (clip_space_pos.z / clip_space_pos.w) * 0.5 + 0.5;
}

float Saturate(float value)
{
    return clamp(value, 0.0, 1.0);
}

vec4 ObjectToClipPos(vec3 pos)
{
    return Uni_Transforms.ObjectToClip * vec4(pos, 1.0);
}

vec3 ObjectToWorldNormal(vec3 normal)
{
    return normalize((Uni_Transforms.ObjectToWorld * vec4(normal, 0.0)).xyz);
}

vec3 ObjectToWorldDir(vec3 dir)
{
    return normalize((Uni_Transforms.ObjectToWorld * vec4(dir, 0.0)).xyz);
}

vec3 WorldSpaceViewDir(vec3 worldPos)
{
    return Uni_Vectors.CameraPosition - worldPos;
}

vec2 TransformUV(vec2 uv, vec4 st)
{
    return uv * st.xy + st.zw;
}

vec3 UnpackNormal(vec4 packednormal)
{
    return normalize(packednormal.xyz * 2.0 - 1.0);
}