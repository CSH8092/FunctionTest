Shader "Unlit/Gradient"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (0.117, 0.137, 0.137, 1)  // ��� ����
        _BottomColor ("Bottom Color", Color) = (0.055, 0.055, 0.055, 1)  // �ϴ� ����
        _MainTex ("Texture", 2D) = "white" {}  // �ؽ�ó (������� �ʴ��� ����)
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

            // ��ܰ� �ϴ� ���� ����
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
                // UV�� Y���� �������� �׶��̼� ����
                float rate = 1.0 - i.uv.y;
                fixed4 col = lerp(_BottomColor, _TopColor, rate);

                // Fog ����
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
