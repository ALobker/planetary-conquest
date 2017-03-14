﻿Shader "Planet/Atmosphere" {
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
			#pragma surface surf Standard alpha vertex:vert
			#pragma target 3.0
			
			const int NumberOfIntegrationIntervals = 16;
			
			float4 AirColor;
			float Density;
			float ScaleHeight;

			float MinimumTransparency;
			float MaximumTransparency;

			float SurfaceHeight;

			struct Input {
				float3 position;
				float3 viewDirection;
			};

			void vert(inout appdata_full data, out Input input) {
				UNITY_INITIALIZE_OUTPUT(Input, input);

				input.position = data.vertex.xyz;
				input.viewDirection = normalize(ObjSpaceViewDir(data.vertex));
			}

			float calculateDensity(float midPointHeight, float angle) {
				// Height from polar coordinates: (R + h1) / cos(theta) - R
				float height = (SurfaceHeight + midPointHeight) / cos(angle) - SurfaceHeight;

				// Atmospheric density: e ^ (-h / H)
				return exp(-height / ScaleHeight);
			}

			float integrate(float distance, float midPointHeight) {
				float angle = atan2(distance, SurfaceHeight + midPointHeight);

				float density = 0.0;

				for(int n = 0; n < NumberOfIntegrationIntervals; n++) {
					float startAngle = angle * n / NumberOfIntegrationIntervals;
					float endAngle = angle * (n + 1) / NumberOfIntegrationIntervals;

					float startDensity = calculateDensity(midPointHeight, startAngle);
					float endDensity = calculateDensity(midPointHeight, endAngle);

					// Trapezoidal rule. The angle comes from a polar coordinate system, so it's just like a regular integration.
					float totalDensity = (endAngle - startAngle) * (endDensity + startDensity) / 2;

					density += totalDensity;
				}

				// Densities are symmetric around the midpoint.
				return density * 2.0;
			}

			void surf(Input input, inout SurfaceOutputStandard output) {
				float3 position = input.position;
				float3 viewDirection = input.viewDirection;

				float3 antiradial = -position;
				float halfDistance = dot(viewDirection, antiradial);
				float distance = 2.0 * halfDistance;

				float3 lowestPoint = position + viewDirection * halfDistance;
				float lowestHeight = length(lowestPoint);

				float altitude = lowestHeight - SurfaceHeight;

				float density = integrate(halfDistance, altitude);

				float atmosphere = density * Density;

				// Invert sphere normals, integrate either intersecting line or depth depending on which one is less.
				// I think this should be able to use the same code path and calculations, it's just the distance that is different.
				// The polar coordinate system will have to run from the opposite direction, I reckon, and no half distance x 2 anymore.
				// This will make the atmosphere disappear on the other side of transparent water, but I reckon it won't be noticable.
				// Light integration should be the same in both cases too.
				// Look up Rayleigh scatttering.
				// Maybe clouds? Could try involving white colored noise in the integration.

				float3 color = AirColor.rgb;
				float transparency = saturate(atmosphere);

				output.Albedo = color;
				output.Alpha = clamp(transparency, MinimumTransparency, MaximumTransparency);
			}
		ENDCG
	}
	FallBack "Diffuse"
}