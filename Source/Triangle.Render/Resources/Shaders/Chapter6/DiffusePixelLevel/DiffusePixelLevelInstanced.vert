#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtils.glsl"
#include "../../TrVertex.glsl"

layout(location = 0) out VertexData
{
    flat int InstanceID;
    vec3 WorldNormal;
}
Out;

void main()
{
    SampleTransforms transforms = GetSampleTransforms(INSTANCE_ID);

    Out.InstanceID = INSTANCE_ID;
    Out.WorldNormal = normalize((In_Normal * mat3(transforms.WorldToObject)));

    gl_Position = transforms.ObjectToClip * vec4(In_Position, 1.0);
}