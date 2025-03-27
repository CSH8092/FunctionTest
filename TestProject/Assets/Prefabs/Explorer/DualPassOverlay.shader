Shader "Custom/DualPassOverlay"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _FrontColor("Front Color", Color) = (1,1,1,1)
        _BackColor("Back Overlay Color", Color) = (1,1,1,0.5)
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            // �� Pass 1: �տ� �ִ� ��� (�⺻ ������ ����)
            Pass
            {
                Name "FrontOpaque"
                Tags { "LightMode" = "ForwardBase" }

                ZWrite On
                ZTest LEqual
                Cull Back
                Blend Off

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                fixed4 _FrontColor;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 tex = tex2D(_MainTex, i.uv);
                    return tex * _FrontColor;
                }
                ENDCG
            }

            // �� Pass 2: �ڿ� �ִ� ��� (�׻� �׷����� ������)
            Pass
            {
                Name "BackOverlay"
                Tags { "LightMode" = "AlwaysOverlay" }

                ZWrite Off
                ZTest Greater // �� ���� �ȼ����� �ڿ� �ִ� ��츸 ���
                Cull Back
                Blend SrcAlpha OneMinusSrcAlpha

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                fixed4 _BackColor;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 tex = tex2D(_MainTex, i.uv);
                    return tex * _BackColor;
                }
                ENDCG
            }
        }

            FallBack "Diffuse"
}
