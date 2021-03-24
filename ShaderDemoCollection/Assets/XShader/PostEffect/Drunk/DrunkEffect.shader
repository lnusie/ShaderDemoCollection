Shader "X_Shader/PostEffect/DrunkEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OffsetFactor("OffsetFactor",float) = 1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            }; 

            float _Intensity;
            float _OffsetFactor;
             half4 _MainTex_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed2 offsetDir = 0;
                offsetDir.x = sin(_Time.y) * _OffsetFactor;
                offsetDir.y = sin(_Time.y + 10) * 0.5;
                fixed4 col2 = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x,_MainTex_TexelSize.x)*offsetDir);
                col.r = col2.r;
                fixed2 offsetDir2 = 0;
                offsetDir2.x = cos(_Time.y) * _OffsetFactor;
                offsetDir2.y = -cos(_Time.y + 10) * 0.5;
                fixed4 col3 = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x,_MainTex_TexelSize.x)*offsetDir2);
                col.g = col3.g;
                return col;
            }
            ENDCG
        }
    }
}
