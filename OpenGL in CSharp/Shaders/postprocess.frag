#version 440

// Gaussian kernel
const float gaussian_3x3[3][3] = {
	{1.0 / 16.0, 2.0 / 16.0, 1.0 / 16.0},
	{2.0 / 16.0, 4.0 / 16.0, 2.0 / 16.0},
	{1.0 / 16.0, 2.0 / 16.0, 1.0 / 16.0}
};

// Gaussian kernel 7x7
const float gaussian_7x7[7][7] = {
	{0.011362, 0.014962, 0.017649, 0.018648, 0.017649, 0.014962, 0.011362},
	{0.014962, 0.019703, 0.023240, 0.024556, 0.023240, 0.019703, 0.014962},
	{0.017649, 0.023240, 0.027413, 0.028964, 0.027413, 0.023240, 0.017649},
	{0.018648, 0.024556, 0.028964, 0.030603, 0.028964, 0.024556, 0.018648},
	{0.017649, 0.023240, 0.027413, 0.028964, 0.027413, 0.023240, 0.017649},
	{0.014962, 0.019703, 0.023240, 0.024556, 0.023240, 0.019703, 0.014962},
	{0.011362, 0.014962, 0.017649, 0.018648, 0.017649, 0.014962, 0.011362}
};

// Edge detection kernel
const float edge_detection[3][3] = {
	{-1.0, -1.0, -1.0},
	{-1.0,  8.0, -1.0},
	{-1.0, -1.0, -1.0}
};

layout(binding = 0) uniform sampler2D input_image;

layout(location = 0) out vec4 final_color;

void main() {
	//  Uncomment and comment the kernel application below to make the post-process only pass through the given color
    //	vec3 color = texelFetch(input_image, ivec2(gl_FragCoord.xy), 0).rgb;

	vec3 color = vec3(0.0);
	const int size = 3;
	for(int i = -size; i <= size; i++) {
		for(int j = -size; j <= size; j++) {
			color += gaussian_7x7[i + size][j + size] * texelFetch(input_image, ivec2(gl_FragCoord.xy) + ivec2(i, j), 0).rgb;
		}
	}

	// https://en.wikipedia.org/wiki/Tone_mapping
	// http://filmicworlds.com/blog/filmic-tonemapping-operators/
	final_color = vec4(color / (color + 1.0), 1.0);	
	//final_color = vec4(0.5, 0.5, 0.5, 0.5);
}