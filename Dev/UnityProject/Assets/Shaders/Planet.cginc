/**
 * Returns -1 if the value is smaller than 0, and 1 if the value is larger than or equal to 0.
 */
inline float stretch(float value) {
	return step(0.0, value) * 2.0 - 1.0;
}

/**
 * Returns the first value if A is larger than or equal to B, and the second value if A
 * is smaller than B.
 */
inline float3 pick(float3 value1, float3 value2, float a, float b) {
	return lerp(value2, value1, step(b, a));
}

/**
 * Changes the sign of the value along the specified cardinal axis.
 */
inline float3 flip(float3 value, float newSign, float3 axis) {
	return (1 - axis) * value + newSign * axis * value;
}


/**
 * Flips the tangent space normal along the bitangent axis if requested.
 */
float3 correctBitangent(float3 normal, float correction) {
	float correctness = 1.0 - correction;
	float signum = correctness * 2.0 - 1.0;

	normal.y = signum * normal.y;

	return normal;
}

/**
 * Invert the smoothness if requested.
 */
float3 correctSmoothness(float smoothness, float correction) {
	float correctness = 1.0 - correction;

	return lerp(1.0 - smoothness, smoothness, correctness);
}


/**
 * Samples the color from the texture using the specified coordinates.
 */
float4 sampleColor(sampler2D colors, float2 coordinates) {
	return tex2D(colors, coordinates);
}

/**
 * Samples the normal from the texture using the specified coordinates, flips it along the
 * bitangent axis if requested, transforms it from tangent space to object space, and flips it to
 * the same side of the tangent plane as the surface normal.
 */
float3 sampleNormal(sampler2D normals, float2 coordinates, float bitangentCorrection, float3 tangent, float3 bitangent, float3 normal, float3 surfaceNormal) {
	// Create the tangent space transformations for the tangent space normal.
	float3x3 objectSpaceToTangentSpace = float3x3(tangent, bitangent, normal);
	float3x3 tangentSpaceToObjectSpace = transpose(objectSpaceToTangentSpace);

	// Sample the tangent space normal and transform it to object space.
	float3 normalInTangentSpace = correctBitangent(UnpackNormal(tex2D(normals, coordinates)), bitangentCorrection);
	float3 normalInObjectSpace = mul(tangentSpaceToObjectSpace, normalInTangentSpace);

	// Flip the sampled normal over its tangent plane (in object space) if it is not
	// on the same side of its tangent plane as the surface normal. This way it will
	// always face outwards. This is necessary since normals have a direction, as
	// opposed to the other values such as color, which are scalars.
	return flip(normalInObjectSpace, stretch(dot(surfaceNormal, normal)), normal);
}
