#version 440

layout (location = 3) uniform vec4 lightPosition;
layout (location = 4) uniform vec3 lightColor;
layout (location = 5) uniform vec3 camPosition;

layout (location = 8) uniform vec3 materialAmbientColor;
layout (location = 9) uniform vec3 materialDiffuseColor;
layout (location = 10) uniform vec3 materialSpecularColor;
layout (location = 11) uniform float materialShininess;

uniform layout (binding = 0) sampler2D texture0;
//uniform layout (binding = 1) sampler2D texture1;

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoors;
layout (location = 2) in vec3 normals;

out vec4 finalColor;

void main(void)
{
	//if (texture(texture0, texCoors).a < 0.5) {
		//discard;
	//}
    vec3 toLightVec = lightPosition.xyz - position * lightPosition.w;
    vec3 N = normalize(normals);

    vec3 L = normalize(toLightVec);
	//vec3 N = normalize(fs_normal); // Normal vector of the current fragment	
	vec3 E = normalize(camPosition - position); // Direction from the current fragment to the camera	
	vec3 H = normalize(L + E); // Half vector between (E)ye and L

	float NdotL = max(dot(N, L), 0.0);
	float NdotH = max(dot(N, H), 0.0);

	float distance_ = lightPosition.w == 1.0 ? pow(length(toLightVec), 2) : 1.0;

	vec3 diffuse = materialDiffuseColor * lightColor * texture(texture0, texCoors).xyz;

	vec3 specular = materialSpecularColor;

	//materialAmbientColor +
	vec3 color = texture(texture0, texCoors).xyz +
		NdotL * diffuse +
		pow(NdotH, materialShininess) * specular;


	finalColor = vec4(color / distance_, 1.0);
}