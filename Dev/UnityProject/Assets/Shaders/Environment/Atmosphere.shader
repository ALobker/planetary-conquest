/**
 * The shader for the atmosphere of the planet.
 * 
 * The inside of the atmosphere is visualized on its outer boundary by integrating its density
 * along all lines of sight intersecting the outer boundary. This achieves a volumetric effect,
 * but means the atmosphere is only visible from outside the atmosphere.
 * 
 * The densities are integrated with a numerical approximation in polar coordinates.
 * 
 * The atmosphere is considered to be "superdiffuse", that is, every sample reflects light in all
 * directions equally. This is implemented in a very simple custom lighting model that only takes
 * into account attenuation. Only important directional and point lights are considered, which means
 * every light is applied in a separate pass. Non-important lights do not affect the atmosphere.
 * 
 * An approximate occlusion by the surface or water is used to implement a shadow (one per pass).
 * The shadow is applied in the integration step, by considering the sample to have no density.
 * 
 * This is a surface shader, so lighting and any other advanced features are provided by Unity.
 * However, a number of them have been disabled because of our custom lighting implementation.
 */
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
			// Use a surface shader with a custom lighting model and vertex shader. Only ambient,
			// point, and directional lighting is enabled. More advanced lighting features like
			// deferred lighting, baked lighting, spherical harmonics, and shadowing are disabled.
			#pragma surface surf Atmosphere vertex:vert alpha exclude_path:deferred exclude_path:prepass noshadow novertexlights nolightmap nodynlightmap nodirlightmap nofog nometa

			// We support DirectX 9.0c or higher, but exceed the limitations of Windows RT.
			#pragma target 3.0
			#pragma exclude_renderers d3d11_9x
			
			
			// Make use of the common functionality of the planet related shaders.
			#include "Planet.cginc"
			
			
			/**
			 * Enables the depth texture provided by Unity.
			 */
			sampler2D _CameraDepthTexture;
			
			
			/**
			 * The number of intervals used to numerically approximate the integration of the
			 * densities along a line of sight.
			 */
			int NumberOfIntegrationIntervals;
			

			float4 AirColor;
			float Density;
			float ScaleHeight;

			float MinimumTransparency;
			float MaximumTransparency;

			float SurfaceHeight;
			float WaterHeight;


			/**
			* The properties to interpolate to the surface shader (in object space).
			* 
			* Unity automatically provides the screen position through the "screenPos" member.
			*/
			struct Input {
				float3 position;
				float3 viewDirection;

				float4 screenPos;
			};


			/**
			* The vertex shader.
			*/
			void vert(inout appdata_full data, out Input input) {
				UNITY_INITIALIZE_OUTPUT(Input, input);

				// Interpolate the position and view direction to the surface shader in object space.
				input.position = data.vertex.xyz;
				input.viewDirection = normalize(ObjSpaceViewDir(data.vertex));
			}


			// TODO move to include
			float calculateDensity(float angle, float midPointHeight, float3 midPointPosition, float3 viewDirection) {
				// Height from polar coordinates: (R + h) / cos(theta) - R
				float height = (SurfaceHeight + midPointHeight) / cos(angle) - SurfaceHeight;

				float3 position = midPointPosition - viewDirection * (height + SurfaceHeight) * sin(angle);

				// TODO Cache?
				float4 lightPositionOrDirection = mul(unity_WorldToObject, _WorldSpaceLightPos0);
				float3 lightDirection = lerp(-lightPositionOrDirection.xyz, normalize(lightPositionOrDirection.xyz - position), lightPositionOrDirection.w);

				float3 lowestPoint = position - lightDirection * dot(position, lightDirection);

				// TODO scale
				float heightFalloff = 50.0;
				float a = saturate((length(lowestPoint) - max(SurfaceHeight, WaterHeight)) / heightFalloff);

				// TODO scale, 10.0 is hardcoded.
				// TODO figure out why the dot product is the wrong way around
				float distanceFalloff = 100.0 * (exp(length(lowestPoint) / max(SurfaceHeight, WaterHeight) * 10.0) - 1.0) / (exp(1.0 * 10.0) - 1.0);
				float b = saturate(dot(position, lightDirection) / distanceFalloff);

				// TODO lerp between light side and dark side bla
				float luminosity = lerp(1.0, a, b);
				
				// Atmospheric density from height: e ^ (-h / H)
				return luminosity * exp(-height / ScaleHeight);
			}

			// TODO move to include
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


			/**
			* The surface shader.
			*/
			void surf(Input input, inout SurfaceOutput output) {
				// Retrieve the properties interpolated to the surface shader in object space.
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
				
				// Set up the surface.
				output.Albedo = color;
				output.Alpha = clamp(transparency, MinimumTransparency, MaximumTransparency);
			}


			/**
			 * The lighting model.
			 */
			fixed4 LightingAtmosphere(SurfaceOutput s, half3 viewDir, half atten) {
				// Since the lighting is actually done during the integration of the densities
				// along a line of sight (only one light is applied per pass), we just use the
				// value provided by the surface shader. Only the attenuation is applied. Since
				// the attenuation is provided only on the outer boundary, this will not result
				// in an entirely correct value, but it should be a good enough approximation.
				return fixed4(atten * s.Albedo, s.Alpha);
			}
		ENDCG
	}
	FallBack "Diffuse"
}
