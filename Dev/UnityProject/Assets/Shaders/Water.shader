Shader "Planet/Water" {
	Properties {
		[NoScaleOffset]
		WaterTexture("Water Texture", 2D) = "white" {}
		WaterScale("Water Scale", Float) = 1.0
		[NoScaleOffset]
		FoamTexture("Foam Texture", 2D) = "white" {}
		FoamScale("Foam Scale", Float) = 1.0
		WaterDepth("Water Depth", Float) = 1.0
		FoamDepth("Foam Depth", Float) = 1.0
		MinimumTransparency("Minimum Transparency", Float) = 0.0
		MaximumTransparency("Maximum Transparency", Float) = 1.0
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
			#pragma surface surf Standard alpha vertex:vert
			#pragma target 3.0

			sampler2D _CameraDepthTexture;
			
			sampler2D WaterTexture;
			float WaterScale;

			sampler2D FoamTexture;
			float FoamScale;
			
			float WaterDepth;
			float FoamDepth;

			float MinimumTransparency;
			float MaximumTransparency;

			struct Input {
				float3 position;
				float3 normal;

				// Provided by Unity.
				float4 screenPos;
			};

			void vert(inout appdata_full data, out Input input) {
				UNITY_INITIALIZE_OUTPUT(Input, input);

				input.position = data.vertex.xyz;
				input.normal = data.normal.xyz;
			}

			void surf(Input input, inout SurfaceOutputStandard output) {
				float3 position = input.position;
				float3 normal = input.normal;

				float angleXY = abs(dot(normal, float3(0.0f, 0.0f, 1.0)));
				float angleYZ = abs(dot(normal, float3(1.0f, 0.0f, 0.0)));
				float angleZX = abs(dot(normal, float3(0.0f, 1.0f, 0.0)));

				float angleSum = angleXY + angleYZ + angleZX;

				float weightXY = angleXY / angleSum;
				float weightYZ = angleYZ / angleSum;
				float weightZX = angleZX / angleSum;

				float3 waterCoordinates = position / WaterScale;

				fixed3 waterColor = 0.0f;

				waterColor += tex2D(WaterTexture, waterCoordinates.xy) * weightXY;
				waterColor += tex2D(WaterTexture, waterCoordinates.yz) * weightYZ;
				waterColor += tex2D(WaterTexture, waterCoordinates.zx) * weightZX;

				float3 foamCoordinates = position / FoamScale;

				fixed3 foamColor = 0.0f;

				foamColor += tex2D(FoamTexture, foamCoordinates.xy) * weightXY;
				foamColor += tex2D(FoamTexture, foamCoordinates.yz) * weightYZ;
				foamColor += tex2D(FoamTexture, foamCoordinates.zx) * weightZX;

				float4 screenPosition = UNITY_PROJ_COORD(input.screenPos);
				float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, screenPosition).r);
				float fragmentDepth = LinearEyeDepth(screenPosition.z / screenPosition.w);
				float waterDepth = abs(fragmentDepth - depth);

				float water = saturate(waterDepth / WaterDepth);
				float foam = 1.0 - saturate(waterDepth / FoamDepth);

				float3 color = lerp(waterColor, foamColor, foam);
				float transparency = lerp(water, 1.0, foam);

				output.Albedo = color;
				output.Alpha = clamp(transparency, MinimumTransparency, MaximumTransparency);
			}
		ENDCG
	}
	FallBack "Diffuse"
}
