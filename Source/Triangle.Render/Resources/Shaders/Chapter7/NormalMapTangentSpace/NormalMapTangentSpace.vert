#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtils.glsl"
#include "../../TrVertex.glsl"

layout(location = 0) out VertexData
{
    vec4 UV;
    vec3 LightDir;
    vec3 ViewDir;
}
Out;

layout(std140, binding = UNIFORM_BUFFER_BINDING_START + 0) uniform Material
{
    vec4 Color;
    float BumpScale;
    vec4 Specular;
    float Gloss;
}
Uni_Material;

void main()
{
    Out.UV.xy = TransformUV(In_TexCoord, Uni_TexScaleOffset.Channel0ST);
    Out.UV.zw = TransformUV(In_TexCoord, Uni_TexScaleOffset.Channel1ST);

    TANGENT_SPACE_ROTATION;

    Out.LightDir = rotation * ObjSpaceDirectionalLightDir();
    Out.ViewDir = rotation * ObjSpaceViewDir(In_Position);

    gl_Position = ObjectToClipPos(In_Position);
}