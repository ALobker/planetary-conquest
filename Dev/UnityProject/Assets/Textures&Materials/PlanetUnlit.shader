Shader "Unlit/PlanetUnlit"
{
	Properties
	{

	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR1;
			};

			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float radius = length(v.vertex);
				o.color = saturate((radius - 900.0f) / 200.0f);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = i.color;

				return col;
			}
			ENDCG
		}
	}
}
