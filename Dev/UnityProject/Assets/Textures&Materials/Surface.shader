Shader "Planet/Surface" {
	Properties {
		[NoScaleOffset]
		Texture("Texture", 2D) = "white" {}
		Scale("Scale", Float) = 1.0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 200
		
		CGPROGRAM
			#pragma surface surf Standard
			#pragma target 3.0

			sampler2D Texture;
			
			float Scale;

			struct Input {
				float3 worldPos;
				float3 worldNormal;
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

				fixed3 color = 0.0f;

				color += tex2D(Texture, worldPosition.xy) * weightXY;
				color += tex2D(Texture, worldPosition.yz) * weightYZ;
				color += tex2D(Texture, worldPosition.zx) * weightZX;

				output.Albedo = color;
			}
		ENDCG
	}
	FallBack "Diffuse"
}
