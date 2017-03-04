Shader "Planet/Surface" {
	Properties {
		[Header(Grass)]
		[NoScaleOffset] GrassTexture("Grass Texture", 2D) = "white" {}
		[NoScaleOffset] GrassNormals("Grass Normals", 2D) = "white" {}
		GrassScale("Grass Scale", Float) = 1.0

		[Header(Options)]
		[ToggleOff] BitangentCorrection("Invert Green Channel of Normal Maps", Float) = 0.0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 200
		
		CGPROGRAM
			#pragma surface surf Standard vertex:vert
			#pragma target 3.0


			#define EPSILON 1e-3

			#define X_AXIS float3(1.0, 0.0, 0.0)
			#define Y_AXIS float3(0.0, 1.0, 0.0)
			#define Z_AXIS float3(0.0, 0.0, 1.0)


			/**
			 * Whether or not to correct (invert) the bitangent of the normal maps.
			 * 
			 * Can be either 0 (don't correct) or 1 (do correct).
			 */
			float BitangentCorrection;
			
			sampler2D GrassTexture;
			sampler2D GrassNormals;
			float GrassScale;

			float WaterLevel;


			struct Input {
				float3 position;
				float3 normal;
			};


			void vert(inout appdata_full data, out Input input) {
				UNITY_INITIALIZE_OUTPUT(Input, input);

				input.position = data.vertex.xyz;
				input.normal = data.normal.xyz;

				// Since we work with object space normals, replace the tangent space in such a way
				// that the transformation from object space is the identity transformation. Then we
				// won't have to bother with tangent space transformations in the surface shader.
				data.tangent = float4(X_AXIS, 1);
				data.normal = float4(Z_AXIS, 0);
			}


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
			 * Flips the tangent space normal along the bitangent axis if configured to do so.
			 */
			float3 correctBitangent(float3 normal) {
				float bitangentCorrectness = 1.0 - BitangentCorrection;
				float bitangentSign = bitangentCorrectness * 2.0 - 1.0;

				normal.y = bitangentSign * normal.y;

				return normal;
			}

			/**
			 * Samples the color from the texture using the specified coordinates.
			 */
			float4 sampleColor(sampler2D colors, float2 coordinates) {
				return tex2D(colors, coordinates);
			}

			/**
			 * Samples the normal from the texture using the specified coordinates, transforms it
			 * from tangent space to object space, and flips it to the same side of the tangent
			 * plane as the surface normal.
			 */
			float3 sampleNormal(sampler2D normals, float2 coordinates, float3 tangent, float3 bitangent, float3 normal, float3 surfaceNormal) {
				// Create the tangent space transformations for the tangent space normal.
				float3x3 objectSpaceToTangentSpace = float3x3(tangent, bitangent, normal);
				float3x3 tangentSpaceToObjectSpace = transpose(objectSpaceToTangentSpace);

				// Sample the tangent space normal and transform it to object space.
				float3 normalInTangentSpace = correctBitangent(UnpackNormal(tex2D(normals, coordinates)));
				float3 normalInObjectSpace = mul(tangentSpaceToObjectSpace, normalInTangentSpace);

				// Flip the sampled normal over its tangent plane (in object space) if it is not
				// on the same side of its tangent plane as the surface normal. This way it will
				// always face outwards. This is necessary since normals have a direction, as
				// opposed to the other values such as color, which are scalars.
				return flip(normalInObjectSpace, stretch(dot(surfaceNormal, normal)), normal);
			}
			
			void surf(Input input, inout SurfaceOutputStandard output) {
				float3 position = input.position;
				float3 normal = input.normal;

				// Get a measure of how close the surface normal is to each of the three cardinal
				// axes. Since a dot product with a cardinal axis is just the corresponding
				// component of the input vector, we can just use those directly instead.
				float measureX = abs(normal.x);
				float measureY = abs(normal.y);
				float measureZ = abs(normal.z);

				// Use the measures to determine how much each cardinal axis contributes to the
				// surface normal.
				float measureTotal = measureX + measureY + measureZ;

				float weightX = measureX / measureTotal;
				float weightY = measureY / measureTotal;
				float weightZ = measureZ / measureTotal;

				// Repeat the textures according to the supplied scale.
				float3 grassCoordinates = position / GrassScale;

				// Take the weighted average of the three sampled colors.
				fixed4 grassColor = 0.0;

				grassColor += sampleColor(GrassTexture, grassCoordinates.xy) * weightZ;
				grassColor += sampleColor(GrassTexture, grassCoordinates.yz) * weightX;
				grassColor += sampleColor(GrassTexture, grassCoordinates.zx) * weightY;

				// Take the weighted average of the three sampled normals in object space.
				float3 grassNormal = 0.0;

				grassNormal += sampleNormal(GrassNormals, grassCoordinates.xy, X_AXIS, Y_AXIS, Z_AXIS, normal) * weightZ;
				grassNormal += sampleNormal(GrassNormals, grassCoordinates.yz, Y_AXIS, Z_AXIS, X_AXIS, normal) * weightX;
				grassNormal += sampleNormal(GrassNormals, grassCoordinates.zx, Z_AXIS, X_AXIS, Y_AXIS, normal) * weightY;

				// If the weighted average is (near) the zero vector we can use the surface normal
				// instead. This is analogous to taking a weighted average over the unit sphere,
				// because the exact centroid would then be (near) the surface normal.
				grassNormal = pick(grassNormal, normal, length(grassNormal), EPSILON);

				// Now we can safely make sure the normal has the unit length.
				grassNormal = normalize(grassNormal);
				
				// If the weighted average produces a normal that points inwards, we can use the
				// inversion instead. This is analogous to taking a weighted average over the long
				// angles of a unit sphere. This means it always produces a weighted average in the
				// hemisphere centered on the surface normal (i.e. above the surface).
				grassNormal = stretch(dot(grassNormal, normal)) * grassNormal;

				output.Albedo = grassColor.rgb;
				output.Metallic = 0.0;
				output.Smoothness = grassColor.a;
				output.Normal = grassNormal;
			}
		ENDCG
	}
	FallBack "Diffuse"
}
