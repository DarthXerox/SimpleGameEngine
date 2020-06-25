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
uniform vec3 tangent;
uniform vec3 biTangent;

layout (location = 0) out vec3 out_position;
layout (location = 1) out vec2 out_texCoors;
layout (location = 2) out vec3 out_normals;
layout (location = 3) out float fogFactor;
layout (location = 4) out mat3 TBN;

const float LOG2 = 1.442695;

void main(void) {
    out_position = (model * vec4(position, 1)).xyz;

    //vec3 relativeToCam = (view * model * vec4(position, 1)).xyz;
    float fogLen = length(camPosition - out_position);
	fogFactor = exp2(-fog.density * fog.density * fogLen * fogLen * LOG2);
	fogFactor = clamp(fogFactor, 0.0, 1.0);

	mat3 normalMatrix = transpose(inverse(mat3(model)));
	out_normals = normalMatrix * normals;
	vec3 T = normalize(vec3(normalMatrix * tangent));
    vec3 B = normalize(vec3(normalMatrix * biTangent));
    vec3 N = normalize(vec3(normalMatrix * normals));
    TBN = mat3(T, B, N);

	out_texCoors = texCoors;
    gl_Position = projection * view * model * vec4(position, 1.0);
}
