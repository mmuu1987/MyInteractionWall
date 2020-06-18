  Shader "Instanced/Texture2DArrayCompute" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_TexArr ("Texture Array", 2DArray) = "" {}
		_Color("Color",color) = (1,1,1,1)
		_BGColor("BGColor",color) = (1,1,1,1)
		_RADIUSBUCE("_RADIUSBUCE",Range(0,0.5))=0.2
		_WHScale("WHScale",vector)=(1,1,1,1)//目前只使用x y
    }
    SubShader {
	//第一个描边PASS
	  //  Pass {

   //         Tags { "RenderType"="Opaque" "Queue"="Transparent"}

			//Blend SrcAlpha  OneMinusSrcAlpha
   //         CGPROGRAM

   //         #pragma vertex vert
   //         #pragma fragment frag
   //         #pragma target 4.5
			////#pragma multi_compile_instancing
   //         #include "UnityCG.cginc"
          
		   
   //         sampler2D _MainTex;
			//fixed4 _BGColor;

      	 
		 // struct PosDir
		 //  {
		 //     float4 position;
   //           float3 velocity;
		 //     float3 initialVelocity;
   //           float4 originalPos;
		 //     float3 moveTarget;
			//  float3 moveDir;
			//  float2 indexRC;
			//  int picIndex;
		 //  };
			//#if SHADER_TARGET >= 45
   //         StructuredBuffer<PosDir> positionBuffer;
   //         #endif

   //         struct v2f
   //         {
   //             float4 pos : SV_POSITION;
   //         };
   //         v2f vert (appdata_base v, uint instanceID : SV_InstanceID)
   //         {
   //         #if SHADER_TARGET >= 45
   //             float4 data = positionBuffer[instanceID].position;
   //         #else
   //             float4 data = 0;
   //         #endif

   //             float3 localPosition = v.vertex.xyz * data.w * 1.05f;//向外拓展，用作描边
   //             float3 worldPosition = data.xyz + localPosition;

   //             v2f o;
   //             o.pos = mul(UNITY_MATRIX_VP, float4(worldPosition, 1.0f));
   //             return o;
   //         }

   //         fixed4 frag (v2f i, uint instanceID : SV_InstanceID) : SV_Target
   //         {
   //            return _BGColor;
   //         }

   //         ENDCG
   //     }

        Pass {

            Tags { "RenderType"="Opaque" "Queue"="Transparent"}

			//Blend SrcAlpha  OneMinusSrcAlpha
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
			//#pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "Assets/ComputeShader/GPUParticle.cginc"
		    UNITY_DECLARE_TEX2DARRAY(_TexArr);
            sampler2D _MainTex;
			fixed4 _Color;
			float _RADIUSBUCE;
			float4 _WHScale;
      	 

			#if SHADER_TARGET >= 45
            StructuredBuffer<PosAndDir> positionBuffer;
			StructuredBuffer<float4> colorBuffer;
            #endif

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv_MainTex : TEXCOORD0;
				float2 uv_Main2Tex:TEXCOORD2;
				float2 RadiusBuceVU : TEXCOORD1;
				uint index:SV_InstanceID;//告诉片元，输送实例ID
             
            };
            v2f vert (appdata_base v, uint instanceID : SV_InstanceID)
            {
            #if SHADER_TARGET >= 45
                float4 data = positionBuffer[instanceID].position;
            #else
                float4 data = 0;
            #endif

                float3 localPosition = v.vertex.xyz * data.w;
				localPosition.x *=_WHScale.x;
				localPosition.y *=_WHScale.y;
                float3 worldPosition = data.xyz + localPosition;
				float3 cache =  positionBuffer[instanceID].initialVelocity;
				float4 uvOffset  =  positionBuffer[instanceID].uvOffset;
				float4 uv2Offset =  positionBuffer[instanceID].uv2Offset;
				
				//mainUV=uvOffset;
                v2f o;
                o.pos = mul(UNITY_MATRIX_VP, float4(worldPosition, 1.0f));
                o.uv_MainTex  = v.texcoord * uvOffset.xy  + uvOffset.zw;
				o.uv_Main2Tex = v.texcoord * uv2Offset.xy + uv2Offset.zw;
			   // v.texcoord.x *=-1;
				//o.uv_MainTex = v.texcoord;
				o.index = instanceID;
				o.RadiusBuceVU=v.texcoord-float2(0.5,0.5);       //将模型UV坐标原点置为中心原点,为了方便计算
                return o;
            }

            fixed4 frag (v2f i, uint instanceID : SV_InstanceID) : SV_Target
            {
			   fixed4 col = fixed4(1,0,0,1);
			   fixed4 col2 = fixed4(1,0,0,1);
			   #if SHADER_TARGET >= 45
               int index = positionBuffer[instanceID].picIndex;
			   int index2 =positionBuffer[instanceID].bigIndex;
			   float lerpValue = positionBuffer[instanceID].velocity.w;
			   float2 uv1 = i.uv_MainTex;
			   float2 uv2 = i.uv_Main2Tex;
			   float2 lerpUV = lerp(uv1,uv2,lerpValue);
			    //圆角作用  
			    if(abs(i.RadiusBuceVU.x)<0.5-_RADIUSBUCE||abs(i.RadiusBuceVU.y)<0.5-_RADIUSBUCE)    //即上面说的|x|<(0.5-r)或|y|<(0.5-r)
                {
					col = UNITY_SAMPLE_TEX2DARRAY(_TexArr, float3(i.uv_MainTex, index));
					col2 = UNITY_SAMPLE_TEX2DARRAY(_TexArr, float3(lerpUV, index2));
                }
				else
				{
				if(length( abs( i.RadiusBuceVU)-float2(0.5-_RADIUSBUCE,0.5-_RADIUSBUCE)) <_RADIUSBUCE)
				{
				 col = UNITY_SAMPLE_TEX2DARRAY(_TexArr, float3(i.uv_MainTex, index));
				 col2 = UNITY_SAMPLE_TEX2DARRAY(_TexArr, float3(lerpUV, index2));
				}
				else
				{
				discard;
				}
				 
				}
				col = lerp(col,col2,lerpValue);
            #else
                 col = _Color;
            #endif
               return fixed4(col.xyz,1);
            }

            ENDCG
        }
		
    }

	FallBack "Diffuse" 
}