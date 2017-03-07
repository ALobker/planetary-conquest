Shader "Planet/Surface" {
	Properties {
		[Header(Grass)]
		[NoScaleOffset] Grass_AlbedoTexture("Albedo", 2D) = "white" {}
		[NoScaleOffset] Grass_NormalTexture("Normal", 2D) = "bump" {}
		[NoScaleOffset] Grass_SurfaceTexture("Metallic (R), Smoothness (G), and Height (B)", 2D) = "black" {}
		Grass_TintColor("Tint", Color) = (1.0, 1.0, 1.0, 1.0)
		Grass_TextureScale("Scale", Float) = 1.0
		[ToggleOff] Grass_BitangentCorrection("Invert Green Channel of Normal", Float) = 0.0
		[ToggleOff] Grass_SmoothnessCorrection("Invert Smoothness", Float) = 0.0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 200
		
		CGPROGRAM
			#pragma surface surf Standard vertex:vert
			#pragma target 3.0

			#include "Planet.cginc"

			#define EPSILON 1e-3

			#define X_AXIS float3(1.0, 0.0, 0.0)
			#define Y_AXIS float3(0.0, 1.0, 0.0)
			#define Z_AXIS float3(0.0, 0.0, 1.0)
			
			
			sampler2D Grass_AlbedoTexture;
			sampler2D Grass_NormalTexture;
			sampler2D Grass_SurfaceTexture;

			float4 Grass_TintColor;

			float Grass_TextureScale;

			/**
			* Whether or not to correct (invert) the bitangent of the normal maps.
			*
			* Can be either 0 (don't correct) or 1 (do correct).
			*/
			float Grass_BitangentCorrection;

			float Grass_SmoothnessCorrection;


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
				float3 grassCoordinates = position / Grass_TextureScale;

				// Take the weighted average of the three sampled colors.
				fixed4 grassColor = 0.0;

				grassColor += sampleColor(Grass_AlbedoTexture, grassCoordinates.xy) * weightZ;
				grassColor += sampleColor(Grass_AlbedoTexture, grassCoordinates.yz) * weightX;
				grassColor += sampleColor(Grass_AlbedoTexture, grassCoordinates.zx) * weightY;

				// Take the weighted average of the three sampled normals in object space.
				float3 grassNormal = 0.0;

				grassNormal += sampleNormal(Grass_NormalTexture, grassCoordinates.xy, Grass_BitangentCorrection, X_AXIS, Y_AXIS, Z_AXIS, normal) * weightZ;
				grassNormal += sampleNormal(Grass_NormalTexture, grassCoordinates.yz, Grass_BitangentCorrection, Y_AXIS, Z_AXIS, X_AXIS, normal) * weightX;
				grassNormal += sampleNormal(Grass_NormalTexture, grassCoordinates.zx, Grass_BitangentCorrection, Z_AXIS, X_AXIS, Y_AXIS, normal) * weightY;

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

				// TODO Use surface texture etc.
				output.Albedo = grassColor.rgb * Grass_TintColor.rgb;
				output.Metallic = 0.0;
				output.Smoothness = correctSmoothness(grassColor.a, Grass_SmoothnessCorrection);
				output.Normal = grassNormal;
			}
		ENDCG
	}
	FallBack "Diffuse"
}
