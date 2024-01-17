#version 460

layout(location = 0) in vec3 In_Position;

layout(location = 0) out VertexData
{
    vec3 NearPos;
    vec3 FarPos;
}
Out;

layout(std140, binding = 0) uniform Matrices
{
    mat4 View;
    mat4 Projection;
}
Uni_Matrices;

vec3 UnprojectPoint(float x, float y, float z, mat4 viewInvMat, mat4 projInvMat)
{
    vec4 point = viewInvMat * projInvMat * vec4(x, y, z, 1.0);

    return point.xyz / point.w;
}

void main()
{
    mat4 viewInvMat = inverse(Uni_Matrices.View);
    mat4 projInvMat = inverse(Uni_Matrices.Projection);

    Out.NearPos = UnprojectPoint(In_Position.x, In_Position.y, -1.0, viewInvMat, projInvMat).xyz;
    Out.FarPos = UnprojectPoint(In_Position.x, In_Position.y, 1.0, viewInvMat, projInvMat).xyz;

    gl_Position = vec4(In_Position, 1.0);
}