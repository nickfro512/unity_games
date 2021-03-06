// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/OVRMRClipPlane"
{
	Properties
	{
		_Color("Chroma Key Color", Color) = (0,1,0,1)
		_Visible("Visible", Range(0.0,1.0)) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
			};

			fixed4 _Color;
			fixed  _Visible;
			
			v2f vert (appdata v)
			{
				v2f o;
#if UNITY_VERSION >= 540
				o.vertex = UnityObjectToClipPos(v.vertex);
#else
				o.vertex = UnityObjectToClipPos(v.vertex);
#endif
				o.vertex *= _Visible;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = _Color;
				return col;
			}
			ENDCG
		}
	}
}
