#version 320 es

in vec3 In_Position;
in vec3 In_Normal;
in vec2 In_TexCoord;

out vec3 Out_Position;
out vec3 Out_Normal;
out vec2 Out_UV;

uniform mat4 Uni_Model;
uniform mat4 Uni_View;
uniform mat4 Uni_Projection;

void main()
{
    Out_Position = vec3(Uni_Model * vec4(In_Position, 1.0));
    Out_Normal = mat3(Uni_View * Uni_Model) * In_Normal;
    Out_UV = In_TexCoord;

    gl_Position = Uni_Projection * Uni_View * Uni_Model * vec4(Out_Position, 1.0);
}