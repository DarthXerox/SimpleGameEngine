#version 440

layout (location = 0) in vec2 texCoors;

out vec4 final_color;

uniform layout (binding = 3) sampler2D texture0;
uniform layout (binding = 4) sampler2D texture1;


void main(void)
{
    final_color = mix(texture(texture0, texCoors), texture(texture1, texCoors), 0.4);
}