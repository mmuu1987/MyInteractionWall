Shader "Unlit/RotTest"
{
	Properties
	{
		
		_Angle("Angle",float)=0
		
		_TexArrOne ("Texture Array", 2DArray) = "" {}
		_TexArrTwo ("Texture Array", 2DArray) = "" {}
	
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
				float angle :TEXCOORD1;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};
		    UNITY_DECLARE_TEX2DARRAY(_TexArrOne);
			UNITY_DECLARE_TEX2DARRAY(_TexArrTwo);
			
			
			float _Angle;
			
		
			v2f vert (appdata v)
			{
				v2f o;
				//_Angle += _Time.y *50;
				float4 rot = rotate_angle_axis(_Angle/RadianRatio,float3(1,0,0));
				float3 newVector = rotate_vector_at(v.vertex,float3(0,0,0),rot);
				
				
				o.vertex = UnityObjectToClipPos(float4(newVector,1));
				//o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv =v.uv;
				o.angle = _Angle;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			float rot(float angle)
			{
			  
				if(angle>0)angle*=-1;
				//-80  -100  转换图片 4*3 1920 1080

				float f =1;
			   
			   angle =  fmod(angle,360);

			   if(angle>-180)
			   {
			     
			      if(angle >=-100 && angle <= -80)
				  {
				   f = (angle +80)/-20;
			      }
				  else if(angle<-100) f=1;
				  else if(angle>-80)f=0;
			   }
			   else if (angle<-180 && angle >-360)
			   {
			        if(angle >=-280 && angle <= -260)
			    	{
				     f =abs( (angle +280)/20);
			    	}
			        else if(angle<-280) f=0;
			   }
			   return f;
			}
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col1 =  UNITY_SAMPLE_TEX2DARRAY(_TexArrOne, float3(i.uv.xy, 0));
				fixed4 col2 =  UNITY_SAMPLE_TEX2DARRAY(_TexArrTwo, float3(i.uv.xy, 0));

				float f =rot(i.angle);

				fixed4 col = lerp(col1,col2,f)
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
