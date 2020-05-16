#version 440

uniform layout (location = 3) vec4 lightPosition;
uniform layout (location = 4) vec3 lightColor;
uniform layout (binding = 0) sampler2D texture0;
//uniform layout (binding = 1) sampler2D texture1;

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoors;
layout (location = 2) in vec3 normals;

out vec4 final_color;

void main(void)
{
    vec3 toLightVec = normalize(lightPosition.xyz - position * lightPosition.w);
    vec3 normalsVec = normalize(normals);

    float brightness = max(dot(normalsVec, toLightVec), 0.0);
    vec3 diffuse = brightness * lightColor;

    final_color =  vec4(diffuse, 1.0) * texture(texture0, texCoors);
}