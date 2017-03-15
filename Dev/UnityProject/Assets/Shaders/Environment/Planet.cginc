/**
 * A small value used during float comparisons.
 */
#define EPSILON 1e-3

/**
 * The cardinal axes.
 */
#define X_AXIS float3(1.0, 0.0, 0.0)
#define Y_AXIS float3(0.0, 1.0, 0.0)
#define Z_AXIS float3(0.0, 0.0, 1.0)


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
	return lerp(value, newSign * value, axis);
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
* Removes the need for the tangent space transformation that is normally performed by a surface
* shader.
*
* Note that this replaces the surface normal in the vertex data, so if you need it you should
* access it before calling this function.
*/
void disableTangentSpaceTransformation(inout appdata_full data) {
	// Replace the tangent space in such a way that the transformation from object space is the
	// identity transformation.
	data.tangent = float4(X_AXIS, 1);
	data.normal = float4(Z_AXIS, 0);
}


/**
* Calculates a set of normalized weights that represent how much each cardinal axis contributes to
* the surface normal.
*/
float3 calculateWeights(float3 surfaceNormal) {
	// Get a measure of how close the surface normal is to each of the three cardinal
	// axes. Since a dot product with a cardinal axis is just the corresponding
	// component of the input vector, we can just use those directly instead.
	float3 measures = abs(surfaceNormal);

	// Use the measures to determine how much each cardinal axis contributes to the
	// surface normal.
	return measures / (measures.x + measures.y + measures.z);
}


/**
 * Samples the color from the texture using the specified coordinates.
 */
float3 sampleColor(sampler2D colors, float2 coordinates) {
	// Sample only the color channels in case there is an alpha channel.
	return tex2D(colors, coordinates).rgb;
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

/**
 * Samples the surface properties from the texture using the specified coordinates, and inverts the
 * smoothness if requested.
 */
float3 sampleSurface(sampler2D surfaces, float2 coordinates, float smoothnessCorrection) {
	// Since the surface properties are stored per channel, we can just sample their combination instead.
	float3 surface = sampleColor(surfaces, coordinates);

	// Invert the smoothness if requested.
	surface.g = correctSmoothness(surface.g, smoothnessCorrection);

	return surface;
}


/**
 * Samples a color from the texture using a three dimensional interpolation based on the weights.
 */
float3 sampleLayerColor(float3 position, float scale, sampler2D colors, float3 weights) {
	// Repeat the texture according to the supplied scale.
	float3 coordinates = position / scale;

	// Take the weighted average of the three sampled values.
	float3 color = float3(0.0, 0.0, 0.0);

	color += sampleColor(colors, coordinates.xy) * weights.z;
	color += sampleColor(colors, coordinates.yz) * weights.x;
	color += sampleColor(colors, coordinates.zx) * weights.y;

	return color;
}

/**
 * Samples a normal from the texture using a three dimensional interpolation based on the weights.
 */
float3 sampleLayerNormal(float3 position, float scale, sampler2D normals, float bitangentCorrection, float3 surfaceNormal, float3 weights) {
	// Repeat the texture according to the supplied scale.
	float3 coordinates = position / scale;

	// Take the weighted average of the three sampled normals in object space.
	float3 normal = float3(0.0, 0.0, 0.0);

	normal += sampleNormal(normals, coordinates.xy, bitangentCorrection, X_AXIS, Y_AXIS, Z_AXIS, surfaceNormal) * weights.z;
	normal += sampleNormal(normals, coordinates.yz, bitangentCorrection, Y_AXIS, Z_AXIS, X_AXIS, surfaceNormal) * weights.x;
	normal += sampleNormal(normals, coordinates.zx, bitangentCorrection, Z_AXIS, X_AXIS, Y_AXIS, surfaceNormal) * weights.y;

	// If the weighted average is (near) the zero vector we can use the surface normal
	// instead. This is analogous to taking a weighted average over the unit sphere,
	// because the exact centroid would then be (near) the surface normal.
	normal = pick(normal, surfaceNormal, length(normal), EPSILON);

	// Now we can safely make sure the normal has the unit length.
	normal = normalize(normal);

	// If the weighted average produces a normal that points inwards, we can use the
	// inversion instead. This is analogous to taking a weighted average over the long
	// angles of a unit sphere. This means it always produces a weighted average in the
	// hemisphere centered on the surface normal (i.e. above the surface).
	return stretch(dot(normal, surfaceNormal)) * normal;
}

/**
* Samples surface properties from the texture using a three dimensional interpolation based on the weights.
*/
float3 sampleLayerSurface(float3 position, float scale, sampler2D surfaces, float smoothnessCorrection, float3 weights) {
	// Repeat the texture according to the supplied scale.
	float3 coordinates = position / scale;

	// Take the weighted average of the three sampled values.
	float3 surface = float3(0.0, 0.0, 0.0);

	surface += sampleSurface(surfaces, coordinates.xy, smoothnessCorrection) * weights.z;
	surface += sampleSurface(surfaces, coordinates.yz, smoothnessCorrection) * weights.x;
	surface += sampleSurface(surfaces, coordinates.zx, smoothnessCorrection) * weights.y;

	return surface;
}


/**
 * Samples the metal from the surface vector.
 */
float sampleSurfaceMetallic(float3 surface) {
	return surface.r;
}

/**
 * Samples the smoothness from the surface vector.
 */
float sampleSurfaceSmoothness(float3 surface) {
	return surface.g;
}

/**
 * Samples the height from the surface vector.
 */
float sampleSurfaceHeight(float3 surface) {
	return surface.b;
}


/**
 * Performs a height-adjusted linear blend between the two colors.
 * 
 * Based on http://www.gamasutra.com/blogs/AndreyMishkinis/20130716/196339/Advanced_Terrain_Texture_Splatting.php,
 * but truncates the depth to the alpha value so height blending does not occur outside of the interpolation interval.
 */
float3 blendLayerColors(float3 color1, float height1, float3 color2, float height2, float depth, float alpha) {
	// Vectorize the weights.
	float2 heights = float2(height1, height2);
	float2 alphas = float2(1.0 - alpha, alpha);

	// Incorporate the height into the weights.
	heights = alphas + heights;

	// Calculate the overall height.
	float height = max(heights.x, heights.y);

	// Calculate the depths to use. This limits blending to the interpolation interval.
	float2 depths = min(depth, alphas);

	// Calculate the weights as the relative distances between the heights and a lower bound determined by the depths.
	// These weights do not have to be divided by the total distance, because they will be normalized later anyways.
	float2 weights = saturate(heights - (height - depths));

	// Interpolate between the two colors based on the adjusted weights.
	return (color1 * weights.x + color2 * weights.y) / (weights.x + weights.y);
}

/**
* Performs a height-adjusted linear blend between the two normals.
* 
* See above for more details regarding the implementation.
*/
float3 blendLayerNormals(float3 normal1, float height1, float3 normal2, float height2, float depth, float alpha) {
	// Blending normals is just a component-wise weighted interpolation. This is exactly what is
	// used to blend colors, so we can overload to that. Color weights are also normalized, which
	// is not necessary for normals as we normalize them to unit length anyways, but we still
	// overload to reduce code duplication.
	return normalize(blendLayerColors(normal1, height1, normal2, height2, depth, alpha));
}

/**
* Performs a height-adjusted linear blend between the two surfaces.
* 
* See above for more details regarding the implementation.
*/
float3 blendLayerSurfaces(float3 surface1, float height1, float3 surface2, float height2, float depth, float alpha) {
	// Blending surfaces is just a component-wise weighted interpolation. This is exactly what is
	// used to blend colors, so we can overload to that.
	return blendLayerColors(surface1, height1, surface2, height2, depth, alpha);
}
