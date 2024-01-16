#version 320 es

precision highp float;

in VertexData
{
    vec3 WorldNormal;
    vec3 WorldPos;
    vec2 UV;
}
In;

out vec4 Out_Color;

uniform sampler2D MainTex;
uniform vec4 MainTexST;

layout(std140, binding = 1) uniform Vectors
{
    vec3 CameraPosition;
}
Uni_Vectors;

layout(std140, binding = 2) uniform Material
{
    vec4 Color;
    vec4 Specular;
    float Gloss;
}
Uni_Material;

layout(std140, binding = 3) uniform AmbientLight
{
    vec3 Color;
}
Uni_AmbientLight;

layout(std140, binding = 4) uniform DirectionalLight
{
    vec3 Position;
    vec3 Direction;
    vec3 Color;
}
Uni_DirectionalLight;

float saturate(float value)
{
    return clamp(value, 0.0, 1.0);
}

vec2 transformUV(vec2 uv, vec4 st)
{
    return uv * st.xy + st.zw;
}

void main()
{
    vec3 worldNormal = normalize(In.WorldNormal);
    vec3 worldLightDir = normalize(Uni_DirectionalLight.Position);

    vec3 albedo = texture(MainTex, transformUV(In.UV, MainTexST)).rgb * Uni_Material.Color.rgb;

    vec3 ambient = albedo * Uni_AmbientLight.Color;

    vec3 diffuse = albedo * Uni_DirectionalLight.Color * saturate(dot(worldNormal, worldLightDir));

    vec3 viewDir = normalize(Uni_Vectors.CameraPosition - In.WorldPos);
    vec3 halfDir = normalize(worldLightDir + viewDir);
    vec3 specular = Uni_Material.Specular.rgb * Uni_DirectionalLight.Color *
                    pow(saturate(dot(worldNormal, halfDir)), Uni_Material.Gloss);

    Out_Color = vec4(ambient + diffuse + specular, 1.0);
}