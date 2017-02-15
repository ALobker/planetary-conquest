Shader "Planet/Surface" {
	Properties {
		[NoScaleOffset]
		GrassTexture("Grass", 2D) = "white" {}
		Scale("Scale", Float) = 1.0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 200
		
		CGPROGRAM
			#pragma surface surf Standard vertex:vert
			#pragma target 3.0

			sampler2D GrassTexture;
			
			float Scale;

			struct Input {
				float3 position;
				float3 normal;
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

				position /= Scale;

				fixed3 color = 0.0f;

				color += tex2D(GrassTexture, position.xy) * weightXY;
				color += tex2D(GrassTexture, position.yz) * weightYZ;
				color += tex2D(GrassTexture, position.zx) * weightZX;

				output.Albedo = color;
			}
		ENDCG
	}
	FallBack "Diffuse"
}
