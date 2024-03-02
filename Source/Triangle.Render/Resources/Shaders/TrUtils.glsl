#define PI 3.14159265359
#define WO_PI 6.28318530718
#define FOUR_PI 12.56637061436
#define INV_PI 0.31830988618
#define INV_TWO_PI 0.15915494309
#define INV_FOUR_PI 0.07957747155
#define HALF_PI 1.57079632679
#define INV_HALF_PI 0.636619772367

#define UNIFORM_BUFFER_BINDING_START 8
#define UNIFORM_SAMPLER_2D_BINDING_START 5
#define MAX_POINT_LIGHTS 50
#define ANTI_ALIASING 4

struct PointLight
{
    vec3 Color;
    vec3 Position;
    float Intensity;
    float Range;
};

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
    int FrameCount;
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
}
Uni_DirectionalLight;

layout(std140, binding = 5) uniform PointLights
{
    int Count;
    PointLight Lights[MAX_POINT_LIGHTS];
}
Uni_PointLights;

layout(std140, binding = 6) uniform TexParams
{
    vec4 Channel0Size;
    vec4 Channel1Size;
    vec4 Channel2Size;
    vec4 Channel3Size;
    vec4 Channel4Size;
}
Uni_TexParams;

layout(std140, binding = 7) uniform TexScaleOffset
{
    vec4 Channel0ST;
    vec4 Channel1ST;
    vec4 Channel2ST;
    vec4 Channel3ST;
    vec4 Channel4ST;
}
Uni_TexScaleOffset;

layout(binding = 0) uniform sampler2D Channel0;
layout(binding = 1) uniform sampler2D Channel1;
layout(binding = 2) uniform sampler2D Channel2;
layout(binding = 3) uniform sampler2D Channel3;
layout(binding = 4) uniform sampler2D Channel4;

/// <summary>
/// computes depth from position
/// </summary>
float ComputeDepth(vec3 pos)
{
    vec4 clip_space_pos = Uni_Transforms.Projection * Uni_Transforms.View * vec4(pos, 1.0);

    return (clip_space_pos.z / clip_space_pos.w) * 0.5 + 0.5;
}

/// <summary>
/// 0 - 1 float
/// </summary>
float Saturate(float value)
{
    return clamp(value, 0.0, 1.0);
}

/// <summary>
/// 0 - 1 vec2
/// </summary>
vec2 Saturate(vec2 value)
{
    return clamp(value, 0.0, 1.0);
}

/// <summary>
/// 0 - 1 vec3
/// </summary>
vec3 Saturate(vec3 value)
{
    return clamp(value, 0.0, 1.0);
}

/// <summary>
/// 0 - 1 vec4
/// </summary>
vec4 Saturate(vec4 value)
{
    return clamp(value, 0.0, 1.0);
}

/// <summary>
/// Returns the position of the vertex in clip space.
/// </summary>
vec4 ObjectToClipPos(vec3 pos)
{
    return Uni_Transforms.ObjectToClip * vec4(pos, 1.0);
}

/// <summary>
/// Transforms normal from object space to world space.
/// </summary>
vec3 ObjectToWorldNormal(vec3 normal)
{
    return normalize((normal * mat3(Uni_Transforms.WorldToObject)));
}

/// <summary>
/// Transforms direction from object space to world space.
/// </summary>
vec3 ObjectToWorldDir(vec3 dir)
{
    return normalize((Uni_Transforms.ObjectToWorld * vec4(dir, 0.0)).xyz);
}

/// <summary>
/// Returns the direction of the directional light in object space.
/// </summary>
vec3 ObjSpaceDirectionalLightDir()
{
    return (Uni_Transforms.WorldToObject * vec4(Uni_DirectionalLight.Position, 0.0)).xyz;
}

/// <summary>
/// Returns the direction of the view in object space.
/// </summary>
vec3 ObjSpaceViewDir(vec3 objPos)
{
    vec3 objSpaceCameraPos = (Uni_Transforms.WorldToObject * vec4(Uni_Vectors.CameraPosition, 1.0)).xyz;

    return objSpaceCameraPos - objPos;
}

/// <summary>
/// Returns the direction of the directional light in world space.
/// </summary>
vec3 WorldSpaceDirectionalLightDir()
{
    return Uni_DirectionalLight.Position;
}

/// <summary>
/// Returns the direction of the view in world space.
/// </summary>
vec3 WorldSpaceViewDir(vec3 worldPos)
{
    return Uni_Vectors.CameraPosition - worldPos;
}

/// <summary>
/// Transforms the UV coordinates using the scale and offset.
/// </summary>
vec2 TransformUV(vec2 uv, vec4 st)
{
    return uv * st.xy + st.zw;
}

/// <summary>
/// Unpacks a normal from a packed normal.
/// </summary>
vec3 UnpackNormal(vec4 packednormal)
{
    return normalize(packednormal.xyz * 2.0 - 1.0);
}

vec4 SampleTexture(sampler2D tex, vec2 uv)
{
    return texture(tex, uv);
}

vec3 GammaCorrection(vec3 color)
{
    return pow(color, vec3(1.0 / 2.2));
}