Shader "Unlit/MultiDepthCPU"
{
	Properties
	{
		_Index("Index",int) = 1
		_TexArrOne ("Texture Array", 2DArray) = "" {}
		_Alpha("alpha",Range(0,1))=1
	}
	SubShader
	{
	
	Pass
		{

		 Tags { "Queue"="Transparent"   "RenderType"="Transparent"   "IgnoreProjection" = "True"}
		 ZWrite off
		  Blend SrcAlpha  OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
		    #pragma target 4.5
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
			 
		 
			float _Alpha;
			v2f vert (appdata v)
			{
				v2f o;
				v.vertex.xy*=1.03f;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return fixed4(1,1,1,_Alpha);
			}
			ENDCG
		}
	


		Pass
		{

		 Tags { "Queue"="Transparent"   "RenderType"="Transparent"   "IgnoreProjection" = "True"}
		 ZWrite off
		  Blend SrcAlpha  OneMinusSrcAlpha
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
			float _Alpha;
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
			
				return fixed4(col.rgb,_Alpha);
			}
			ENDCG
		}
}
}
