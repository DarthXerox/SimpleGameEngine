#version 440

struct Fog {
	vec3 color;
	float density;
};


struct Light {
    vec4  position;
    vec3  direction;
    float cutOff;
    float outerCutOff;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;
};

uniform layout (location = 0) mat4 model;
uniform layout (location = 1) mat4 view;
uniform layout (location = 2) mat4 projection;
uniform layout (location = 5) vec3 camPosition;




uniform  layout (location = 50) Fog fog;


layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoors;
layout (location = 2) in vec3 normals;
layout (location = 13) in vec3 tangent;
layout (location = 14) in vec3 biTangent;

layout (location = 0) out vec3 out_position;
layout (location = 1) out vec2 out_texCoors;
layout (location = 2) out vec3 out_normals;
layout (location = 3) out float fogFactor;
layout (location = 4) out mat3 TBN;
layout (location = 5) out vec3 out_tangPos;

const float LOG2 = 1.442695;

void main(void) {

    out_position = vec3(model * vec4(position, 1.0));   
    out_texCoors = texCoors;

    
    mat3 normalMatrix = transpose(inverse(mat3(model)));
    vec3 T = normalize(normalMatrix * tangent);
    vec3 N = normalize(normalMatrix * normals);
    T = normalize(T - dot(T, N) * N);
    vec3 B = cross(N, T);
    
    mat3 TBN = transpose(mat3(T, B, N));    
    vs_out.TangentLightPos = TBN * lightPos;
    vs_out.TangentViewPos  = TBN * viewPos;
    vs_out.TangentFragPos  = TBN * vs_out.FragPos;
        
    gl_Position = projection * view * model * vec4(aPos, 1.0);


    out_position = (model * vec4(position, 1)).xyz;

    //vec3 relativeToCam = (view * model * vec4(position, 1)).xyz;
    float fogLen = length(camPosition - out_position);
	fogFactor = exp2(-fog.density * fog.density * fogLen * fogLen * LOG2);
	fogFactor = clamp(fogFactor, 0.0, 1.0);

	out_normals = transpose(inverse(mat3(model))) * normals;
	out_texCoors = texCoors;

    gl_Position = projection * view * model * vec4(position, 1.0);


	vec3 T = normalize(vec3(model * vec4(tangent,   0.0)));
    vec3 B = normalize(vec3(model * vec4(biTangent, 0.0)));
    vec3 N = normalize(vec3(model * vec4(normals,    0.0)));
    TBN = mat3(T, B, N);

	
}
