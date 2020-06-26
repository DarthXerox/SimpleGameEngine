#version 440

struct Fog {
	vec3 color;
	float density;
};


uniform layout (location = 0) mat4 model;
uniform layout (location = 1) mat4 view;
uniform layout (location = 2) mat4 projection;
uniform layout (location = 5) vec3 camPosition;
uniform  layout (location = 50) Fog fog;

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoors;
layout (location = 2) in vec3 normals;

layout (location = 0) out vec3 out_position;
layout (location = 1) out vec2 out_texCoors;
layout (location = 2) out vec3 out_normals;
layout (location = 3) out float fogFactor;


const float LOG2 = 1.442695;

void main(void) {
    //vec4 pos = projection * transform * modelview * vec4( position,  1.0);
    //out_position = pos.xyz;
    //gl_Position = pos;

    //out_texCoors = texCoors;
    //out_normals = ( transform * vec4(normals, 1.0)).xyz;

    

    out_position = (model * vec4(position, 1)).xyz;

    //vec3 relativeToCam = (view * model * vec4(position, 1)).xyz;
    float fogLen = length(camPosition - out_position);
	fogFactor = exp2(-fog.density * fog.density * fogLen * fogLen * LOG2);
	fogFactor = clamp(fogFactor, 0.0, 1.0);

	out_normals = transpose(inverse(mat3(model))) * normals;
	out_texCoors = texCoors;

    gl_Position = projection * view * model * vec4(position, 1.0);
}