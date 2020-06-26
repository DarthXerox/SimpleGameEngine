#version 440
//layout (location = 0) in vec2 position;
//layout (location = 1) in vec2 texCoords;
//
//uniform mat4 projection;
//
//out vec2 TexCoords;
//
//void main()
//{
//    gl_Position = projection * vec4(position, 0.0, 1.0);
//    TexCoords = texCoords;
//}

layout (location = 0) in vec2 in_pos;
layout (location = 1) in vec2 in_uv;

out vec2 vUV;

layout (location = 0) uniform mat4 model;
layout (location = 1) uniform mat4 projection;
            
void main()
{
    vUV         = in_uv.xy;
	gl_Position = projection * model * vec4(in_pos.xy, 0.0, 1.0);
}
