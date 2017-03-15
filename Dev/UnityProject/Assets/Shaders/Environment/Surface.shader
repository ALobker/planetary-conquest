/**
 * The shader for the surface of the planet.
 * 
 * Since each layer has the same properties, it is easier to document them here:
 * 
 * AlbedoTexture:        The colors of the layer.
 * NormalTexture:        The normals of the layer.
 * SurfaceTexture:       The metals of the layer (in the red channel), the smoothness of the layer
 *                       (in the green channel), and the height of the layer (in the blue channel).
 * TextureScale:         The resizing of the texture in world space.
 * BitangentCorrection:  Whether or not to correct (invert) the bitangent of the normal. Can be
 *                       either 0 (don't correct) or 1 (do correct).
 * SmoothnessCorrection: Whether or not to correct (invert) the smoothness. Can be either 0 (don't
 *                       correct) or 1 (do correct).
 */
Shader "Planet/Surface" {
	Properties {
		// Material properties for the rock layer. See above for their descriptions.
		[Header(Rock)]
		[NoScaleOffset] Rock_AlbedoTexture("Albedo", 2D) = "white" {}
		[NoScaleOffset] Rock_NormalTexture("Normal", 2D) = "bump" {}
		[NoScaleOffset] Rock_SurfaceTexture("Metallic (R), Smoothness (G), and Height (B)", 2D) = "black" {}
		Rock_TextureScale("Scale", Float) = 1.0
		[Space]
		[ToggleOff] Rock_BitangentCorrection("Invert Green Channel of Normal", Float) = 0.0
		[ToggleOff] Rock_SmoothnessCorrection("Invert Smoothness", Float) = 0.0

		// Material properties for the sand layer. See above for their descriptions.
		[Header(Sand)]
		Sand_MinimumHeight("Minimum Height", Float) = 0.0
		Sand_MaximumHeight("Maximum Height", Float) = 0.0
		Sand_Distance("Distance", Float) = 0.0
		Sand_Depth("Depth", Float) = 1.0
		[Space]
		[NoScaleOffset] Sand_AlbedoTexture("Albedo", 2D) = "white" {}
		[NoScaleOffset] Sand_NormalTexture("Normal", 2D) = "bump" {}
		[NoScaleOffset] Sand_SurfaceTexture("Metallic (R), Smoothness (G), and Height (B)", 2D) = "black" {}
		Sand_TextureScale("Scale", Float) = 1.0
		[Space]
		[ToggleOff] Sand_BitangentCorrection("Invert Green Channel of Normal", Float) = 0.0
		[ToggleOff] Sand_SmoothnessCorrection("Invert Smoothness", Float) = 0.0

		// Material properties for the dirt layer. See above for their descriptions.
		[Header(Dirt)]
		[NoScaleOffset] Dirt_AlbedoTexture("Albedo", 2D) = "white" {}
		[NoScaleOffset] Dirt_NormalTexture("Normal", 2D) = "bump" {}
		[NoScaleOffset] Dirt_SurfaceTexture("Metallic (R), Smoothness (G), and Height (B)", 2D) = "black" {}
		Dirt_TextureScale("Scale", Float) = 1.0
		[Space]
		[ToggleOff] Dirt_BitangentCorrection("Invert Green Channel of Normal", Float) = 0.0
		[ToggleOff] Dirt_SmoothnessCorrection("Invert Smoothness", Float) = 0.0

		// Material properties for the grass layer. See above for their descriptions.
		[Header(Grass)]
		[NoScaleOffset] Grass_AlbedoTexture("Albedo", 2D) = "white" {}
		[NoScaleOffset] Grass_NormalTexture("Normal", 2D) = "bump" {}
		[NoScaleOffset] Grass_SurfaceTexture("Metallic (R), Smoothness (G), and Height (B)", 2D) = "black" {}
		Grass_TextureScale("Scale", Float) = 1.0
		[Space]
		[ToggleOff] Grass_BitangentCorrection("Invert Green Channel of Normal", Float) = 0.0
		[ToggleOff] Grass_SmoothnessCorrection("Invert Smoothness", Float) = 0.0

		// Material properties for the snow layer. See above for their descriptions.
		[Header(Snow)]
		[NoScaleOffset] Snow_AlbedoTexture("Albedo", 2D) = "white" {}
		[NoScaleOffset] Snow_NormalTexture("Normal", 2D) = "bump" {}
		[NoScaleOffset] Snow_SurfaceTexture("Metallic (R), Smoothness (G), and Height (B)", 2D) = "black" {}
		Snow_TextureScale("Scale", Float) = 1.0
		[Space]
		[ToggleOff] Snow_BitangentCorrection("Invert Green Channel of Normal", Float) = 0.0
		[ToggleOff] Snow_SmoothnessCorrection("Invert Smoothness", Float) = 0.0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 200
		
		CGPROGRAM
			#pragma surface surf Standard vertex:vert
			#pragma target 3.0


			// Make use of the common functionality of the planet related shaders.
			#include "Planet.cginc"
			
			
			// Material properties for the rock layer. See above for their descriptions.
			sampler2D Rock_AlbedoTexture;
			sampler2D Rock_NormalTexture;
			sampler2D Rock_SurfaceTexture;
			float Rock_TextureScale;

			float Rock_BitangentCorrection;
			float Rock_SmoothnessCorrection;


			// Material properties for the sand layer. See above for their descriptions.
			float Sand_MinimumHeight;
			float Sand_MaximumHeight;
			float Sand_Distance;
			float Sand_Depth;

			sampler2D Sand_AlbedoTexture;
			sampler2D Sand_NormalTexture;
			sampler2D Sand_SurfaceTexture;
			float Sand_TextureScale;

			float Sand_BitangentCorrection;
			float Sand_SmoothnessCorrection;


			// Material properties for the dirt layer. See above for their descriptions.
			sampler2D Dirt_AlbedoTexture;
			sampler2D Dirt_NormalTexture;
			sampler2D Dirt_SurfaceTexture;
			float Dirt_TextureScale;

			float Dirt_BitangentCorrection;
			float Dirt_SmoothnessCorrection;


			// Material properties for the grass layer. See above for their descriptions.
			sampler2D Grass_AlbedoTexture;
			sampler2D Grass_NormalTexture;
			sampler2D Grass_SurfaceTexture;
			float Grass_TextureScale;

			float Grass_BitangentCorrection;
			float Grass_SmoothnessCorrection;


			// Material properties for the snow layer. See above for their descriptions.
			sampler2D Snow_AlbedoTexture;
			sampler2D Snow_NormalTexture;
			sampler2D Snow_SurfaceTexture;
			float Snow_TextureScale;

			float Snow_BitangentCorrection;
			float Snow_SmoothnessCorrection;


			/**
			 * The distance between the surface of the water and the origin of the planet. This is
			 * used in calculations pertaining the height of the sand layer.
			 */
			float WaterLevel;


			/**
			 * The properties to interpolate to the surface shader (in object space).
			 */
			struct Input {
				float3 position;
				float3 surfaceNormal;
			};


			/**
			 * The vertex shader.
			 */
			void vert(inout appdata_full data, out Input input) {
				UNITY_INITIALIZE_OUTPUT(Input, input);

				// Interpolate the position and surface normal to the surface shader in object space.
				input.position = data.vertex.xyz;
				input.surfaceNormal = data.normal.xyz;

				// Since we work in object space, we have no need for the tangent space transformation
				// that a surface shader normally performs.
				disableTangentSpaceTransformation(data);
			}


			/**
			 * The surface shader.
			 */
			void surf(Input input, inout SurfaceOutputStandard output) {
				// The properties interpolated to the surface shader.
				float3 position = input.position;
				float3 surfaceNormal = input.surfaceNormal;

				// Get a measure of how much each cardinal axis contributes to the surface normal.
				float3 weights = calculateWeights(surfaceNormal);

				// Start with some default layer properties. This should be entirely overridden by
				// the rock layer if everything is set up right, but just in case it's not, you get
				// a simple grey surface. It also keeps the rock layer symmetric with the other
				// layers in terms of properties and code paths.
				float3 color = float3(0.5, 0.5, 0.5);
				float3 normal = surfaceNormal;
				float3 surface = float3(0.0, 0.0, 0.5);

				// TODO Sample all the layers. Combine them iteratively, with the resulting values replacing the previous ones each time.
				float3 rockColor = sampleLayerColor(position, Rock_TextureScale, Rock_AlbedoTexture, weights);
				float3 rockNormal = sampleLayerNormal(position, Rock_TextureScale, Rock_NormalTexture, Rock_BitangentCorrection, surfaceNormal, weights);
				float3 rockSurface = sampleLayerSurface(position, Rock_TextureScale, Rock_SurfaceTexture, Rock_SmoothnessCorrection, weights);

				float3 sandColor = sampleLayerColor(position, Sand_TextureScale, Sand_AlbedoTexture, weights);
				float3 sandNormal = sampleLayerNormal(position, Sand_TextureScale, Sand_NormalTexture, Sand_BitangentCorrection, surfaceNormal, weights);
				float3 sandSurface = sampleLayerSurface(position, Sand_TextureScale, Sand_SurfaceTexture, Sand_SmoothnessCorrection, weights);

				// TODO Use scales off average height in these calculations instead.
				float height = length(position);

				float alpha = saturate(0.5 + (height - Sand_MinimumHeight) / Sand_Distance);

				// TODO Other end.

				// TODO Both ends of the slopes too. Multiply to get final alpha.

				// TODO Incorporate water level toggle.

				// TODO Incorporate noise toggles?

				color = blendLayerColors(rockColor, sampleSurfaceHeight(rockSurface), sandColor, sampleSurfaceHeight(sandSurface), Sand_Depth, alpha);
				normal = blendLayerNormals(rockNormal, sampleSurfaceHeight(rockSurface), sandNormal, sampleSurfaceHeight(sandSurface), Sand_Depth, alpha);
				surface = blendLayerSurfaces(rockSurface, sampleSurfaceHeight(rockSurface), sandSurface, sampleSurfaceHeight(sandSurface), Sand_Depth, alpha);

				// The surface properties have conveniently been combined component-wise inside the
				// surface vector, so now we can retrieve them.
				float metallic = sampleSurfaceMetallic(surface);
				float smoothness = sampleSurfaceSmoothness(surface);

				// Set up the surface.
				output.Albedo = color;
				output.Metallic = metallic;
				output.Smoothness = smoothness;
				output.Normal = normal;
			}
		ENDCG
	}
	FallBack "Diffuse"
}
