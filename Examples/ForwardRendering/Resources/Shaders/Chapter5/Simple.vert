#version 320 es

in vec3 In_Position;
in vec3 In_Normal;

out VertexData
{
    vec4 Color;
}
Out;

uniform mat4 Uni_Model;
uniform mat4 Uni_View;
uniform mat4 Uni_Projection;

void main()
{
    Out.Color = vec4(In_Normal * 0.5 + vec3(0.5), 1.0);

    gl_Position = Uni_Projection * Uni_View * Uni_Model * vec4(In_Position, 1.0);
}