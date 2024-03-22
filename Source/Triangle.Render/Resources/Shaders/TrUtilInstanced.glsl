#extension GL_GOOGLE_include_directive : require

#include "TrUtils.glsl"

#define BUFFEER_BINDING_START 1

struct SampleTransforms
{
    mat4 Model;
    mat4 ObjectToWorld;
    mat4 ObjectToClip;
    mat4 WorldToObject;
};

layout(std140, binding = 0) buffer Matrix
{
    mat4 Data[];
}
Buffer_Matrix;

SampleTransforms GetSampleTransforms(int index)
{
    SampleTransforms transforms;
    transforms.Model = Buffer_Matrix.Data[index];
    transforms.ObjectToWorld = transforms.Model;
    transforms.ObjectToClip = Uni_Transforms.Projection * Uni_Transforms.View * transforms.Model;
    transforms.WorldToObject = inverse(transforms.ObjectToWorld);

    return transforms;
}