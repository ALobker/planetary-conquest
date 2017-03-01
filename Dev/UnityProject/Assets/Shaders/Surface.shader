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
			#pragma surface surf Bla vertex:vert
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

			fixed4 LightingBla(SurfaceOutput s, float3 lightDir, float atten) {
				return fixed4(s.Albedo, s.Alpha);
			}

			void vert(inout appdata_full data, out Input input) {
				UNITY_INITIALIZE_OUTPUT(Input, input);

				input.position = data.vertex.xyz;
				input.normal = data.normal.xyz;

				float3 bitangent = cross(data.normal.xyz, data.tangent.xyz);
				data.tangent = float4(cross(bitangent, data.normal.xyz), data.tangent.w);

				input.tangent = data.tangent;
			}
			
			void surf(Input input, inout SurfaceOutput/*Standard*/ output) {
				float3 position = input.position;
				float3 normal = input.normal;

				//float angleXY = saturate(dot(normal, float3(0.0, 0.0, 1.0)));
				//float angleYZ = saturate(dot(normal, float3(1.0, 0.0, 0.0)));
				//float angleZX = saturate(dot(normal, float3(0.0, 1.0, 0.0)));

				//float angleYX = saturate(dot(normal, float3(0.0, 0.0, -1.0)));
				//float angleZY = saturate(dot(normal, float3(-1.0, 0.0, 0.0)));
				//float angleXZ = saturate(dot(normal, float3(0.0, -1.0, 0.0)));

				//float angleSum = angleXY + angleYZ + angleZX;// + angleYX + angleZY + angleXZ;

				//float weightXY = angleXY / angleSum;
				//float weightYZ = angleYZ / angleSum;
				//float weightZX = angleZX / angleSum;

				//float weightYX = angleYX / angleSum;
				//float weightZY = angleZY / angleSum;
				//float weightXZ = angleXZ / angleSum;

				float3 grassCoordinates = position / GrassScale;

				//fixed4 grassColor = 0.0;

				//grassColor += tex2D(GrassTexture, grassCoordinates.xy) * (weightXY + weightYX);
				//grassColor += tex2D(GrassTexture, grassCoordinates.yz) * (weightYZ + weightZY);
				//grassColor += tex2D(GrassTexture, grassCoordinates.zx) * (weightZX + weightXZ);

				float3 grassNormal = 0.0;

				//float3 grassNormalXY = UnpackNormal(tex2D(GrassNormals, grassCoordinates.xy));
				//float3 grassNormalYZ = UnpackNormal(tex2D(GrassNormals, grassCoordinates.yz));
				float3 grassNormalZX = UnpackNormal(tex2D(GrassNormals, grassCoordinates.xz));

				//float3 grassNormalYX = grassNormalXY * float3(1.0, 1.0, 1.0);
				//float3 grassNormalZY = grassNormalYZ * float3(1.0, 1.0, 1.0);
				//float3 grassNormalXZ = grassNormalZX * float3(1.0, 1.0, 1.0);
				
				//grassNormalXY = grassNormalXY + grassNormalYX * weightYX;
				//grassNormalYZ = grassNormalYZ * weightYZ + grassNormalZY * weightZY;
				//grassNormalZX = grassNormalZX * weightZX + grassNormalXZ * weightXZ;

				float4 tangent = input.tangent;
				float3 bitangent = cross(normal, tangent.xyz) * tangent.w;
				float3x3 objectSpaceToTangentSpace = float3x3(tangent.xyz, bitangent, normal);
				float3x3 tangentSpaceToObjectSpace = transpose(objectSpaceToTangentSpace);

				float3 xAxis = float3(1.0, 0.0, 0.0);
				float3 yAxis = float3(0.0, 1.0, 0.0);
				float3 zAxis = float3(0.0, 0.0, 1.0);

				//float3x3 objectSpaceToTangentSpaceXY = float3x3(xAxis, yAxis, zAxis);
				//float3x3 tangentSpaceToObjectSpaceXY = transpose(objectSpaceToTangentSpaceXY);

				//float3x3 objectSpaceToTangentSpaceYZ = float3x3(yAxis, zAxis, xAxis);
				//float3x3 tangentSpaceToObjectSpaceYZ = transpose(objectSpaceToTangentSpaceYZ);

				float3x3 objectSpaceToTangentSpaceZX = float3x3(xAxis, zAxis, yAxis);
				float3x3 tangentSpaceToObjectSpaceZX = transpose(objectSpaceToTangentSpaceZX);

				//float3 grassNormalInObjectSpaceXY = mul(grassNormalXY, tangentSpaceToObjectSpaceXY);
				//float3 grassNormalInObjectSpaceYZ = mul(grassNormalYZ, tangentSpaceToObjectSpaceYZ);
				float3 grassNormalInObjectSpaceZX = mul(grassNormalZX, tangentSpaceToObjectSpaceZX);

				//grassNormal += grassNormalInObjectSpaceXY * weightXY;
				//grassNormal += grassNormalInObjectSpaceYZ * weightYZ;
				grassNormal += grassNormalInObjectSpaceZX;// *weightZX;

				float3 grassNormalInTangentSpace = mul(grassNormal, objectSpaceToTangentSpace);

				output.Albedo = 1;

				if(grassNormalInTangentSpace.z < 0) {
					output.Albedo = float3(-grassNormalInTangentSpace.z, 0, 0);
				}
				else if(grassNormalInTangentSpace.z > 0) {
					output.Albedo = float3(0, 0, grassNormalInTangentSpace.z);
				}
				else {
					output.Albedo = float3(0, 1, 0);
				}

				//if(length(grassNormalInTangentSpace) < 0.001) {
				//	grassNormalInTangentSpace = zAxis;
				//}
				//else if(grassNormalInTangentSpace.z < 0) {
				//	grassNormalInTangentSpace = zAxis;//-grassNormalInTangentSpace;
				//}

				//grassNormalXY = grassNormalXY;
				//grassNormalYZ = float3(grassNormalYZ.z, grassNormalYZ.y, -grassNormalYZ.x);
				//grassNormalZX = float3(grassNormalZX.x, grassNormalZX.z, -grassNormalZX.y);

				//grassNormal += grassNormalXY;
				//grassNormal += grassNormalYZ;
				//grassNormal += grassNormalZX;

				//normalize(grassNormalInTangentSpace);

				//output.Albedo = grassColor.rgb;
				//output.Metallic = 0.0;
				//output.Smoothness = 0.0;// grassColor.a;
				output.Normal = zAxis;//grassNormalInTangentSpace;

				if (position.x < 0 || position.y < 0 || position.z < 0) {
					output.Albedo = 0;
				}
			}
		ENDCG
	}
	FallBack "Diffuse"
}
