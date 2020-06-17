Shader "Unlit/MarkShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Mark ("Mark", 2D) = "white" {}
		_Hollow ("Hollow", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha 
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
				
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _Mark;
			sampler2D _Hollow;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			
			{
			    fixed4 col  = float4(1,1,1,1);

			    fixed4 mark = tex2D(_Mark, i.uv);
				fixed4 Hollow = tex2D(_Hollow, i.uv);
				fixed4 mainTex = tex2D(_MainTex, i.uv);

				mainTex.a = 1-mark.a;

				if(Hollow.a>0.3)mainTex.rgb=Hollow.rgb;

				col = mainTex;
				
				return col;
			}
			ENDCG
		}
	}
}
