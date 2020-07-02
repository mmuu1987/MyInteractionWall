﻿Shader "Unlit/TransparentTest"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Rnage("Range",Range(0,1)) =0.5
	}
	SubShader
	{
		 Tags { "RenderType"="Transparent" "Queue"="Transparent"}

		 ZWrite off
		
		 Blend SrcAlpha  OneMinusSrcAlpha
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
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
			
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Rnage;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
			
				return fixed4(col.r,col.g,col.b,_Rnage);
			}
			ENDCG
		}
	}
}
