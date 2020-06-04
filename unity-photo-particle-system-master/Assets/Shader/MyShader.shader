  Shader "Instanced/MyShader" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		
		_Color("Color",color) = (1,1,1,1)
    }
    SubShader {

        Pass {

            Tags { "RenderType"="Opaque" "Queue"="Transparent"}

			Blend SrcAlpha  OneMinusSrcAlpha
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            #include "UnityCG.cginc"
          

            sampler2D _MainTex;
			
			fixed4 _Color;

      	  struct PosDir
		   {
		      float4 position;
              float3 velocity;
			  float3 initialVelocity;
			  float3 fluidUp;
			  float3 fluidDown;
			  float4 oldPos;
			  float3 addvalUp;
			  float3 addvalDown;
              int heardIndex;
              float4 originalPos;
			  float4 freeMoveArg;
			  int delayFrame;
		   };

			#if SHADER_TARGET >= 45
            StructuredBuffer<PosDir> positionBuffer;
			StructuredBuffer<float4> colorBuffer;
            #endif

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv_MainTex : TEXCOORD0;
             
            };
            v2f vert (appdata_base v, uint instanceID : SV_InstanceID)
            {
            #if SHADER_TARGET >= 45
                float4 data = positionBuffer[instanceID].position;
            #else
                float4 data = 0;
            #endif

                float3 localPosition = v.vertex.xyz * data.w;
                float3 worldPosition = data.xyz + localPosition;

                v2f o;
                o.pos = mul(UNITY_MATRIX_VP, float4(worldPosition, 1.0f));
                o.uv_MainTex = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i, uint instanceID : SV_InstanceID) : SV_Target
            {
			   fixed4 col = tex2D(_MainTex, i.uv_MainTex) * colorBuffer[instanceID];
               return col;
            }

            ENDCG
        }
		
    }

	FallBack "Diffuse" 
}