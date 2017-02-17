Shader "Planet/Surface" {
	Properties {
		[NoScaleOffset]
		GrassTexture("Grass Texture", 2D) = "white" {}
		GrassScale("Grass Scale", Float) = 1.0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 200
		
		CGPROGRAM
			#pragma surface surf Standard vertex:vert
			#pragma target 3.0

			sampler2D GrassTexture;
			float GrassScale;

			float WaterLevel;

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

				float3 grassCoordinates = position / GrassScale;

				fixed3 grassColor = 0.0f;

				grassColor += tex2D(GrassTexture, grassCoordinates.xy) * weightXY;
				grassColor += tex2D(GrassTexture, grassCoordinates.yz) * weightYZ;
				grassColor += tex2D(GrassTexture, grassCoordinates.zx) * weightZX;

				output.Albedo = grassColor;
			}
		ENDCG
	}
	FallBack "Diffuse"
}
