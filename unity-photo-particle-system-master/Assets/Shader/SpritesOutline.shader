﻿Shader "Unlit/SpritesOutline"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

		_OutLineWidth("OutLineWidth",Range(0,0.5))=0.1
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
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _OutLineWidth;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			  	
				float u = i.uv.x;
				float v = i.uv.y;
			    fixed4 col = fixed4(1,1,1,1);

			   if(u>=_OutLineWidth && u<=1-_OutLineWidth  && v>=_OutLineWidth && v<=1-_OutLineWidth )
			   {
			     	
					 col = tex2D(_MainTex, i.uv);
					  
			   }
				return col;
			}
			ENDCG
		}
	}
}
