#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtils.glsl"
#include "../TrVertex.glsl"

layout(location = 0) out VertexData
{
    vec2 UV[9];
}
Out;

void main()
{
    vec2 uv = In_TexCoord;
    vec2 texelSize = Uni_TexParams.Channel0Size.xy;

    Out.UV[0] = uv + texelSize * vec2(-1.0, -1.0);
    Out.UV[1] = uv + texelSize * vec2(0.0, -1.0);
    Out.UV[2] = uv + texelSize * vec2(1.0, -1.0);
    Out.UV[3] = uv + texelSize * vec2(-1.0, 0.0);
    Out.UV[4] = uv + texelSize * vec2(0.0, 0.0);
    Out.UV[5] = uv + texelSize * vec2(1.0, 0.0);
    Out.UV[6] = uv + texelSize * vec2(-1.0, 1.0);
    Out.UV[7] = uv + texelSize * vec2(0.0, 1.0);
    Out.UV[8] = uv + texelSize * vec2(1.0, 1.0);

    gl_Position = vec4(In_Position, 1.0);
}