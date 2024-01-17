#version 460

#include "TrShaderUtilities.glsl"
#include "TrVertex.glsl"

layout(location = 0) out VertexData
{
    vec4 Color;
}
Out;

void main()
{
    Out.Color = vec4(In_Normal * 0.5 + vec3(0.5), 1.0);

    gl_Position = Uni_Transforms.ObjectToClip * vec4(In_Position, 1.0);
}