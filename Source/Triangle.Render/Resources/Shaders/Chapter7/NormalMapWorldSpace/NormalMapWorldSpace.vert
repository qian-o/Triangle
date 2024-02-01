#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtils.glsl"
#include "../../TrVertex.glsl"

layout(location = 0) out VertexData
{
    vec4 UV;
    vec4 TtoW0;
    vec4 TtoW1;
    vec4 TtoW2;
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

    vec3 worldPos = (Uni_Transforms.ObjectToWorld * vec4(In_Position, 1.0)).xyz;
    vec3 worldNormal = ObjectToWorldNormal(In_Normal);
    vec3 worldTangent = ObjectToWorldDir(In_Tangent);
    vec3 worldBinormal = cross(worldNormal, worldTangent);

    Out.TtoW0 = vec4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
    Out.TtoW1 = vec4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
    Out.TtoW2 = vec4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

    gl_Position = ObjectToClipPos(In_Position);
}