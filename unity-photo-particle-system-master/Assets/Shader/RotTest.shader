Shader "Unlit/RotTest"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RotTex("RotTex",2D) = "white"{}
		_Angle("Angle",float)=0
		_Range("Range",Range(0,1))=0.5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		cull Off
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "Assets/Common/Shaders/Math.cginc"
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
			float _Angle;
			sampler2D _RotTex;
			float _Range;
			v2f vert (appdata v)
			{
				v2f o;
				//_Angle += _Time.y *50;
				//float4 rot = rotate_angle_axis(_Angle/RadianRatio,float3(0,1,0));
				//float3 newVector = rotate_vector_at(v.vertex,float3(0,0,0),rot);
				//if(_Angle%360>=90 && _Angle%360<=270)
				//{
				//  v.uv.x *=-1;
				//}
				
				//o.vertex = UnityObjectToClipPos(float4(newVector,1));
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col1 = tex2D(_MainTex, i.uv);
				fixed4 col2 = tex2D(_RotTex,i.uv);

				fixed4 col = lerp(col1,col2,_Range)
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
