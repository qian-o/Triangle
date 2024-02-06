#version 460

#extension GL_GOOGLE_include_directive : require

#include "../TrUtils.glsl"

layout(location = 0) in VertexData
{
    vec2 UV[9];
}
In;

layout(location = 0) out vec4 Out_Color;

layout(std140, binding = UNIFORM_BUFFER_BINDING_START + 0) uniform Parameters
{
    vec4 EdgeColor;
}
Uni_Parameters;

float luminance(vec4 color)
{
    return dot(color.rgb, vec3(0.299, 0.587, 0.114));
}

float sobel()
{
    const float Gx[9] = {-1, -2, -1, 0, 0, 0, 1, 2, 1};
    const float Gy[9] = {-1, 0, 1, -2, 0, 2, -1, 0, 1};

    float texColor;
    float edgeX = 0.0;
    float edgeY = 0.0;
    for (int j = 0; j < 9; j++)
    {
        texColor = luminance(SampleTexture(Channel0, In.UV[j]));
        edgeX += texColor * Gx[j];
        edgeY += texColor * Gy[j];
    }

    return 1 - abs(edgeX) - abs(edgeY);
}

void main()
{
    float edge = sobel();

    Out_Color = mix(Uni_Parameters.EdgeColor, vec4(0.0), edge);
}