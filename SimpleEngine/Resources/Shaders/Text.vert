#version 440

layout (location = 0) in vec2 in_pos;
layout (location = 1) in vec2 in_uv;


layout (location = 0) uniform mat4 model;
layout (location = 1) uniform mat4 projection;

layout (location = 0) out vec2 texCoords;

            
void main()
{
    texCoords = in_uv;
	gl_Position = projection * model * vec4(in_pos.xy, 0.0, 1.0);
}
