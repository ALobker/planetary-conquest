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
			// TODO bla	for now we disable everything we don't need bla we can look at expanding this later (only forwardbase, forwardadd, and ambient are enabled right now)
			#pragma surface surf Atmosphere vertex:vert alpha exclude_path:deferred exclude_path:prepass noshadow novertexlights nolightmap nodynlightmap nodirlightmap nofog nometa
			
			// TODO bla about DX9
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
			float WaterHeight;

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

			float calculateDensity(float angle, float midPointHeight, float3 midPointPosition, float3 viewDirection) {
				// Height from polar coordinates: (R + h) / cos(theta) - R
				float height = (SurfaceHeight + midPointHeight) / cos(angle) - SurfaceHeight;

				float3 position = midPointPosition + viewDirection * (height + SurfaceHeight) * sin(angle);

				// TODO Cache?
				float4 lightPositionOrDirection = mul(unity_WorldToObject, _WorldSpaceLightPos0);
				float3 lightDirection = lerp(-lightPositionOrDirection.xyz, normalize(lightPositionOrDirection.xyz - position), lightPositionOrDirection.w);

				float3 lowestPoint = position + lightDirection * dot(-position, lightDirection);

				float differenceSign = sign(length(lowestPoint) - max(SurfaceHeight, WaterHeight));
				float differencePack = 0.5 + 0.5 * stretch(differenceSign);

				// Atmospheric density from height: e ^ (-h / H)
				return differencePack * exp(-height / ScaleHeight);
			}

			float integrate(float startAngle, float endAngle, float midPointHeight, float3 midPointPosition, float3 viewDirection) {
				float density = 0.0;

				float angle = endAngle - startAngle;

				for(int n = 0; n < NumberOfIntegrationIntervals; n++) {
					float startAngleStep = startAngle + angle * n / NumberOfIntegrationIntervals;
					float endAngleStep = startAngle + angle * (n + 1) / NumberOfIntegrationIntervals;

					float startDensity = calculateDensity(startAngleStep, midPointHeight, midPointPosition, viewDirection);
					float endDensity = calculateDensity(endAngleStep, midPointHeight, midPointPosition, viewDirection);

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

				float4 screenPosition = UNITY_PROJ_COORD(input.screenPos);
				float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, screenPosition).r);
				float fragmentDepth = LinearEyeDepth(screenPosition.z / screenPosition.w);
				float atmosphereDepth = abs(fragmentDepth - depth);

				float3 lowestPoint = position - viewDirection * distance;
				float lowestHeight = length(lowestPoint);

				// TODO Can be negative now bla
				float midPointHeight = lowestHeight - SurfaceHeight;

				// TODO Depth is in world space but whatever for now.
				float3 surfacePoint = position - viewDirection * atmosphereDepth;

				float theta = acos(lowestHeight / length(position));
				float beta = acos(lowestHeight / length(surfacePoint));
				float gamma = acos(lowestHeight / WaterHeight);

				float signum = -sign(dot(viewDirection, surfacePoint));

				// TODO bla smallest angle, sign for beta angle bla
				float angle = min(min(theta, signum * beta), -gamma);

				float startAngle = -theta;
				float endAngle = angle;

				// TODO blaaa
				float density = integrate(startAngle, endAngle, midPointHeight, lowestPoint, viewDirection);

				float atmosphere = density * Density;

				// TODO Eliminate banding bla
				float3 color = AirColor.rgb;
				float transparency = dither(saturate(atmosphere), screenPosition);

				output.Albedo = color;
				output.Alpha = clamp(transparency, MinimumTransparency, MaximumTransparency);
			}


			float4 LightingAtmosphere(SurfaceOutput s, half3 viewDir, float atten) {
				return float4(atten * s.Albedo, s.Alpha);
			}
		ENDCG
	}
	FallBack "Diffuse"
}
