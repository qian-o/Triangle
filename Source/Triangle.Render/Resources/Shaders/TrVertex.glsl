#ifdef VULKAN
#define INSTANCE_ID gl_InstanceIndex
#else
#define INSTANCE_ID gl_InstanceID
#endif

#define TANGENT_SPACE_ROTATION                                                                                         \
    vec3 binormal = cross(normalize(In_Normal), normalize(In_Tangent));                                                \
    mat3 rotation = transpose(mat3(In_Tangent, binormal, In_Normal));

layout(location = 0) in vec3 In_Position;
layout(location = 1) in vec3 In_Normal;
layout(location = 2) in vec3 In_Tangent;
layout(location = 3) in vec3 In_Bitangent;
layout(location = 4) in vec4 In_Color;
layout(location = 5) in vec2 In_TexCoord;