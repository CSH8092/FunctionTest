Shader "Unlit/Gradient"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (0.117, 0.137, 0.137, 1)  // 상단 색상
        _BottomColor ("Bottom Color", Color) = (0.055, 0.055, 0.055, 1)  // 하단 색상
        _MainTex ("Texture", 2D) = "white" {}  // 텍스처 (사용하지 않더라도 포함)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            ZWRITE OFF

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            // 상단과 하단 색상 정의
            fixed4 _TopColor;
            fixed4 _BottomColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // UV의 Y축을 기준으로 그라데이션 보간
                float rate = 1.0 - i.uv.y;
                fixed4 col = lerp(_BottomColor, _TopColor, rate);

                // Fog 적용
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
