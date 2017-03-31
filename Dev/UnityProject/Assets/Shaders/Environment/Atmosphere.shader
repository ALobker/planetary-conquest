Shader "Planet/Atmosphere" {
	Properties {
		AirColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		Density("Density", Float) = 1.0
		ScaleHeight("Scale Height", Float) = 1.0
		MinimumTransparency("Minimum Transparency", Float) = 0.0
		MaximumTransparency("Maximum Transparency", Float) = 1.0
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
			#pragma surface surf Custom alpha vertex:vert
			#pragma target 3.0
			
			
			// Make use of the common functionality of the planet related shaders.
			#include "Planet.cginc"
			
			
			static const int NumberOfIntegrationIntervals = 16;
			
			sampler2D _CameraDepthTexture;
			
			float4 AirColor;
			float Density;
			float ScaleHeight;

			float MinimumTransparency;
			float MaximumTransparency;

			float SurfaceHeight;

			struct Input {
				float3 position;
				float3 viewDirection;

				// Provided by Unity.
				float4 screenPos;
			};

			void vert(inout appdata_full data, out Input input) {
				UNITY_INITIALIZE_OUTPUT(Input, input);

				input.position = data.vertex.xyz;
				input.viewDirection = normalize(ObjSpaceViewDir(data.vertex));
			}

			float calculateDensity(float angle, float midPointHeight) {
				// Height from polar coordinates: (R + h) / cos(theta) - R
				float height = (SurfaceHeight + midPointHeight) / cos(angle) - SurfaceHeight;

				// Atmospheric density from height: e ^ (-h / H)
				return exp(-height / ScaleHeight);
			}

			float integrate(float startAngle, float endAngle, float midPointHeight) {
				float density = 0.0;

				float angle = endAngle - startAngle;

				for(int n = 0; n < NumberOfIntegrationIntervals; n++) {
					float startAngleStep = startAngle + angle * n / NumberOfIntegrationIntervals;
					float endAngleStep = startAngle + angle * (n + 1) / NumberOfIntegrationIntervals;

					float startDensity = calculateDensity(startAngleStep, midPointHeight);
					float endDensity = calculateDensity(endAngleStep, midPointHeight);

					// Trapezoidal rule. The angle comes from a polar coordinate system, so it's just like a regular integration.
					float totalDensity = (endAngleStep - startAngleStep) * (endDensity + startDensity) / 2;

					density += totalDensity;
				}

				return density;
			}

			void surf(Input input, inout SurfaceOutput/*Standard*/ output) {
				float3 position = input.position;
				float3 viewDirection = input.viewDirection;

				// TODO opposite angle bla
				float distance = dot(viewDirection, position);

				// Invert sphere normals, integrate either intersecting line or depth depending on which one is less.
				// I think this should be able to use the same code path and calculations, it's just the distance that is different.
				// The polar coordinate system will have to run from the opposite direction, I reckon, and no half distance x 2 anymore.
				// This will make the atmosphere disappear on the other side of transparent water, but I reckon it won't be noticable.
				// Light integration should be the same in both cases too.
				// Look up Rayleigh scattering.
				// Maybe clouds? Could try involving white colored noise in the integration.

				float4 screenPosition = UNITY_PROJ_COORD(input.screenPos);
				float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, screenPosition).r);
				float fragmentDepth = LinearEyeDepth(screenPosition.z / screenPosition.w);
				float atmosphereDepth = abs(fragmentDepth - depth);

				// Let's see, we need to integrate over the distance here, but blabla angle blabla height?
				float3 lowestPoint = position - viewDirection * distance;
				float lowestHeight = length(lowestPoint);

				// Something about the lowest height. How's that work?
				// atmodist is essentially a capped version that is below surfaceheight. need to know min/max angle (or max/min actually)?
				// Gotta draw this out when I'm not tired I guess.

				// TODO Can be negative now bla
				float midPointHeight = lowestHeight - SurfaceHeight;

				float3 surfacePoint = position - viewDirection * atmosphereDepth;

				float theta = acos(lowestHeight / length(position));
				float beta = acos(lowestHeight / length(surfacePoint));

				// Distance is in object space but whatever for now. atmosphereDepth is in camera space? are distances the same?
				float angle = pick(theta, -beta, atmosphereDepth, distance * 2.0);

				float startAngle = -theta;
				float endAngle = angle;

				// TODO blaaa
				float density = integrate(startAngle, endAngle, midPointHeight);

				float atmosphere = density * Density;

				// TODO Eliminate banding bla
				float3 color = AirColor.rgb;
				float transparency = dither(saturate(atmosphere), screenPosition);

				output.Albedo = color;
				output.Alpha = clamp(transparency, MinimumTransparency, MaximumTransparency);
			}


			half4 LightingCustom(SurfaceOutput s, half3 viewDir, float atten) {
				float4 color;

				color.rgb = s.Albedo;
				color.a = s.Alpha;

				return color;
			}
		ENDCG
	}
	FallBack "Diffuse"
}
