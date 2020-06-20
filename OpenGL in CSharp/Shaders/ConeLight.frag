#version 440

// position.w == 0 means directional light
// else if cutOff == -1 means a point light
// else its a spotlight
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

struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float shininess;
};

uniform Light light;
uniform vec3 camPosition;
uniform Material material;

layout (binding = 0) uniform sampler2D texture0;

//uniform layout (binding = 1) sampler2D texture1;
//uniform layout (binding = 2) sampler2D normalTexture; //bumb texture

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoors;
layout (location = 2) in vec3 normals;

out vec4 finalColor; 

void main() {
    vec4 textureColor = texture(texture0, texCoors);
    if (textureColor.a < 0.5) { // fir needles, grass...
        discard;
    }


    vec3 toLightVec = light.position.xyz - position * light.position.w;
    vec3 N = normalize(normals);
    vec3 L = normalize(toLightVec);

	vec3 E = normalize(camPosition - position); 
	vec3 H = normalize(L + E);

	float NdotL = max(dot(N, L), 0.0);
	float NdotH = max(dot(N, H), 0.0);

	float distance2 = light.position.w == 1.0 ? pow(length(toLightVec), 2) : 1.0;

	vec3 ambient = object.ambient_color.rgb * light.ambient_color.rgb;
	vec3 diffuse = texture(diffuse_texture, fs_texture_coordinate).rgb * object.diffuse_color.rgb * light.diffuse_color.rgb;
	vec3 specular = object.specular_color.rgb * light.specular_color.rgb;

	vec3 color = ambient.rgb
		+ NdotL * diffuse.rgb
		+ pow(NdotH, object.specular_color.w) * specular;
	color /= distance2;


	

	final_color = vec4(color, 1.0);


}
