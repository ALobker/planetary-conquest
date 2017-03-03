Shader "Planet/Surface" {
	Properties {
		[NoScaleOffset]
		GrassTexture("Grass Texture", 2D) = "white" {}
		[NoScaleOffset]
		GrassNormals("Grass Normals", 2D) = "white" {}
		GrassScale("Grass Scale", Float) = 1.0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 200
		
		CGPROGRAM
			#pragma surface surf Standard vertex:vert
			#pragma target 3.0

			#define EPSILON 1e-3

			#define POSITIVE_X_AXIS float3(1.0, 0.0, 0.0)
			#define POSITIVE_Y_AXIS float3(0.0, 1.0, 0.0)
			#define POSITIVE_Z_AXIS float3(0.0, 0.0, 1.0)

			#define NEGATIVE_X_AXIS float3(-1.0, 0.0, 0.0)
			#define NEGATIVE_Y_AXIS float3(0.0, -1.0, 0.0)
			#define NEGATIVE_Z_AXIS float3(0.0, 0.0, -1.0)

			sampler2D GrassTexture;
			sampler2D GrassNormals;
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

				// Since we work with object space normals, replace the tangent space in such a way
				// that the transformation from object space is the identity transformation. Then we
				// won't have to bother with tangent space transformations in the surface shader.
				data.tangent = float4(POSITIVE_X_AXIS, 1);
				data.normal = float4(POSITIVE_Z_AXIS, 0);
			}
			
			void surf(Input input, inout SurfaceOutputStandard output) {
				float3 position = input.position;
				float3 normal = input.normal;

				float angleXY = saturate(dot(normal, float3(0.0, 0.0, 1.0)));
				float angleYZ = saturate(dot(normal, float3(1.0, 0.0, 0.0)));
				float angleZX = saturate(dot(normal, float3(0.0, 1.0, 0.0)));

				//float angleYX = saturate(dot(normal, float3(0.0, 0.0, -1.0)));
				//float angleZY = saturate(dot(normal, float3(-1.0, 0.0, 0.0)));
				//float angleXZ = saturate(dot(normal, float3(0.0, -1.0, 0.0)));

				float angleSum = angleXY + angleYZ + angleZX;// + angleYX + angleZY + angleXZ;

				float weightXY = angleXY / angleSum;
				float weightYZ = angleYZ / angleSum;
				float weightZX = angleZX / angleSum;

				//float weightYX = angleYX / angleSum;
				//float weightZY = angleZY / angleSum;
				//float weightXZ = angleXZ / angleSum;

				float3 grassCoordinates = position / GrassScale;

				//fixed4 grassColor = 0.0;

				//grassColor += tex2D(GrassTexture, grassCoordinates.xy) * (weightXY + weightYX);
				//grassColor += tex2D(GrassTexture, grassCoordinates.yz) * (weightYZ + weightZY);
				//grassColor += tex2D(GrassTexture, grassCoordinates.zx) * (weightZX + weightXZ);

				float3x3 objectSpaceToTangentSpaceXY = float3x3(POSITIVE_X_AXIS, POSITIVE_Y_AXIS, POSITIVE_Z_AXIS);
				float3x3 tangentSpaceToObjectSpaceXY = transpose(objectSpaceToTangentSpaceXY);

				float3x3 objectSpaceToTangentSpaceYZ = float3x3(POSITIVE_Y_AXIS, POSITIVE_Z_AXIS, POSITIVE_X_AXIS);
				float3x3 tangentSpaceToObjectSpaceYZ = transpose(objectSpaceToTangentSpaceYZ);

				float3x3 objectSpaceToTangentSpaceZX = float3x3(POSITIVE_Z_AXIS, POSITIVE_X_AXIS, POSITIVE_Y_AXIS);
				float3x3 tangentSpaceToObjectSpaceZX = transpose(objectSpaceToTangentSpaceZX);

				float3 grassNormalInTangentSpaceXY = UnpackNormal(tex2D(GrassNormals, grassCoordinates.xy));
				float3 grassNormalInTangentSpaceYZ = UnpackNormal(tex2D(GrassNormals, grassCoordinates.yz));
				float3 grassNormalInTangentSpaceZX = UnpackNormal(tex2D(GrassNormals, grassCoordinates.zx));

				float3 grassNormalInObjectSpaceXY = mul(tangentSpaceToObjectSpaceXY, grassNormalInTangentSpaceXY);
				float3 grassNormalInObjectSpaceYZ = mul(tangentSpaceToObjectSpaceYZ, grassNormalInTangentSpaceYZ);
				float3 grassNormalInObjectSpaceZX = mul(tangentSpaceToObjectSpaceZX, grassNormalInTangentSpaceZX);

				float3 grassNormal = 0.0;

				// Take the weighted average of the three sampled normals in object space.
				grassNormal += grassNormalInObjectSpaceXY * weightXY;
				grassNormal += grassNormalInObjectSpaceYZ * weightYZ;
				grassNormal += grassNormalInObjectSpaceZX * weightZX;

				// If the weighted average is (near) the zero vector we can use the surface normal
				// instead. This is analogous to taking a weighted average over the unit sphere,
				// because the exact centroid would then be (near) the surface normal.
				grassNormal = lerp(grassNormal, normal, step(length(grassNormal), EPSILON));

				// Now we can safely make sure the normal has the unit length.
				grassNormal = normalize(grassNormal);
				
				// If the weighted average produces a normal that points inwards, we can use the
				// inversion instead. This is analogous to taking a weighted average over the long
				// angles of a unit sphere. This means it always produces a weighted average in the
				// hemisphere centered on the surface normal (i.e. above the surface).
				grassNormal = (step(0.0, dot(grassNormal, normal)) * 2.0 - 1.0) * grassNormal;

				output.Albedo = 0.5;//grassColor.rgb;
				//output.Metallic = 0.0;
				//output.Smoothness = grassColor.a;
				output.Normal = grassNormal;

				if (position.x < 0 || position.y < 0 || position.z < 0) {
					output.Albedo = 0;
				}
			}
		ENDCG
	}
	FallBack "Diffuse"
}
