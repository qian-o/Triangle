#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtilInstanced.glsl"

layout(location = 0) in VertexData
{
    flat int InstanceID;
}
In;

layout(location = 0) out vec4 Out_Color;

layout(std140, binding = BUFFEER_BINDING_START + 0) buffer Color
{
    vec4 Data[];
}
Buffer_Color;

void main()
{
    Out_Color = Buffer_Color.Data[In.InstanceID];
}