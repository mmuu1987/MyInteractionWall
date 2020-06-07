Shader "Unlit/Texture2DArray 1"
{
	 Properties
    {
        _TexArrOne ("Texture Array", 2DArray) = "" {}
		_TexArrTwo ("Texture Array", 2DArray) = "" {}
        _Index("Texture Array Index", Range(0,4)) = 0
    }

    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // 会提示警告: Unrecognized #pragma directive: require at line 24
            // #pragma require 2darray

            UNITY_DECLARE_TEX2DARRAY(_TexArrOne);
			UNITY_DECLARE_TEX2DARRAY(_TexArrTwo);
            int _Index;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
			    fixed4 clo1 =  UNITY_SAMPLE_TEX2DARRAY(_TexArrOne, float3(i.uv.xy, _Index));
				fixed4 clo2 =  UNITY_SAMPLE_TEX2DARRAY(_TexArrTwo, float3(i.uv.xy, _Index));
                return lerp(clo1,clo2,0.5);
            }
            ENDCG
        }
    }

    Fallback Off
}
