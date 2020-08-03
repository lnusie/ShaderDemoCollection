Shader "X_Shader/PostEffect/KawaseBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelOffset("PixelOffset",Float) = 1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            NAME "PostEffect/KawaseBlur"
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

            sampler2D _MainTex;
            fixed4 _MainTex_TexelSize;
            float _PixelOffset;

            fixed4 KawaseSample(float2 uv, fixed texelSize, float pixelOffset)
            { 
                half4 col = half4(0,0,0,0);
                col += tex2D(_MainTex,uv + fixed2(-0.5 * pixelOffset, 0.5 * pixelOffset) * texelSize);    
                col += tex2D(_MainTex,uv + fixed2(-0.5 * pixelOffset, -0.5 * pixelOffset) * texelSize);    
                col += tex2D(_MainTex,uv + fixed2(0.5 * pixelOffset, 0.5 * pixelOffset) * texelSize);    
                col += tex2D(_MainTex,uv + fixed2(0.5 *pixelOffset, -0.5 * pixelOffset) * texelSize);    
                col *= 0.25;
                return col;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            } 


            fixed4 frag (v2f i) : SV_Target
            {
                fixed texelSize = _MainTex_TexelSize.x;
                fixed4 col = KawaseSample(i.uv, texelSize, _PixelOffset); 
                return col;
            }
            ENDCG
        }
    }
}
