#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtils.glsl"
#include "../TrVertex.glsl"

layout(location = 0) out VertexData
{
    vec2 UV;
}
Out;

void main()
{
    Out.UV = In_TexCoord;

    mat4 model = mat4(mat3(Uni_Transforms.Model));
    mat4 view = mat4(mat3(Uni_Transforms.View));
    vec4 position = Uni_Transforms.Projection * view * model * vec4(In_Position, 1.0);

    gl_Position = position.xyww;
}