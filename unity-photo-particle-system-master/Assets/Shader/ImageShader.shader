Shader "Unlit/ImageShader"
{
	Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
	
	_MainTex("Texture",2D) = "white"{}

	_Arg("arg",Range(0,5)) =1.73205080756887

}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100

    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha 

    Pass {  
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
               UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _Color;
			sampler2D _MainTex;
			float _Arg;
            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
             
                return o;
            }

            fixed4 frag (v2f input) : COLOR
            {
                fixed4 col = _Color;

                fixed3 n[6] = {
                    fixed3(0, 1, -1),
                    fixed3(1, 0, -1),
                    fixed3(-1, 1, 0),
                    fixed3(1, -1, 0),
                    fixed3(-1, 0, 1),
                    fixed3(0, -1, 1),
                };


                fixed c = 1;


                fixed disc = distance(input.uv.xy,fixed2(0.5,0.5));


                fixed mindis = 1;
                for(int i=0; i<6; i++){
                    fixed2 pos = fixed2(0.5+ 0.5 * _Arg * (n[i].x + n[i].z * .5f),0.5 + 0.5 * 1.5 * n[i].z);
                    fixed a = distance(input.uv.xy,pos);
                    mindis = min(mindis,a);
                }

                fixed e = step(abs(disc-mindis),0.04);

				fixed4 texcol = tex2D(_MainTex,input.uv);
                //实心显示
                 e = step(disc,mindis);
                col = _Color * e*texcol;

              
                return col;
            }
        ENDCG
    }
}
}
