﻿Shader "Unlit/MultiDepthMotion"
{
	Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_TexArr ("Texture Array", 2DArray) = "" {}
		_Color("Color",color) = (1,1,1,1)
		_BGColor("BGColor",color) = (1,1,1,1)
		_RADIUSBUCE("_RADIUSBUCE",Range(0,0.5))=0.2
		_WHScale("WHScale",vector)=(1,1,1,1)//目前只使用x y
    }
 SubShader {
	
        Pass {

            Tags { "RenderType"="Opaque" "Queue"="Transparent"}
			cull Off
			Blend SrcAlpha  OneMinusSrcAlpha
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
			//#pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "Assets/Common/Shaders/Math.cginc"
		    UNITY_DECLARE_TEX2DARRAY(_TexArr);
            sampler2D _MainTex;
			fixed4 _Color;
			float _RADIUSBUCE;
			float4 _WHScale;
      	  struct PosDir
		   {
		      float4 position;
              float4 velocity;
		      float3 initialVelocity;
              float4 originalPos;
		      float3 moveTarget;
			  float3 moveDir;
			  float2 indexRC;
			  int picIndex;
			  int bigIndex;
			  float4 uvOffset; 
			  float4 uv2Offset; 
		   };

			#if SHADER_TARGET >= 45
            StructuredBuffer<PosDir> positionBuffer;
			StructuredBuffer<float4> colorBuffer;
            #endif

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv_MainTex : TEXCOORD0;
				float2 RadiusBuceVU : TEXCOORD1;
				float angle :TEXCOORD2;
				uint index:SV_InstanceID;//告诉片元，输送实例ID
             
            };
            v2f vert (appdata_base v, uint instanceID : SV_InstanceID)
            {
            #if SHADER_TARGET >= 45
                float4 data = positionBuffer[instanceID].position;
            #else
                float4 data = 0;
            #endif
			    v2f o;
				float lerpValue = positionBuffer[instanceID].originalPos.w;//得到旋转的插值系数
				float angle = -360* lerpValue;
				float4 rot = rotate_angle_axis(angle/RadianRatio,float3(1,0,0));
				float3 newVector = rotate_vector_at(v.vertex,float3(0,0,0),rot);

                float3 localPosition = newVector * data.w;
				localPosition.x *=_WHScale.x;
				localPosition.y *=_WHScale.y;
                float3 worldPosition = data.xyz + localPosition;
				

				

				o.pos = UnityObjectToClipPos(float4(worldPosition,1));
              
			    o.angle = angle;
				o.uv_MainTex = v.texcoord;
				o.index = instanceID;
				o.RadiusBuceVU=v.texcoord-float2(0.5,0.5);       //将模型UV坐标原点置为中心原点,为了方便计算
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

            fixed4 frag (v2f i, uint instanceID : SV_InstanceID) : SV_Target
            {
			   fixed4 col = fixed4(1,0,0,1);
			   fixed4 col2 = fixed4(1,0,0,1);
			   #if SHADER_TARGET >= 45
               int index = positionBuffer[instanceID].picIndex;
			   int index2 =positionBuffer[instanceID].bigIndex;

			   float4 velocity =  positionBuffer[instanceID].velocity;
			   float alpha = velocity.y;
			   float f = rot(i.angle);
			
			    //圆角作用  
			    if(abs(i.RadiusBuceVU.x)<0.5-_RADIUSBUCE||abs(i.RadiusBuceVU.y)<0.5-_RADIUSBUCE)    //即上面说的|x|<(0.5-r)或|y|<(0.5-r)
                {
					col = UNITY_SAMPLE_TEX2DARRAY(_TexArr, float3(i.uv_MainTex, index));
					col2 = UNITY_SAMPLE_TEX2DARRAY(_TexArr, float3(i.uv_MainTex, index2));
                }
				else
				{
				if(length( abs( i.RadiusBuceVU)-float2(0.5-_RADIUSBUCE,0.5-_RADIUSBUCE)) <_RADIUSBUCE)
				{
				 col = UNITY_SAMPLE_TEX2DARRAY(_TexArr, float3(i.uv_MainTex, index));
				 col2 = UNITY_SAMPLE_TEX2DARRAY(_TexArr, float3(i.uv_MainTex, index2));
				}
				else
				{
				discard;
				}
				 
				}
				col = lerp(col,col2,f);
            #else
                 col = _Color;
            #endif
               return fixed4(col.xyz,alpha);
            }

            ENDCG
        }
		
    }

	FallBack "Diffuse" 
}
