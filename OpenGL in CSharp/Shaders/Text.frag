#version 440
//in vec2 TexCoords;
//
//uniform sampler2D text;
//uniform vec3 textColor;
//
//layout (location = 0) out vec4 color;
//
//void main()
//{    
//    vec4 sampled = vec4(1.0, 1.0, 1.0, texture(text, TexCoords).r);
//    color = vec4(textColor, 1.0) * sampled;
//} 
//

in vec2 vUV;

layout (binding=0) uniform sampler2D u_texture;

layout (location = 2) uniform vec3 textColor;

out vec4 fragColor;

void main()
{
    vec2 uv = vUV.xy;
    float text = texture(u_texture, uv).r;
    fragColor = vec4(textColor.rgb*text, text);
}