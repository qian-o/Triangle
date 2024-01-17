#define UNIFORM_BUFFER_BINDING_START 4

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
    vec3 CameraPosition;
    vec3 CameraUp;
    vec3 CameraRight;
}
Uni_Vectors;

layout(std140, binding = 2) uniform AmbientLight
{
    vec3 Color;
}
Uni_AmbientLight;

layout(std140, binding = 3) uniform DirectionalLight
{
    vec3 Color;
    vec3 Position;
    vec3 Direction;
}
Uni_DirectionalLight;

float ComputeDepth(vec3 pos)
{
    vec4 clip_space_pos = Uni_Transforms.Projection * Uni_Transforms.View * vec4(pos, 1.0);

    return (clip_space_pos.z / clip_space_pos.w) * 0.5 + 0.5;
}

float Saturate(float value)
{
    return clamp(value, 0.0, 1.0);
}

vec3 WorldSpaceViewDirection(vec3 worldPos)
{
    return Uni_Vectors.CameraPosition - worldPos;
}