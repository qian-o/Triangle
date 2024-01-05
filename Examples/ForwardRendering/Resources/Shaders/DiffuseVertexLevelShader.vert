#version 320 es

in vec3 In_Position;
in vec3 In_Normal;
in vec2 In_TexCoord;

out VertexData
{
    vec4 Color;
}
Out;

uniform mat4 Uni_Model;
uniform mat4 Uni_View;
uniform mat4 Uni_Projection;
uniform AmbientLight
{
    vec3 Color;
}
Uni_AmbientLight;
uniform DirectionalLight
{
    vec3 Color;
    vec3 Direction;
}
Uni_DirectionalLight;

void main()
{
    vec3 worldNormal = normalize(mat3(Uni_Model) * In_Normal);

    vec3 diffuse = max(dot(worldNormal, Uni_DirectionalLight.Direction), 0.0) * Uni_DirectionalLight.Color;

    Out.Color = vec4(Uni_AmbientLight.Color + diffuse, 1.0);

    gl_Position = Uni_Projection * Uni_View * Uni_Model * vec4(In_Position, 1.0);
}