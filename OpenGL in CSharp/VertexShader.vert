#version 440

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 color;

layout (location = 0) out vec3 out_color;

uniform layout (location = 2) mat4 modelview;

void main(void) {
    gl_Position = modelview * vec4( position,  1.0);
    out_color = color;
}