#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtilInstanced.glsl"
#include "../TrVertex.glsl"

layout(location = 0) out VertexData
{
    flat int InstanceID;
}
Out;

void main()
{
    SampleTransforms transforms = GetSampleTransforms(INSTANCE_ID);

    Out.InstanceID = INSTANCE_ID;

    gl_Position = transforms.ObjectToClip * vec4(In_Position, 1.0);
}