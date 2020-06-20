#version 440

uniform layout (location = 0) mat4 model;
uniform layout (location = 1) mat4 view;
uniform layout (location = 2) mat4 projection;

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoors;
layout (location = 2) in vec3 normals;

layout (location = 0) out vec3 out_position;
layout (location = 1) out vec2 out_texCoors;
layout (location = 2) out vec3 out_normals;

void main(void) {
    //vec4 pos = projection * transform * modelview * vec4( position,  1.0);
    //out_position = pos.xyz;
    //gl_Position = pos;

    //out_texCoors = texCoors;
    //out_normals = ( transform * vec4(normals, 1.0)).xyz;

    out_position = (model * vec4(position, 1)).xyz;
	out_normals = transpose(inverse(mat3(model))) * normals;
	out_texCoors = texCoors;

    gl_Position = projection * view * model * vec4(position, 1.0);
}