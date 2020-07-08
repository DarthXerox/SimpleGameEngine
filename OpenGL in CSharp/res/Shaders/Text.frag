#version 440

layout (location = 0) in vec2 texCoords;

layout (binding = 0) uniform sampler2D u_texture;

layout (location = 2) uniform vec3 textColor;

out vec4 fragColor;

void main()
{
    float texture0 = texture(u_texture, texCoords).r;
    fragColor = vec4(textColor.rgb * texture0, texture0);
}