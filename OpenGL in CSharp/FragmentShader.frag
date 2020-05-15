#version 440

layout (location = 0) in vec2 texCoors;

out vec4 final_color;

uniform layout (binding = 0) sampler2D texture0;
//uniform layout (binding = 1) sampler2D texture1;


void main(void)
{
    final_color =  texture(texture0, texCoors);
}