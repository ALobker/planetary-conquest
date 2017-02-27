Shader "Planet/Surface" {
	Properties {
		[NoScaleOffset]
		GrassTexture("Grass Texture", 2D) = "white" {}
		GrassNormals("Grass Normals", 2D) = "white" {}
		GrassScale("Grass Scale", Float) = 1.0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 200
		
		CGPROGRAM
			#pragma surface surf Standard vertex:vert
			#pragma target 3.0

			sampler2D GrassTexture;
			sampler2D GrassNormals;
			float GrassScale;

			float WaterLevel;

			struct Input {
				float3 position;
				float3 normal;
				float4 tangent;
			};

			void vert(inout appdata_full data, out Input input) {
				UNITY_INITIALIZE_OUTPUT(Input, input);

				input.position = data.vertex.xyz;
				input.normal = data.normal.xyz;

				float3 bitangent = cross(data.normal.xyz, data.tangent.xyz);
				data.tangent = float4(cross(bitangent, data.normal.xyz), data.tangent.w);

				input.tangent = data.tangent;
			}
			
			void surf(Input input, inout SurfaceOutputStandard output) {
				float3 position = input.position;
				float3 normal = input.normal;

				float angleXY = saturate(dot(normal, float3(0.0f, 0.0f, 1.0)));
				float angleYZ = saturate(dot(normal, float3(1.0f, 0.0f, 0.0)));
				float angleZX = saturate(dot(normal, float3(0.0f, 1.0f, 0.0)));

				float angleYX = saturate(dot(normal, float3(0.0f, 0.0f, -1.0)));
				float angleZY = saturate(dot(normal, float3(-1.0f, 0.0f, 0.0)));
				float angleXZ = saturate(dot(normal, float3(0.0f, -1.0f, 0.0)));

				float angleSum = angleXY + angleYZ + angleZX + angleYX + angleZY + angleXZ;

				float weightXY = angleXY / angleSum;
				float weightYZ = angleYZ / angleSum;
				float weightZX = angleZX / angleSum;

				float weightYX = angleYX / angleSum;
				float weightZY = angleZY / angleSum;
				float weightXZ = angleXZ / angleSum;

				float3 grassCoordinates = position / GrassScale;

				fixed4 grassColor = 0.0;

				grassColor += tex2D(GrassTexture, grassCoordinates.xy) * (weightXY + weightYX);
				grassColor += tex2D(GrassTexture, grassCoordinates.yz) * (weightYZ + weightZY);
				grassColor += tex2D(GrassTexture, grassCoordinates.zx) * (weightZX + weightXZ);

				float3 grassNormal = 0.0;

				float3 grassNormalXY = UnpackNormal(tex2D(GrassNormals, grassCoordinates.xy));
				float3 grassNormalYZ = UnpackNormal(tex2D(GrassNormals, grassCoordinates.yz));
				float3 grassNormalZX = UnpackNormal(tex2D(GrassNormals, grassCoordinates.zx));

				float3 grassNormalYX = grassNormalXY * float3(1.0, 1.0, 1.0);
				float3 grassNormalZY = grassNormalYZ * float3(1.0, 1.0, 1.0);
				float3 grassNormalXZ = grassNormalZX * float3(1.0, 1.0, 1.0);
				
				grassNormalXY = grassNormalXY * weightXY + grassNormalYX * weightYX;
				grassNormalYZ = grassNormalYZ * weightYZ + grassNormalZY * weightZY;
				grassNormalZX = grassNormalZX * weightZX + grassNormalXZ * weightXZ;

				float4 tangent = input.tangent;
				float3 bitangent = cross(normal, tangent.xyz) * tangent.w;
				float3x3 objectSpaceToTangentSpace = float3x3(tangent.xyz, bitangent, normal);
				float3x3 tangentSpaceToObjectSpace = transpose(objectSpaceToTangentSpace);



				grassNormalXY = grassNormalXY;
				grassNormalYZ = float3(grassNormalYZ.z, grassNormalYZ.y, -grassNormalYZ.x);
				grassNormalZX = float3(grassNormalZX.x, grassNormalZX.z, -grassNormalZX.y);

				grassNormal += grassNormalXY;
				//grassNormal += grassNormalYZ;
				//grassNormal += grassNormalZX;

				normalize(grassNormal);

				output.Albedo = grassColor.rgb;
				//output.Metallic = 0.0;
				//output.Smoothness = grassColor.a;
				output.Normal = grassNormal;//mul(objectSpaceToTangentSpace, grassNormal);
			}
		ENDCG
	}
	FallBack "Diffuse"
}
