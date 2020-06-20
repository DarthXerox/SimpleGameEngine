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

layout (location = 3) uniform vec4 lightPosition;
layout (location = 4) uniform vec3 lightColor;
layout (location = 5) uniform vec3 camPosition;

layout (location = 8) uniform vec3 materialAmbientColor;
layout (location = 9) uniform vec3 materialDiffuseColor;
layout (location = 10) uniform vec3 materialSpecularColor;
layout (location = 11) uniform float materialShininess;

layout (location = 12) uniform Light light;

uniform layout (binding = 0) sampler2D texture0;
//uniform layout (binding = 1) sampler2D texture1;

layout (location = 0) in vec3 position;
layout (location = 1) in vec2 texCoors;
layout (location = 2) in vec3 normals;

out vec4 finalColor;


vec3 calculateDiffuse() {
	return vec3(0,0,0);
}
 

void main(void) {
	if (texture(texture0, texCoors).a < 0.5) {
		discard;
	}


    vec3 toLightVec = light.position.xyz - position * light.position.w;
    vec3 L = normalize(toLightVec);

//	float theta     = dot(L, normalize(-light.direction));
//	float epsilon   = light.cutOff - light.outerCutOff;
//	float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);  
//	if (theta > light.cutOff) {
//        finalColor = vec4(1,0, 0, 1);
//	} else {
//        finalColor = vec4(0,1, 0, 1);
//        }


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

	float theta     = dot(L, normalize(-light.direction));
	float epsilon   = light.cutOff - light.outerCutOff;
	float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);  

	ambient *= attenuation;
	diffuse *= attenuation * intensity;
	specular *= attenuation * intensity;

	vec3 color = ambient.rgb +
		NdotL * diffuse.rgb +
		pow(NdotH, materialShininess) * specular.rgb;
	color /= distance_;

	finalColor = vec4(color , 1.0);


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
//


//	vec3 toLightVec = coneLight.position.xyz - position; //* coneLight.position.w;
//    vec3 L = normalize(toLightVec);
//	float theta = dot(L, normalize(-coneLight.direction));
//
//	if (theta > coneLight.cutOff) {
//		vec3 N = normalize(normals);
//
//		// Normal vector of the current fragment	
//		vec3 E = normalize(camPosition - position); // Direction from the current fragment to the camera	
//		vec3 H = normalize(L + E); // Half vector between (E)ye and L
//		float NdotL = max(dot(N, L), 0.0);
//		float NdotH = max(dot(N, H), 0.0);
//
//		float distance_ = coneLight.position.w == 1.0 ? pow(length(toLightVec), 2) : 1.0;
//
//		vec3 diffuse = materialDiffuseColor * coneLight.diffuse * texture(texture0, texCoors).xyz;
//
//		vec3 specular = materialSpecularColor * coneLight.specular;
//
//		//materialAmbientColor +
//		//lightColor * texture(texture0, texCoors).xyz
//		vec3 color = coneLight.ambient * texture(texture0, texCoors).xyz +
//			NdotL * diffuse +
//			pow(NdotH, materialShininess) * specular;
//
//
//		finalColor = vec4(color / distance_, 1.0);
//		//finalColor = vec4(0,1, 0, 1);
//	} else {
//		finalColor = vec4(lightColor * texture(texture0, texCoors).xyz, 1.0);
//	}

	
}