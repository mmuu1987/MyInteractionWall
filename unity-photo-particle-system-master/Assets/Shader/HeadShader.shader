Shader "Unlit/HeadShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Mark ("Mark", 2D) = "white" {}
		_Hollow ("Hollow", 2D) = "white" {}
		_Range("Range",Range(0,1))=0.1
		//MASK SUPPORT ADD
		_StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
		//MASK SUPPORT END
	}
	SubShader
	{
		 Tags { "RenderType"="Transparent" "Queue"="Transparent"}



		 //MASK SUPPORT ADD
		Stencil
{
    Ref [_Stencil]
    Comp [_StencilComp]
    Pass [_StencilOp] 
    ReadMask [_StencilReadMask]
    WriteMask [_StencilWriteMask]
}

        ColorMask [_ColorMask]
	//MASK SUPPORT END

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
			float _Range;
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

				fixed4 mainTex = tex2D(_MainTex, i.uv);
			    fixed4 mark = tex2D(_Mark, i.uv);
				fixed4 Hollow = tex2D(_Hollow, i.uv);
				
				if(mark.a<=0.5)
				{
				 col = mainTex;
				}
				else
				{
				  if(Hollow.a>=_Range)
				  { 
				 
				    col = Hollow;
					//col.a=1;
				  }
				}
				

				

				//if(Hollow.a>0.3)mainTex.rgb=Hollow.rgb;

				//col = mainTex;
				
				return col;
			}
			ENDCG
		}
	}
}
