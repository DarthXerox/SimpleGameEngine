#version 440

layout (location = 0) in vec2 texCoors;

out vec4 final_color;

uniform layout (binding = 3) sampler2D texture0;

void main(void)
{
    final_color = texture(texture0, texCoors);
    //final_color = vec4(texCoors, 1.0f, 1.0f);
}