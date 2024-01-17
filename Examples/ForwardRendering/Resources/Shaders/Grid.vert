#version 460

#include "TrShaderUtilities.glsl"
#include "TrVertex.glsl"

layout(location = 0) out VertexData
{
    vec3 NearPos;
    vec3 FarPos;
}
Out;

vec3 UnprojectPoint(float x, float y, float z, mat4 viewInvMat, mat4 projInvMat)
{
    vec4 point = viewInvMat * projInvMat * vec4(x, y, z, 1.0);

    return point.xyz / point.w;
}

void main()
{
    mat4 viewInvMat = inverse(Uni_Transforms.View);
    mat4 projInvMat = inverse(Uni_Transforms.Projection);

    Out.NearPos = UnprojectPoint(In_Position.x, In_Position.y, -1.0, viewInvMat, projInvMat).xyz;
    Out.FarPos = UnprojectPoint(In_Position.x, In_Position.y, 1.0, viewInvMat, projInvMat).xyz;

    gl_Position = vec4(In_Position, 1.0);
}