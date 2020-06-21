#version 440

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

struct Fog {
	vec3 color;
	float density;
};

layout (location = 3) uniform vec4 lightPosition;
layout (location = 4) uniform vec3 lightColor;
layout (location = 5) uniform vec3 camPosition;

layout (location = 8) uniform vec3 materialAmbientColor;
layout (location = 9) uniform vec3 materialDiffuseColor;
layout (location = 10) uniform vec3 materialSpecularColor;
layout (location = 11) uniform float materialShininess;

layout (location = 12) uniform Light light;
uniform Light moon;
uniform Fog fog;

uniform layout (binding = 0) sampler2D texture0;
//uniform layout (binding = 1) sampler2D texture1;

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoors;
layout (location = 2) in vec3 normals;

layout (location = 3) in float fogFactor;

out vec4 finalColor;


vec4 calculateColorSpotlight(in Light light) {
	vec3 toLightVec = light.position.xyz - position * light.position.w;
    vec3 L = normalize(toLightVec);
    vec3 N = normalize(normals);
	vec3 E = normalize(camPosition - position); 
	vec3 H = normalize(L + E); 
	float NdotL = max(dot(N, L), 0.0);
	float NdotH = max(dot(N, H), 0.0);

	float distance_ = lightPosition.w == 1.0 ? pow(length(toLightVec), 2) : 1.0;
	float attenuation = 1.0 / (light.constant + light.linear * distance_ + 
    		    light.quadratic * (distance_ * distance_)); 

	//vec3 ambient = texture(texture0, texCoors).xyz * light.ambient.rgb;
	vec3 ambient = materialAmbientColor.rgb * light.ambient.rgb;
	vec3 diffuse = materialDiffuseColor.rgb * light.diffuse.rgb * texture(texture0, texCoors).xyz;
	vec3 specular = materialSpecularColor.rgb * light.specular.rgb;

	float theta = dot(L, normalize(-light.direction));
	float epsilon = light.cutOff - light.outerCutOff;
	float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);  

	ambient *= attenuation;
	diffuse *= attenuation * intensity;
	specular *= attenuation * intensity;

	vec3 color = ambient.rgb +
		NdotL * diffuse.rgb +
		pow(NdotH, materialShininess) * specular.rgb;
	color /= distance_;

	return vec4(mix(fog.color, color, fogFactor), 1.0);
}
 


void main(void) {
	if (texture(texture0, texCoors).a < 0.5) {
		discard;
	}

	finalColor = calculateColorSpotlight(light);


//    vec3 toLightVec = lightPosition.xyz - position * lightPosition.w;
//    vec3 N = normalize(normals);
//
//    vec3 L = normalize(toLightVec);
//	// Normal vector of the current fragment	
//	vec3 E = normalize(camPosition - position); // Direction from the current fragment to the camera	
//	vec3 H = normalize(L + E); // Half vector between (E)ye and L
//	float NdotL = max(dot(N, L), 0.0);
//		float NdotH = max(dot(N, H), 0.0);
//
//		float distance_ = lightPosition.w == 1.0 ? pow(length(toLightVec), 2) : 1.0;
//
//		vec3 diffuse = materialDiffuseColor * lightColor * texture(texture0, texCoors).xyz;
//
//		vec3 specular = materialSpecularColor;
//
//		//materialAmbientColor +
//		//lightColor * texture(texture0, texCoors).xyz
//		vec3 color = lightColor * texture(texture0, texCoors).xyz +
//			NdotL * diffuse +
//			pow(NdotH, materialShininess) * specular;
//
//
//		finalColor = vec4(color / distance_, 1.0);
}
