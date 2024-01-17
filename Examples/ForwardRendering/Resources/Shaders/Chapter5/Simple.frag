#version 460

layout(location = 0) in VertexData
{
    vec4 Color;
}
In;

layout(location = 0) out vec4 Out_Color;

layout(std140, binding = 1) uniform Parameters
{
    vec4 Color;
}
Uni_Parameters;

void main()
{
    Out_Color = In.Color * Uni_Parameters.Color;
}