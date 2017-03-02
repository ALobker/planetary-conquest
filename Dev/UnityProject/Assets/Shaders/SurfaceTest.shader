Shader "Planet/SurfaceTest" {
	Properties{
		[NoScaleOffset]
	GrassTexture("Grass Texture", 2D) = "white" {}
	GrassNormals("Grass Normals", 2D) = "white" {}
	GrassScale("Grass Scale", Float) = 1.0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200


		// ------------------------------------------------------------
		// Surface shader code generated out of a CGPROGRAM block:


		// ---- forward rendering base pass:
		Pass{
		Name "FORWARD"
		Tags{ "LightMode" = "ForwardBase" }

		CGPROGRAM
		// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma target 3.0
#pragma multi_compile_fog
#pragma multi_compile_fwdbase
#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
		// Surface shader code generated based on:
		// vertex modifier: 'vert'
		// writes to per-pixel normal: YES
		// writes to emission: no
		// writes to occlusion: no
		// needs world space reflection vector: no
		// needs world space normal vector: no
		// needs screen space position: no
		// needs world space position: no
		// needs view direction: no
		// needs world space view direction: no
		// needs world space position for lighting: YES
		// needs world space view direction for lighting: YES
		// needs world space view direction for lightmaps: no
		// needs vertex color: no
		// needs VFACE: no
		// passes tangent-to-world matrix to pixel shader: YES
		// reads from normal: no
		// 0 texcoords actually used
#define UNITY_PASS_FORWARDBASE
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

#define _SPECULARHIGHLIGHTS_OFF

		// Original surface shader snippet:
#line 10 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
		/* UNITY: Original start of shader */
		//#pragma surface surf Standard vertex:vert
		//#pragma target 3.0

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
		//input.normal = data.normal.xyz;

		//float3 bitangent = cross(data.normal.xyz, data.tangent.xyz);
		//data.tangent = float4(cross(bitangent, data.normal.xyz), data.tangent.w);

		float3 tangent = data.tangent.xyz;
		
		float3 normal = normalize(data.normal.xyz);

		// ortho-normalize Tangent
		tangent = normalize(tangent - normal * dot(tangent, normal));

		// recalculate Binormal
		half3 newB = cross(normal, tangent);
		float3 binormal = newB * sign(dot(newB, binormal));

		data.tangent = float4(tangent, 1);
		data.normal = float4(normal, 0);

		float3 xAxis = float3(1.0, 0.0, 0.0);
		float3 yAxis = float3(0.0, 1.0, 0.0);
		float3 zAxis = float3(0.0, 0.0, 1.0);

		//data.tangent = float4(xAxis, 1);
		//data.normal = float4(zAxis, 0);

		input.tangent = data.tangent;
		input.normal = data.normal.xyz;
	}

	void surf(Input input, inout SurfaceOutputStandard output) {
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
		float3 grassNormalZX = normalize(UnpackNormal(tex2D(GrassNormals, grassCoordinates.zx)));

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

		float3x3 objectSpaceToTangentSpaceZX = transpose(float3x3(zAxis, xAxis, yAxis));
		float3x3 tangentSpaceToObjectSpaceZX = transpose(objectSpaceToTangentSpaceZX);

		//float3 grassNormalInObjectSpaceXY = mul(grassNormalXY, tangentSpaceToObjectSpaceXY);
		//float3 grassNormalInObjectSpaceYZ = mul(grassNormalYZ, tangentSpaceToObjectSpaceYZ);
		float3 grassNormalInObjectSpaceZX = mul(grassNormalZX, tangentSpaceToObjectSpaceZX);

		//grassNormal += grassNormalInObjectSpaceXY * weightXY;
		//grassNormal += grassNormalInObjectSpaceYZ * weightYZ;
		grassNormal += grassNormalInObjectSpaceZX;// *weightZX;

		float3 grassNormalInTangentSpace = mul(grassNormal, objectSpaceToTangentSpace);

		output.Albedo = 0.5;

		//output.Albedo = dot(grassNormal, xAxis);

		/*if(grassNormalZX.x < -0.1) {
		if(grassNormalZX.z > 0.1) {
		output.Albedo = fixed3(grassNormalZX.z * 0.5 + 0.5, 0, 0);
		}
		}
		else if(grassNormalZX.x > 0.1) {
		if (grassNormalZX.z > 0.1) {
		output.Albedo = fixed3(0, grassNormalZX.z * 0.5 + 0.5, 0);
		}
		}*/

		/*if(grassNormalZX.y > 0.25) {
		if(grassNormalZX.z > 0.96) {
		output.Albedo = fixed3(grassNormalZX.z * 0.5 + 0.5, 0, 0);
		}
		}*/

		/*				if(grassNormalInTangentSpace.z < 0) {
		output.Albedo = float3(-grassNormalInTangentSpace.z, 0, 0);
		}
		else if(grassNormalInTangentSpace.z > 0) {
		output.Albedo = float3(0, 0, grassNormalInTangentSpace.z);
		}
		else {
		output.Albedo = float3(0, 1, 0);
		}
		*/
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
		output.Normal = grassNormal;

		if (position.x < 0 || position.y < 0 || position.z < 0) {
			output.Albedo = 0;
		}
	}


	// vertex-to-fragment interpolation data
	// no lightmaps:
#ifndef LIGHTMAP_ON
	struct v2f_surf {
		float4 pos : SV_POSITION;
		float4 tSpace0 : TEXCOORD0;
		float4 tSpace1 : TEXCOORD1;
		float4 tSpace2 : TEXCOORD2;
		float3 custompack0 : TEXCOORD3; // position
		float3 custompack1 : TEXCOORD4; // normal
		float4 custompack2 : TEXCOORD5; // tangent
#if UNITY_SHOULD_SAMPLE_SH
		half3 sh : TEXCOORD6; // SH
#endif
		SHADOW_COORDS(7)
			UNITY_FOG_COORDS(8)
#if SHADER_TARGET >= 30
			float4 lmap : TEXCOORD9;
#endif
		UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
	};
#endif
	// with lightmaps:
#ifdef LIGHTMAP_ON
	struct v2f_surf {
		float4 pos : SV_POSITION;
		float4 tSpace0 : TEXCOORD0;
		float4 tSpace1 : TEXCOORD1;
		float4 tSpace2 : TEXCOORD2;
		float3 custompack0 : TEXCOORD3; // position
		float3 custompack1 : TEXCOORD4; // normal
		float4 custompack2 : TEXCOORD5; // tangent
		float4 lmap : TEXCOORD6;
		SHADOW_COORDS(7)
			UNITY_FOG_COORDS(8)
			UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
	};
#endif

	// vertex shader
	v2f_surf vert_surf(appdata_full v) {
		UNITY_SETUP_INSTANCE_ID(v);
		v2f_surf o;
		UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
		UNITY_TRANSFER_INSTANCE_ID(v,o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		Input customInputData;
		vert(v, customInputData);
		o.custompack0.xyz = customInputData.position;
		o.custompack1.xyz = customInputData.normal;
		o.custompack2.xyzw = customInputData.tangent;
		o.pos = UnityObjectToClipPos(v.vertex);
		float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
		fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
		fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
		fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
		o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
		o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
		o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
#ifdef DYNAMICLIGHTMAP_ON
		o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#endif
#ifdef LIGHTMAP_ON
		o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif

		// SH/ambient and vertex lights
#ifndef LIGHTMAP_ON
#if UNITY_SHOULD_SAMPLE_SH
		o.sh = 0;
		// Approximated illumination from non-important point lights
#ifdef VERTEXLIGHT_ON
		o.sh += Shade4PointLights(
			unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
			unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
			unity_4LightAtten0, worldPos, worldNormal);
#endif
		o.sh = ShadeSHPerVertex(worldNormal, o.sh);
#endif
#endif // !LIGHTMAP_ON

		TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
		UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
		return o;
	}

	inline half4 LightingStandardo(SurfaceOutputStandard s, half3 viewDir, UnityGI gi)
	{
		s.Normal = normalize(s.Normal);

		half shiftAmount = dot(s.Normal, viewDir);
		//s.Normal = shiftAmount < 0.0f ? s.Normal + viewDir * (-shiftAmount + 1e-5f) : s.Normal;

		//s.Normal = normalize(s.Normal);

		if(shiftAmount > 0.0) {
			return half4(1, 0, 0, 1);
		}

		half oneMinusReflectivity = 1;
		half3 specColor = 1;
		s.Albedo = DiffuseAndSpecularFromMetallic(s.Albedo, s.Metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

		// shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
		// this is necessary to handle transparency in physically correct way - only diffuse component gets affected by alpha
		half outputAlpha;
		s.Albedo = PreMultiplyAlpha(s.Albedo, s.Alpha, oneMinusReflectivity, /*out*/ outputAlpha);

		half4 c = UNITY_BRDF_PBS(s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);
		//c.rgb += UNITY_BRDF_GI(s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);
		c.a = outputAlpha;
		return c;
	}

	// fragment shader
	fixed4 frag_surf(v2f_surf IN) : SV_Target{
		UNITY_SETUP_INSTANCE_ID(IN);
	// prepare and unpack data
	Input surfIN;
	UNITY_INITIALIZE_OUTPUT(Input,surfIN);
	surfIN.position.x = 1.0;
	surfIN.normal.x = 1.0;
	surfIN.tangent.x = 1.0;
	surfIN.position = IN.custompack0.xyz;
	surfIN.normal = IN.custompack1.xyz;
	surfIN.tangent = IN.custompack2.xyzw;
	float3 worldPos = float3(IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w);
#ifndef USING_DIRECTIONAL_LIGHT
	fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
	fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif
	fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
#ifdef UNITY_COMPILER_HLSL
	SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
	SurfaceOutputStandard o;
#endif
	o.Albedo = 0.0;
	o.Emission = 0.0;
	o.Alpha = 0.0;
	o.Occlusion = 1.0;
	fixed3 normalWorldVertex = fixed3(0,0,1);

	// call surface function
	surf(surfIN, o);

	// compute lighting & shadowing factor
	UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
		fixed4 c = 0;
	fixed3 worldN;
	worldN.x = dot(IN.tSpace0.xyz, o.Normal);
	worldN.y = dot(IN.tSpace1.xyz, o.Normal);
	worldN.z = dot(IN.tSpace2.xyz, o.Normal);
	//o.Normal = worldN;

	// Setup lighting environment
	UnityGI gi;
	UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
	gi.indirect.diffuse = 0;
	gi.indirect.specular = 0;
#if !defined(LIGHTMAP_ON)
	gi.light.color = _LightColor0.rgb;
	gi.light.dir = lightDir;
#endif
	// Call GI (lightmaps/SH/reflections) lighting function
	UnityGIInput giInput;
	UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
	giInput.light = gi.light;
	giInput.worldPos = worldPos;
	giInput.worldViewDir = worldViewDir;
	giInput.atten = atten;
#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
	giInput.lightmapUV = IN.lmap;
#else
	giInput.lightmapUV = 0.0;
#endif
#if UNITY_SHOULD_SAMPLE_SH
	giInput.ambient = IN.sh;
#else
	giInput.ambient.rgb = 0.0;
#endif
	giInput.probeHDR[0] = unity_SpecCube0_HDR;
	giInput.probeHDR[1] = unity_SpecCube1_HDR;
#if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
	giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
#endif
#if UNITY_SPECCUBE_BOX_PROJECTION
	giInput.boxMax[0] = unity_SpecCube0_BoxMax;
	giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
	giInput.boxMax[1] = unity_SpecCube1_BoxMax;
	giInput.boxMin[1] = unity_SpecCube1_BoxMin;
	giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
#endif
	LightingStandard_GI(o, giInput, gi);

	// realtime lighting: call lighting function
	c += LightingStandardo(o, worldViewDir, gi);
	UNITY_APPLY_FOG(IN.fogCoord, c); // apply fog
	UNITY_OPAQUE_ALPHA(c.a);
	return c;
	}

		ENDCG

	}

		// ---- forward rendering additive lights pass:
		Pass{
		Name "FORWARD"
		Tags{ "LightMode" = "ForwardAdd" }
		ZWrite Off Blend One One

		CGPROGRAM
		// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma target 3.0
#pragma multi_compile_fog
#pragma multi_compile_fwdadd
#pragma skip_variants INSTANCING_ON
#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
		// Surface shader code generated based on:
		// vertex modifier: 'vert'
		// writes to per-pixel normal: YES
		// writes to emission: no
		// writes to occlusion: no
		// needs world space reflection vector: no
		// needs world space normal vector: no
		// needs screen space position: no
		// needs world space position: no
		// needs view direction: no
		// needs world space view direction: no
		// needs world space position for lighting: YES
		// needs world space view direction for lighting: YES
		// needs world space view direction for lightmaps: no
		// needs vertex color: no
		// needs VFACE: no
		// passes tangent-to-world matrix to pixel shader: YES
		// reads from normal: no
		// 0 texcoords actually used
#define UNITY_PASS_FORWARDADD
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

		// Original surface shader snippet:
#line 10 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
		/* UNITY: Original start of shader */
		//#pragma surface surf Standard vertex:vert
		//#pragma target 3.0

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
		//input.normal = data.normal.xyz;

		//float3 bitangent = cross(data.normal.xyz, data.tangent.xyz);
		//data.tangent = float4(cross(bitangent, data.normal.xyz), data.tangent.w);

		float3 xAxis = float3(1.0, 0.0, 0.0);
		float3 yAxis = float3(0.0, 1.0, 0.0);
		float3 zAxis = float3(0.0, 0.0, 1.0);

		data.tangent = float4(xAxis, 1);
		data.normal = float4(zAxis, 0);

		input.tangent = data.tangent;
		input.normal = data.normal.xyz;
	}

	void surf(Input input, inout SurfaceOutputStandard output) {
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
		float3 grassNormalZX = normalize(UnpackNormal(tex2D(GrassNormals, grassCoordinates.zx)));

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

		float3x3 objectSpaceToTangentSpaceZX = transpose(float3x3(zAxis, xAxis, yAxis));
		float3x3 tangentSpaceToObjectSpaceZX = transpose(objectSpaceToTangentSpaceZX);

		//float3 grassNormalInObjectSpaceXY = mul(grassNormalXY, tangentSpaceToObjectSpaceXY);
		//float3 grassNormalInObjectSpaceYZ = mul(grassNormalYZ, tangentSpaceToObjectSpaceYZ);
		float3 grassNormalInObjectSpaceZX = mul(grassNormalZX, tangentSpaceToObjectSpaceZX);

		//grassNormal += grassNormalInObjectSpaceXY * weightXY;
		//grassNormal += grassNormalInObjectSpaceYZ * weightYZ;
		grassNormal += grassNormalInObjectSpaceZX;// *weightZX;

		float3 grassNormalInTangentSpace = mul(grassNormal, objectSpaceToTangentSpace);

		output.Albedo = 0.5;

		//output.Albedo = dot(grassNormal, xAxis);

		/*if(grassNormalZX.x < -0.1) {
		if(grassNormalZX.z > 0.1) {
		output.Albedo = fixed3(grassNormalZX.z * 0.5 + 0.5, 0, 0);
		}
		}
		else if(grassNormalZX.x > 0.1) {
		if (grassNormalZX.z > 0.1) {
		output.Albedo = fixed3(0, grassNormalZX.z * 0.5 + 0.5, 0);
		}
		}*/

		/*if(grassNormalZX.y > 0.25) {
		if(grassNormalZX.z > 0.96) {
		output.Albedo = fixed3(grassNormalZX.z * 0.5 + 0.5, 0, 0);
		}
		}*/

		/*				if(grassNormalInTangentSpace.z < 0) {
		output.Albedo = float3(-grassNormalInTangentSpace.z, 0, 0);
		}
		else if(grassNormalInTangentSpace.z > 0) {
		output.Albedo = float3(0, 0, grassNormalInTangentSpace.z);
		}
		else {
		output.Albedo = float3(0, 1, 0);
		}
		*/
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
		output.Normal = grassNormal;

		if (position.x < 0 || position.y < 0 || position.z < 0) {
			output.Albedo = 0;
		}
	}


	// vertex-to-fragment interpolation data
	struct v2f_surf {
		float4 pos : SV_POSITION;
		fixed3 tSpace0 : TEXCOORD0;
		fixed3 tSpace1 : TEXCOORD1;
		fixed3 tSpace2 : TEXCOORD2;
		float3 worldPos : TEXCOORD3;
		float3 custompack0 : TEXCOORD4; // position
		float3 custompack1 : TEXCOORD5; // normal
		float4 custompack2 : TEXCOORD6; // tangent
		SHADOW_COORDS(7)
			UNITY_FOG_COORDS(8)
	};

	// vertex shader
	v2f_surf vert_surf(appdata_full v) {
		v2f_surf o;
		UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
		Input customInputData;
		vert(v, customInputData);
		o.custompack0.xyz = customInputData.position;
		o.custompack1.xyz = customInputData.normal;
		o.custompack2.xyzw = customInputData.tangent;
		o.pos = UnityObjectToClipPos(v.vertex);
		float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
		fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
		fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
		fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
		o.tSpace0 = fixed3(worldTangent.x, worldBinormal.x, worldNormal.x);
		o.tSpace1 = fixed3(worldTangent.y, worldBinormal.y, worldNormal.y);
		o.tSpace2 = fixed3(worldTangent.z, worldBinormal.z, worldNormal.z);
		o.worldPos = worldPos;

		TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
		UNITY_TRANSFER_FOG(o,o.pos); // pass fog coordinates to pixel shader
		return o;
	}

	// fragment shader
	fixed4 frag_surf(v2f_surf IN) : SV_Target{
		// prepare and unpack data
		Input surfIN;
	UNITY_INITIALIZE_OUTPUT(Input,surfIN);
	surfIN.position.x = 1.0;
	surfIN.normal.x = 1.0;
	surfIN.tangent.x = 1.0;
	surfIN.position = IN.custompack0.xyz;
	surfIN.normal = IN.custompack1.xyz;
	surfIN.tangent = IN.custompack2.xyzw;
	float3 worldPos = IN.worldPos;
#ifndef USING_DIRECTIONAL_LIGHT
	fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
	fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif
	fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
#ifdef UNITY_COMPILER_HLSL
	SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
	SurfaceOutputStandard o;
#endif
	o.Albedo = 0.0;
	o.Emission = 0.0;
	o.Alpha = 0.0;
	o.Occlusion = 1.0;
	fixed3 normalWorldVertex = fixed3(0,0,1);

	// call surface function
	surf(surfIN, o);
	UNITY_LIGHT_ATTENUATION(atten, IN, worldPos)
		fixed4 c = 0;
	fixed3 worldN;
	worldN.x = dot(IN.tSpace0.xyz, o.Normal);
	worldN.y = dot(IN.tSpace1.xyz, o.Normal);
	worldN.z = dot(IN.tSpace2.xyz, o.Normal);
	//o.Normal = worldN;

	// Setup lighting environment
	UnityGI gi;
	UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
	gi.indirect.diffuse = 0;
	gi.indirect.specular = 0;
#if !defined(LIGHTMAP_ON)
	gi.light.color = _LightColor0.rgb;
	gi.light.dir = lightDir;
#endif
	gi.light.color *= atten;
	c += LightingStandard(o, worldViewDir, gi);
	c.a = 0.0;
	UNITY_APPLY_FOG(IN.fogCoord, c); // apply fog
	UNITY_OPAQUE_ALPHA(c.a);
	return c;
	}

		ENDCG

	}

		// ---- deferred shading pass:
		Pass{
		Name "DEFERRED"
		Tags{ "LightMode" = "Deferred" }

		CGPROGRAM
		// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma target 3.0
#pragma exclude_renderers nomrt
#pragma multi_compile_prepassfinal
#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
		// Surface shader code generated based on:
		// vertex modifier: 'vert'
		// writes to per-pixel normal: YES
		// writes to emission: no
		// writes to occlusion: no
		// needs world space reflection vector: no
		// needs world space normal vector: no
		// needs screen space position: no
		// needs world space position: no
		// needs view direction: no
		// needs world space view direction: no
		// needs world space position for lighting: YES
		// needs world space view direction for lighting: YES
		// needs world space view direction for lightmaps: no
		// needs vertex color: no
		// needs VFACE: no
		// passes tangent-to-world matrix to pixel shader: YES
		// reads from normal: no
		// 0 texcoords actually used
#define UNITY_PASS_DEFERRED
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

		// Original surface shader snippet:
#line 10 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
		/* UNITY: Original start of shader */
		//#pragma surface surf Standard vertex:vert
		//#pragma target 3.0

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
		//input.normal = data.normal.xyz;

		//float3 bitangent = cross(data.normal.xyz, data.tangent.xyz);
		//data.tangent = float4(cross(bitangent, data.normal.xyz), data.tangent.w);

		float3 xAxis = float3(1.0, 0.0, 0.0);
		float3 yAxis = float3(0.0, 1.0, 0.0);
		float3 zAxis = float3(0.0, 0.0, 1.0);

		data.tangent = float4(xAxis, 1);
		data.normal = float4(zAxis, 0);

		input.tangent = data.tangent;
		input.normal = data.normal.xyz;
	}

	void surf(Input input, inout SurfaceOutputStandard output) {
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
		float3 grassNormalZX = normalize(UnpackNormal(tex2D(GrassNormals, grassCoordinates.zx)));

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

		float3x3 objectSpaceToTangentSpaceZX = transpose(float3x3(zAxis, xAxis, yAxis));
		float3x3 tangentSpaceToObjectSpaceZX = transpose(objectSpaceToTangentSpaceZX);

		//float3 grassNormalInObjectSpaceXY = mul(grassNormalXY, tangentSpaceToObjectSpaceXY);
		//float3 grassNormalInObjectSpaceYZ = mul(grassNormalYZ, tangentSpaceToObjectSpaceYZ);
		float3 grassNormalInObjectSpaceZX = mul(grassNormalZX, tangentSpaceToObjectSpaceZX);

		//grassNormal += grassNormalInObjectSpaceXY * weightXY;
		//grassNormal += grassNormalInObjectSpaceYZ * weightYZ;
		grassNormal += grassNormalInObjectSpaceZX;// *weightZX;

		float3 grassNormalInTangentSpace = mul(grassNormal, objectSpaceToTangentSpace);

		output.Albedo = 0.5;

		//output.Albedo = dot(grassNormal, xAxis);

		/*if(grassNormalZX.x < -0.1) {
		if(grassNormalZX.z > 0.1) {
		output.Albedo = fixed3(grassNormalZX.z * 0.5 + 0.5, 0, 0);
		}
		}
		else if(grassNormalZX.x > 0.1) {
		if (grassNormalZX.z > 0.1) {
		output.Albedo = fixed3(0, grassNormalZX.z * 0.5 + 0.5, 0);
		}
		}*/

		/*if(grassNormalZX.y > 0.25) {
		if(grassNormalZX.z > 0.96) {
		output.Albedo = fixed3(grassNormalZX.z * 0.5 + 0.5, 0, 0);
		}
		}*/

		/*				if(grassNormalInTangentSpace.z < 0) {
		output.Albedo = float3(-grassNormalInTangentSpace.z, 0, 0);
		}
		else if(grassNormalInTangentSpace.z > 0) {
		output.Albedo = float3(0, 0, grassNormalInTangentSpace.z);
		}
		else {
		output.Albedo = float3(0, 1, 0);
		}
		*/
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
		output.Normal = grassNormal;

		if (position.x < 0 || position.y < 0 || position.z < 0) {
			output.Albedo = 0;
		}
	}


	// vertex-to-fragment interpolation data
	struct v2f_surf {
		float4 pos : SV_POSITION;
		float4 tSpace0 : TEXCOORD0;
		float4 tSpace1 : TEXCOORD1;
		float4 tSpace2 : TEXCOORD2;
		float3 custompack0 : TEXCOORD3; // position
		float3 custompack1 : TEXCOORD4; // normal
		float4 custompack2 : TEXCOORD5; // tangent
#ifndef DIRLIGHTMAP_OFF
		half3 viewDir : TEXCOORD6;
#endif
		float4 lmap : TEXCOORD7;
#ifndef LIGHTMAP_ON
#if UNITY_SHOULD_SAMPLE_SH
		half3 sh : TEXCOORD8; // SH
#endif
#else
#ifdef DIRLIGHTMAP_OFF
		float4 lmapFadePos : TEXCOORD8;
#endif
#endif
		UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
	};

	// vertex shader
	v2f_surf vert_surf(appdata_full v) {
		UNITY_SETUP_INSTANCE_ID(v);
		v2f_surf o;
		UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
		UNITY_TRANSFER_INSTANCE_ID(v,o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		Input customInputData;
		vert(v, customInputData);
		o.custompack0.xyz = customInputData.position;
		o.custompack1.xyz = customInputData.normal;
		o.custompack2.xyzw = customInputData.tangent;
		o.pos = UnityObjectToClipPos(v.vertex);
		float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
		fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
		fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
		fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
		o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
		o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
		o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
		float3 viewDirForLight = UnityWorldSpaceViewDir(worldPos);
#ifndef DIRLIGHTMAP_OFF
		o.viewDir.x = dot(viewDirForLight, worldTangent);
		o.viewDir.y = dot(viewDirForLight, worldBinormal);
		o.viewDir.z = dot(viewDirForLight, worldNormal);
#endif
#ifdef DYNAMICLIGHTMAP_ON
		o.lmap.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#else
		o.lmap.zw = 0;
#endif
#ifdef LIGHTMAP_ON
		o.lmap.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#ifdef DIRLIGHTMAP_OFF
		o.lmapFadePos.xyz = (mul(unity_ObjectToWorld, v.vertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
		o.lmapFadePos.w = (-UnityObjectToViewPos(v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
#endif
#else
		o.lmap.xy = 0;
#if UNITY_SHOULD_SAMPLE_SH
		o.sh = 0;
		o.sh = ShadeSHPerVertex(worldNormal, o.sh);
#endif
#endif
		return o;
	}
#ifdef LIGHTMAP_ON
	float4 unity_LightmapFade;
#endif
	fixed4 unity_Ambient;

	// fragment shader
	void frag_surf(v2f_surf IN,
		out half4 outGBuffer0 : SV_Target0,
		out half4 outGBuffer1 : SV_Target1,
		out half4 outGBuffer2 : SV_Target2,
		out half4 outEmission : SV_Target3) {
		UNITY_SETUP_INSTANCE_ID(IN);
		// prepare and unpack data
		Input surfIN;
		UNITY_INITIALIZE_OUTPUT(Input,surfIN);
		surfIN.position.x = 1.0;
		surfIN.normal.x = 1.0;
		surfIN.tangent.x = 1.0;
		surfIN.position = IN.custompack0.xyz;
		surfIN.normal = IN.custompack1.xyz;
		surfIN.tangent = IN.custompack2.xyzw;
		float3 worldPos = float3(IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w);
#ifndef USING_DIRECTIONAL_LIGHT
		fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
		fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif
		fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
#ifdef UNITY_COMPILER_HLSL
		SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
		SurfaceOutputStandard o;
#endif
		o.Albedo = 0.0;
		o.Emission = 0.0;
		o.Alpha = 0.0;
		o.Occlusion = 1.0;
		fixed3 normalWorldVertex = fixed3(0,0,1);

		// call surface function
		surf(surfIN, o);
		fixed3 originalNormal = o.Normal;
		fixed3 worldN;
		worldN.x = dot(IN.tSpace0.xyz, o.Normal);
		worldN.y = dot(IN.tSpace1.xyz, o.Normal);
		worldN.z = dot(IN.tSpace2.xyz, o.Normal);
		//o.Normal = worldN;
		half atten = 1;

		// Setup lighting environment
		UnityGI gi;
		UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
		gi.indirect.diffuse = 0;
		gi.indirect.specular = 0;
		gi.light.color = 0;
		gi.light.dir = half3(0,1,0);
		// Call GI (lightmaps/SH/reflections) lighting function
		UnityGIInput giInput;
		UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
		giInput.light = gi.light;
		giInput.worldPos = worldPos;
		giInput.worldViewDir = worldViewDir;
		giInput.atten = atten;
#if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
		giInput.lightmapUV = IN.lmap;
#else
		giInput.lightmapUV = 0.0;
#endif
#if UNITY_SHOULD_SAMPLE_SH
		giInput.ambient = IN.sh;
#else
		giInput.ambient.rgb = 0.0;
#endif
		giInput.probeHDR[0] = unity_SpecCube0_HDR;
		giInput.probeHDR[1] = unity_SpecCube1_HDR;
#if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
		giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
#endif
#if UNITY_SPECCUBE_BOX_PROJECTION
		giInput.boxMax[0] = unity_SpecCube0_BoxMax;
		giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
		giInput.boxMax[1] = unity_SpecCube1_BoxMax;
		giInput.boxMin[1] = unity_SpecCube1_BoxMin;
		giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
#endif
		LightingStandard_GI(o, giInput, gi);

		// call lighting function to output g-buffer
		outEmission = LightingStandard_Deferred(o, worldViewDir, gi, outGBuffer0, outGBuffer1, outGBuffer2);
#ifndef UNITY_HDR_ON
		outEmission.rgb = exp2(-outEmission.rgb);
#endif
	}

	ENDCG

	}

		// ---- meta information extraction pass:
		Pass{
		Name "Meta"
		Tags{ "LightMode" = "Meta" }
		Cull Off

		CGPROGRAM
		// compile directives
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma target 3.0
#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
#pragma skip_variants INSTANCING_ON
#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
		// Surface shader code generated based on:
		// vertex modifier: 'vert'
		// writes to per-pixel normal: YES
		// writes to emission: no
		// writes to occlusion: no
		// needs world space reflection vector: no
		// needs world space normal vector: no
		// needs screen space position: no
		// needs world space position: no
		// needs view direction: no
		// needs world space view direction: no
		// needs world space position for lighting: YES
		// needs world space view direction for lighting: YES
		// needs world space view direction for lightmaps: no
		// needs vertex color: no
		// needs VFACE: no
		// passes tangent-to-world matrix to pixel shader: YES
		// reads from normal: no
		// 0 texcoords actually used
#define UNITY_PASS_META
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"

#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))

		// Original surface shader snippet:
#line 10 ""
#ifdef DUMMY_PREPROCESSOR_TO_WORK_AROUND_HLSL_COMPILER_LINE_HANDLING
#endif
		/* UNITY: Original start of shader */
		//#pragma surface surf Standard vertex:vert
		//#pragma target 3.0

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
		//input.normal = data.normal.xyz;

		//float3 bitangent = cross(data.normal.xyz, data.tangent.xyz);
		//data.tangent = float4(cross(bitangent, data.normal.xyz), data.tangent.w);

		float3 xAxis = float3(1.0, 0.0, 0.0);
		float3 yAxis = float3(0.0, 1.0, 0.0);
		float3 zAxis = float3(0.0, 0.0, 1.0);

		data.tangent = float4(xAxis, 1);
		data.normal = float4(zAxis, 0);

		input.tangent = data.tangent;
		input.normal = data.normal.xyz;
	}

	void surf(Input input, inout SurfaceOutputStandard output) {
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
		float3 grassNormalZX = normalize(UnpackNormal(tex2D(GrassNormals, grassCoordinates.zx)));

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

		float3x3 objectSpaceToTangentSpaceZX = transpose(float3x3(zAxis, xAxis, yAxis));
		float3x3 tangentSpaceToObjectSpaceZX = transpose(objectSpaceToTangentSpaceZX);

		//float3 grassNormalInObjectSpaceXY = mul(grassNormalXY, tangentSpaceToObjectSpaceXY);
		//float3 grassNormalInObjectSpaceYZ = mul(grassNormalYZ, tangentSpaceToObjectSpaceYZ);
		float3 grassNormalInObjectSpaceZX = mul(grassNormalZX, tangentSpaceToObjectSpaceZX);

		//grassNormal += grassNormalInObjectSpaceXY * weightXY;
		//grassNormal += grassNormalInObjectSpaceYZ * weightYZ;
		grassNormal += grassNormalInObjectSpaceZX;// *weightZX;

		float3 grassNormalInTangentSpace = mul(grassNormal, objectSpaceToTangentSpace);

		output.Albedo = 0.5;

		//output.Albedo = dot(grassNormal, xAxis);

		/*if(grassNormalZX.x < -0.1) {
		if(grassNormalZX.z > 0.1) {
		output.Albedo = fixed3(grassNormalZX.z * 0.5 + 0.5, 0, 0);
		}
		}
		else if(grassNormalZX.x > 0.1) {
		if (grassNormalZX.z > 0.1) {
		output.Albedo = fixed3(0, grassNormalZX.z * 0.5 + 0.5, 0);
		}
		}*/

		/*if(grassNormalZX.y > 0.25) {
		if(grassNormalZX.z > 0.96) {
		output.Albedo = fixed3(grassNormalZX.z * 0.5 + 0.5, 0, 0);
		}
		}*/

		/*				if(grassNormalInTangentSpace.z < 0) {
		output.Albedo = float3(-grassNormalInTangentSpace.z, 0, 0);
		}
		else if(grassNormalInTangentSpace.z > 0) {
		output.Albedo = float3(0, 0, grassNormalInTangentSpace.z);
		}
		else {
		output.Albedo = float3(0, 1, 0);
		}
		*/
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
		output.Normal = grassNormal;

		if (position.x < 0 || position.y < 0 || position.z < 0) {
			output.Albedo = 0;
		}
	}

#include "UnityMetaPass.cginc"

	// vertex-to-fragment interpolation data
	struct v2f_surf {
		float4 pos : SV_POSITION;
		float4 tSpace0 : TEXCOORD0;
		float4 tSpace1 : TEXCOORD1;
		float4 tSpace2 : TEXCOORD2;
		float3 custompack0 : TEXCOORD3; // position
		float3 custompack1 : TEXCOORD4; // normal
		float4 custompack2 : TEXCOORD5; // tangent
	};

	// vertex shader
	v2f_surf vert_surf(appdata_full v) {
		v2f_surf o;
		UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
		Input customInputData;
		vert(v, customInputData);
		o.custompack0.xyz = customInputData.position;
		o.custompack1.xyz = customInputData.normal;
		o.custompack2.xyzw = customInputData.tangent;
		o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST);
		float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
		fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
		fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
		fixed3 worldBinormal = cross(worldNormal, worldTangent) * tangentSign;
		o.tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
		o.tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
		o.tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
		return o;
	}

	// fragment shader
	fixed4 frag_surf(v2f_surf IN) : SV_Target{
		// prepare and unpack data
		Input surfIN;
	UNITY_INITIALIZE_OUTPUT(Input,surfIN);
	surfIN.position.x = 1.0;
	surfIN.normal.x = 1.0;
	surfIN.tangent.x = 1.0;
	surfIN.position = IN.custompack0.xyz;
	surfIN.normal = IN.custompack1.xyz;
	surfIN.tangent = IN.custompack2.xyzw;
	float3 worldPos = float3(IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w);
#ifndef USING_DIRECTIONAL_LIGHT
	fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
#else
	fixed3 lightDir = _WorldSpaceLightPos0.xyz;
#endif
#ifdef UNITY_COMPILER_HLSL
	SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
	SurfaceOutputStandard o;
#endif
	o.Albedo = 0.0;
	o.Emission = 0.0;
	o.Alpha = 0.0;
	o.Occlusion = 1.0;
	fixed3 normalWorldVertex = fixed3(0,0,1);

	// call surface function
	surf(surfIN, o);
	UnityMetaInput metaIN;
	UNITY_INITIALIZE_OUTPUT(UnityMetaInput, metaIN);
	metaIN.Albedo = o.Albedo;
	metaIN.Emission = o.Emission;
	return UnityMetaFragment(metaIN);
	}

		ENDCG

	}

		// ---- end of surface shader generated code

		#LINE 181

	}
		FallBack "Diffuse"
}
