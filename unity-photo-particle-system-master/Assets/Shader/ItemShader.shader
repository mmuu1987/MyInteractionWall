Shader "Unlit/ItemShader"
{
	Properties
	{
		_Index("Index",int) = 1
		_TexArrOne ("Texture Array", 2DArray) = "" {}
		_Yscale("Yscale",Range(0,1)) = 1
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
		    #pragma target 4.5
			#include "UnityCG.cginc"
			UNITY_DECLARE_TEX2DARRAY(_TexArrOne);
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
			 
		    int _Index;
			float _Yscale;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float v1 = v.uv.y*_Yscale;
				float u1 = v.uv.x;
				o.uv = float2(u1,v1);
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

			    fixed4 col=  UNITY_SAMPLE_TEX2DARRAY(_TexArrOne, float3(i.uv.xy, _Index));
			
				return col;
			}
			ENDCG
		}
	}
}
