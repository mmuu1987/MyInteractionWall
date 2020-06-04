Shader "COMShader/Shader2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"


			StructuredBuffer<float3> points;

			struct verIN
			{
				uint id:SV_VERTEXID;
			};

			struct verOUT
			{
				float3 uv : TEXCOORD0;
			
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			verOUT vert (verIN v)
			{
				verOUT o;
				o.vertex =mul(UNITY_MATRIX_VP,float4(points[v.id],1));
				o.uv =points[v.id]/64;
				
				return o;
			}
			
			fixed4 frag (verOUT o) : SV_Target
			{
				
			
				return fixed4(o.uv,1);
			}
			ENDCG
		}
	}
}
