Shader "Unlit/ItemShader"
{
	Properties
	{
		_Index("Index",int) = 1
		_TexArrOne ("Texture Array", 2DArray) = "" {}
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
				
				float4 vertex : SV_POSITION;
			};
			 UNITY_DECLARE_TEX2DARRAY(_TexArrOne);
		    int _Index;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				
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
