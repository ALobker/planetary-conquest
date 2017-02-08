Shader "Planet/Water" {
	Properties {
		[NoScaleOffset]
		WaterTexture("Water", 2D) = "white" {}
		[NoScaleOffset]
		FoamTexture("Foam", 2D) = "white" {}
		WaterDepth("Water Depth", Float) = 1.0
		FoamDepth("Foam Depth", Float) = 1.0
		MinimumTransparency("Minimum Transparency", Float) = 0.0
		MaximumTransparency("Maximum Transparency", Float) = 1.0
		Scale("Scale", Float) = 1.0
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200
		
		CGPROGRAM
			#pragma surface surf Standard alpha
			#pragma target 3.0

			sampler2D _CameraDepthTexture;

			sampler2D WaterTexture;
			sampler2D FoamTexture;
			
			float WaterDepth;
			float FoamDepth;

			float MinimumTransparency;
			float MaximumTransparency;

			float Scale;

			struct Input {
				float3 worldPos;
				float3 worldNormal;
				float4 screenPos;
			};

			void surf(Input input, inout SurfaceOutputStandard output) {
				float3 worldPosition = input.worldPos;
				float3 worldNormal = input.worldNormal;

				float angleXY = abs(dot(worldNormal, float3(0.0f, 0.0f, 1.0)));
				float angleYZ = abs(dot(worldNormal, float3(1.0f, 0.0f, 0.0)));
				float angleZX = abs(dot(worldNormal, float3(0.0f, 1.0f, 0.0)));

				float angleSum = angleXY + angleYZ + angleZX;

				float weightXY = angleXY / angleSum;
				float weightYZ = angleYZ / angleSum;
				float weightZX = angleZX / angleSum;

				worldPosition /= Scale;

				fixed3 waterColor = 0.0f;

				waterColor += tex2D(WaterTexture, worldPosition.xy) * weightXY;
				waterColor += tex2D(WaterTexture, worldPosition.yz) * weightYZ;
				waterColor += tex2D(WaterTexture, worldPosition.zx) * weightZX;

				fixed3 foamColor = 0.0f;

				foamColor += tex2D(FoamTexture, worldPosition.xy) * weightXY;
				foamColor += tex2D(FoamTexture, worldPosition.yz) * weightYZ;
				foamColor += tex2D(FoamTexture, worldPosition.zx) * weightZX;

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
