/**
 * The shader for the surface of the planet.
 * 
 * The surface consists of four layers, each with properties that control how and where they appear.
 * 
 * Each layer's material properties are actually sampled over all three axes and then averaged. This
 * implements a 3D texture effect that allows application of the material properties to the entire
 * surface without the need for a mapping.
 * 
 * The layers are blended together using their respective height properties.
 * 
 * This is a surface shader, so lighting and any other advanced features are provided by Unity.
 * 
 * Since each layer has the same properties, it is easier to document them here:
 * 
 * MinimumHeight:        The lowest height the layer will be displayed on. This is a scale, and is
 *                       multiplied with either the surface height or the water level.
 * MaximumHeight:        The highest height the layer will be displayed on. This is a scale, and is
 *                       multiplied with either the surface height or the water level.
 * TransitionHeight:     The length of the transition intervals before the minimum height and after
 *                       the maximum height. This is a scale, and is multiplied with the radius of
 *                       the surface.
 * 
 * MinimumAngle:         The smallest angle the layer will be displayed for, in degrees.
 * MaximumAngle:         The largest angle the layer will be displayed for, in degrees.
 * TransitionAngle:      The length of the transition intervals before the minimum angle and after
 *                       the maximum angle, in degrees.
 * 
 * TransitionDepth:      How much of each layer's height is taken into account while calculating
 *                       the height-based blend. A larger depth produces fuzzier edges.
 * 
 * BaseOnWaterHeight:    Whether to multiply the scales by the water height instead of the surface
 *                       height. Can be either 0 (multiply by the surface height) or 1 (multiply by
 *                       the water height).
 * 
 * AlbedoTexture:        The colors of the layer.
 * NormalTexture:        The normals of the layer.
 * SurfaceTexture:       The metals of the layer (in the red channel), the smoothness of the layer
 *                       (in the green channel), and the height of the layer (in the blue channel).
 * TextureScale:         The amount of times the textures will be repeated (in object space).
 * 
 * BitangentCorrection:  Whether or not to correct (invert) the bitangent of the normal. Can be
 *                       either 0 (don't correct) or 1 (do correct).
 * SmoothnessCorrection: Whether or not to correct (invert) the smoothness. Can be either 0 (don't
 *                       correct) or 1 (do correct).
 */
Shader "Planet/Surface" {
	Properties {
		// Material properties for the rock layer. See above for their descriptions.
		[Header(Rock)]
		Rock_MinimumHeight("Minimum Height", Range(0.0, 2.0)) = 0.0
		Rock_MaximumHeight("Maximum Height", Range(0.0, 2.0)) = 2.0
		Rock_TransitionHeight("Transition Height", Range(0.001, 0.1)) = 0.001
		[Space]
		Rock_MinimumAngle("Minimum Angle", Range(0.0, 90.0)) = 0.0
		Rock_MaximumAngle("Maximum Angle", Range(0.0, 90.0)) = 90.0
		Rock_TransitionAngle("Transition Angle", Range(1.0, 90.0)) = 1.0
		[Space]
		Rock_TransitionDepth("Transition Depth", Range(0.01, 1.0)) = 1.0
		[Space]
		[ToggleOff] Rock_BaseOnWaterHeight("Base On Water Height", Float) = 0.0
		[Space]
		[NoScaleOffset] Rock_AlbedoTexture("Albedo", 2D) = "white" {}
		[NoScaleOffset] [Normal] Rock_NormalTexture("Normal", 2D) = "bump" {}
		[NoScaleOffset] Rock_SurfaceTexture("Metallic (R), Smoothness (G), and Height (B)", 2D) = "black" {}
		[PowerSlider(2.0)] Rock_TextureScale("Scale", Range(1.0, 100.0)) = 1.0
		[Space]
		[ToggleOff] Rock_BitangentCorrection("Invert Green Channel of Normal", Float) = 0.0
		[ToggleOff] Rock_SmoothnessCorrection("Invert Smoothness", Float) = 0.0

		// Material properties for the sand layer. See above for their descriptions.
		[Header(Sand)]
		Sand_MinimumHeight("Minimum Height", Range(0.0, 2.0)) = 0.0
		Sand_MaximumHeight("Maximum Height", Range(0.0, 2.0)) = 2.0
		Sand_TransitionHeight("Transition Height", Range(0.001, 0.1)) = 0.001
		[Space]
		Sand_MinimumAngle("Minimum Angle", Range(0.0, 90.0)) = 0.0
		Sand_MaximumAngle("Maximum Angle", Range(0.0, 90.0)) = 90.0
		Sand_TransitionAngle("Transition Angle", Range(1.0, 90.0)) = 1.0
		[Space]
		Sand_TransitionDepth("Transition Depth", Range(0.01, 1.0)) = 1.0
		[Space]
		[ToggleOff] Sand_BaseOnWaterHeight("Base On Water Height", Float) = 0.0
		[Space]
		[NoScaleOffset] Sand_AlbedoTexture("Albedo", 2D) = "white" {}
		[NoScaleOffset] [Normal] Sand_NormalTexture("Normal", 2D) = "bump" {}
		[NoScaleOffset] Sand_SurfaceTexture("Metallic (R), Smoothness (G), and Height (B)", 2D) = "black" {}
		[PowerSlider(2.0)] Sand_TextureScale("Scale", Range(1.0, 100.0)) = 1.0
		[Space]
		[ToggleOff] Sand_BitangentCorrection("Invert Green Channel of Normal", Float) = 0.0
		[ToggleOff] Sand_SmoothnessCorrection("Invert Smoothness", Float) = 0.0

		// Material properties for the grass layer. See above for their descriptions.
		[Header(Grass)]
		Grass_MinimumHeight("Minimum Height", Range(0.0, 2.0)) = 0.0
		Grass_MaximumHeight("Maximum Height", Range(0.0, 2.0)) = 2.0
		Grass_TransitionHeight("Transition Height", Range(0.001, 0.1)) = 0.001
		[Space]
		Grass_MinimumAngle("Minimum Angle", Range(0.0, 90.0)) = 0.0
		Grass_MaximumAngle("Maximum Angle", Range(0.0, 90.0)) = 90.0
		Grass_TransitionAngle("Transition Angle", Range(1.0, 90.0)) = 1.0
		[Space]
		Grass_TransitionDepth("Transition Depth", Range(0.01, 1.0)) = 1.0
		[Space]
		[ToggleOff] Grass_BaseOnWaterHeight("Base On Water Height", Float) = 0.0
		[Space]
		[NoScaleOffset] Grass_AlbedoTexture("Albedo", 2D) = "white" {}
		[NoScaleOffset] [Normal] Grass_NormalTexture("Normal", 2D) = "bump" {}
		[NoScaleOffset] Grass_SurfaceTexture("Metallic (R), Smoothness (G), and Height (B)", 2D) = "black" {}
		[PowerSlider(2.0)] Grass_TextureScale("Scale", Range(1.0, 100.0)) = 1.0
		[Space]
		[ToggleOff] Grass_BitangentCorrection("Invert Green Channel of Normal", Float) = 0.0
		[ToggleOff] Grass_SmoothnessCorrection("Invert Smoothness", Float) = 0.0

		// Material properties for the snow layer. See above for their descriptions.
		[Header(Snow)]
		Snow_MinimumHeight("Minimum Height", Range(0.0, 2.0)) = 0.0
		Snow_MaximumHeight("Maximum Height", Range(0.0, 2.0)) = 2.0
		Snow_TransitionHeight("Transition Height", Range(0.001, 0.1)) = 0.001
		[Space]
		Snow_MinimumAngle("Minimum Angle", Range(0.0, 90.0)) = 0.0
		Snow_MaximumAngle("Maximum Angle", Range(0.0, 90.0)) = 90.0
		Snow_TransitionAngle("Transition Angle", Range(1.0, 90.0)) = 1.0
		[Space]
		Snow_TransitionDepth("Transition Depth", Range(0.01, 1.0)) = 1.0
		[Space]
		[ToggleOff] Snow_BaseOnWaterHeight("Base On Water Height", Float) = 0.0
		[Space]
		[NoScaleOffset] Snow_AlbedoTexture("Albedo", 2D) = "white" {}
		[NoScaleOffset] [Normal] Snow_NormalTexture("Normal", 2D) = "bump" {}
		[NoScaleOffset] Snow_SurfaceTexture("Metallic (R), Smoothness (G), and Height (B)", 2D) = "black" {}
		[PowerSlider(2.0)] Snow_TextureScale("Scale", Range(1.0, 100.0)) = 1.0
		[Space]
		[ToggleOff] Snow_BitangentCorrection("Invert Green Channel of Normal", Float) = 0.0
		[ToggleOff] Snow_SmoothnessCorrection("Invert Smoothness", Float) = 0.0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 200
		
		CGPROGRAM
			// Use a standard surface shader with a custom vertex shader.
			#pragma surface surf Standard vertex:vert
			
			// We support DirectX 9.0c or higher, but exceed the limitations of Windows RT.
			#pragma target 3.0
			#pragma exclude_renderers d3d11_9x
			
			
			// Make use of the common functionality of the planet related shaders.
			#include "Planet.cginc"
			
			
			// Material properties for the rock layer. See above for their descriptions.
			float Rock_MinimumHeight;
			float Rock_MaximumHeight;
			float Rock_TransitionHeight;

			float Rock_MinimumAngle;
			float Rock_MaximumAngle;
			float Rock_TransitionAngle;

			float Rock_TransitionDepth;

			float Rock_BaseOnWaterHeight;
			
			sampler2D Rock_AlbedoTexture;
			sampler2D Rock_NormalTexture;
			sampler2D Rock_SurfaceTexture;
			float Rock_TextureScale;

			float Rock_BitangentCorrection;
			float Rock_SmoothnessCorrection;


			// Material properties for the sand layer. See above for their descriptions.
			float Sand_MinimumHeight;
			float Sand_MaximumHeight;
			float Sand_TransitionHeight;

			float Sand_MinimumAngle;
			float Sand_MaximumAngle;
			float Sand_TransitionAngle;

			float Sand_TransitionDepth;

			float Sand_BaseOnWaterHeight;

			sampler2D Sand_AlbedoTexture;
			sampler2D Sand_NormalTexture;
			sampler2D Sand_SurfaceTexture;
			float Sand_TextureScale;

			float Sand_BitangentCorrection;
			float Sand_SmoothnessCorrection;


			// Material properties for the grass layer. See above for their descriptions.
			float Grass_MinimumHeight;
			float Grass_MaximumHeight;
			float Grass_TransitionHeight;

			float Grass_MinimumAngle;
			float Grass_MaximumAngle;
			float Grass_TransitionAngle;

			float Grass_TransitionDepth;
			
			float Grass_BaseOnWaterHeight;

			sampler2D Grass_AlbedoTexture;
			sampler2D Grass_NormalTexture;
			sampler2D Grass_SurfaceTexture;
			float Grass_TextureScale;

			float Grass_BitangentCorrection;
			float Grass_SmoothnessCorrection;


			// Material properties for the snow layer. See above for their descriptions.
			float Snow_MinimumHeight;
			float Snow_MaximumHeight;
			float Snow_TransitionHeight;

			float Snow_MinimumAngle;
			float Snow_MaximumAngle;
			float Snow_TransitionAngle;

			float Snow_TransitionDepth;

			float Snow_BaseOnWaterHeight;

			sampler2D Snow_AlbedoTexture;
			sampler2D Snow_NormalTexture;
			sampler2D Snow_SurfaceTexture;
			float Snow_TextureScale;

			float Snow_BitangentCorrection;
			float Snow_SmoothnessCorrection;


			/**
			 * The original radius of the planet.
			 * 
			 * This is used in calculations pertaining scales invariant to the height of the surface.
			 */
			float SurfaceRadius;

			/**
			 * The average distance between the surface of the planet and the origin of the planet.
			 * 
			 * This is used in calculations pertaining scales related to the height of the surface.
			 */
			float SurfaceHeight;

			/**
			 * The average distance between the surface of the water and the origin of the planet.
			 * 
			 * This is used in calculations pertaining scales related to the height of the water.
			 */
			float WaterHeight;


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
				// Normalize the properties interpolated to the surface shader in object space.
				float3 position = input.position / SurfaceRadius;
				float3 surfaceNormal = normalize(input.surfaceNormal);

				// Get a measure of how much each cardinal axis contributes to the surface normal.
				float3 weights = calculateWeights(surfaceNormal);

				// Sample the color, normal, and surface properties of each layer.
				float3 rockColor = sampleLayerColor(position, Rock_TextureScale, Rock_AlbedoTexture, weights);
				float3 rockNormal = sampleLayerNormal(position, Rock_TextureScale, Rock_NormalTexture, Rock_BitangentCorrection, surfaceNormal, weights);
				float3 rockSurface = sampleLayerSurface(position, Rock_TextureScale, Rock_SurfaceTexture, Rock_SmoothnessCorrection, weights);

				float3 sandColor = sampleLayerColor(position, Sand_TextureScale, Sand_AlbedoTexture, weights);
				float3 sandNormal = sampleLayerNormal(position, Sand_TextureScale, Sand_NormalTexture, Sand_BitangentCorrection, surfaceNormal, weights);
				float3 sandSurface = sampleLayerSurface(position, Sand_TextureScale, Sand_SurfaceTexture, Sand_SmoothnessCorrection, weights);

				float3 grassColor = sampleLayerColor(position, Grass_TextureScale, Grass_AlbedoTexture, weights);
				float3 grassNormal = sampleLayerNormal(position, Grass_TextureScale, Grass_NormalTexture, Grass_BitangentCorrection, surfaceNormal, weights);
				float3 grassSurface = sampleLayerSurface(position, Grass_TextureScale, Grass_SurfaceTexture, Grass_SmoothnessCorrection, weights);

				float3 snowColor = sampleLayerColor(position, Snow_TextureScale, Snow_AlbedoTexture, weights);
				float3 snowNormal = sampleLayerNormal(position, Snow_TextureScale, Snow_NormalTexture, Snow_BitangentCorrection, surfaceNormal, weights);
				float3 snowSurface = sampleLayerSurface(position, Snow_TextureScale, Grass_SurfaceTexture, Snow_SmoothnessCorrection, weights);

				// Initialize the base layer with some default color, normal, and surface properties.
				// This should be entirely overridden by the rock layer if everything is set up right,
				// but just in case it's not, you get a simple grey surface. It also keeps the rock
				// layer symmetric with the other layers in terms of properties and code paths.
				float3 color = float3(0.5, 0.5, 0.5);
				float3 normal = surfaceNormal;
				float3 surface = float3(0.0, 0.0, 0.5);

				// Iteratively blend the color, normal, and surface properties of each layer into
				// the base layer. The blend weight is based on the height and slope of the surface,
				// while the actual blend is based on the heights of the two blended layers.
				float rockBaseHeight = pick(SurfaceHeight, WaterHeight, 0.5, Rock_BaseOnWaterHeight);
				float rockBlend = calculateLayerBlend(position, surfaceNormal, rockBaseHeight, Rock_MinimumHeight, Rock_MaximumHeight, Rock_TransitionHeight, Rock_MinimumAngle, Rock_MaximumAngle, Rock_TransitionAngle, SurfaceRadius);
				blendLayers(color, normal, surface, rockColor, rockNormal, rockSurface, rockBlend, Rock_TransitionDepth);

				float sandBaseHeight = pick(SurfaceHeight, WaterHeight, 0.5, Sand_BaseOnWaterHeight);
				float sandBlend = calculateLayerBlend(position, surfaceNormal, sandBaseHeight, Sand_MinimumHeight, Sand_MaximumHeight, Sand_TransitionHeight, Sand_MinimumAngle, Sand_MaximumAngle, Sand_TransitionAngle, SurfaceRadius);
				blendLayers(color, normal, surface, sandColor, sandNormal, sandSurface, sandBlend, Sand_TransitionDepth);

				float grassBaseHeight = pick(SurfaceHeight, WaterHeight, 0.5, Grass_BaseOnWaterHeight);
				float grassBlend = calculateLayerBlend(position, surfaceNormal, grassBaseHeight, Grass_MinimumHeight, Grass_MaximumHeight, Grass_TransitionHeight, Grass_MinimumAngle, Grass_MaximumAngle, Grass_TransitionAngle, SurfaceRadius);
				blendLayers(color, normal, surface, grassColor, grassNormal, grassSurface, grassBlend, Grass_TransitionDepth);

				float snowBaseHeight = pick(SurfaceHeight, WaterHeight, 0.5, Snow_BaseOnWaterHeight);
				float snowBlend = calculateLayerBlend(position, surfaceNormal, snowBaseHeight, Snow_MinimumHeight, Snow_MaximumHeight, Snow_TransitionHeight, Snow_MinimumAngle, Snow_MaximumAngle, Snow_TransitionAngle, SurfaceRadius);
				blendLayers(color, normal, surface, snowColor, snowNormal, snowSurface, snowBlend, Snow_TransitionDepth);

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
