#version 440

layout(location = 0) in vec3 color;

out vec4 final_color;


void main(void)
{
    final_color = vec4(color, 1.0);
}