#version 460

layout(location = 0) out vec4 Out_Color;

layout(std140, binding = 1) uniform Parameters
{
    vec4 Color;
    float Intensity;
    float Range;
}
Uni_Parameters;

void main()
{
    Out_Color = Uni_Parameters.Color;
}