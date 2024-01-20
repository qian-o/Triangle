// Original author: https://www.shadertoy.com/user/iq
// Project link: https://www.shadertoy.com/view/Xds3zN

#version 460

#extension GL_GOOGLE_include_directive : require

#include "../../TrUtils.glsl"
#include "../../TrVertex.glsl"

void main()
{
    gl_Position = vec4(In_Position, 1.0);
}