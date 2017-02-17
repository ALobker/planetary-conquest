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
			#pragma surface surf Standard alpha vertex:vert
			#pragma target 3.0
			
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

			void surf(Input input, inout SurfaceOutputStandard output) {
				float3 position = input.position;
				float3 viewDirection = input.viewDirection;

				float3 antiradial = -position;
				float halfDistance = dot(viewDirection, antiradial);
				float distance = 2.0 * halfDistance;

				float3 lowestPoint = position + viewDirection * halfDistance;
				float lowestHeight = length(lowestPoint);

				float altitude = lowestHeight - SurfaceHeight;



				float density = exp(-altitude / ScaleHeight);

				float atmosphere = distance * Density * density;



				float3 color = AirColor.rgb;
				float transparency = saturate(atmosphere);

				output.Albedo = color;
				output.Alpha = clamp(transparency, MinimumTransparency, MaximumTransparency);
			}
		ENDCG
	}
	FallBack "Diffuse"
}
