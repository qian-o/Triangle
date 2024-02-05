#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtils.glsl"
#include "../../TrVertex.glsl"

layout(location = 0) out VertexData
{
    vec4 Color;
}
Out;

layout(std140, binding = UNIFORM_BUFFER_BINDING_START + 0) uniform Material
{
    vec4 Diffuse;
}
Uni_Material;

void main()
{
    vec3 worldNormal = ObjectToWorldNormal(In_Normal);
    vec3 worldLightDir = normalize(Uni_DirectionalLight.Position);

    vec3 diffuse = Uni_DirectionalLight.Color * Uni_Material.Diffuse.rgb * Saturate(dot(worldNormal, worldLightDir));

    Out.Color = vec4(Uni_AmbientLight.Color + diffuse, 1.0);

    gl_Position = Uni_Transforms.ObjectToClip * vec4(In_Position, 1.0);
}