Shader "X_Shader/PostEffect/GrainyBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _IterationCount("IterationCount",Float) = 1
        _BlurRadio("BlurRadio",Float) = 1
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _IterationCount;
            float4 _MainTex_TexelSize;

            float GetRandom(float2 vec)
            { 
                return sin(dot(vec, half2(1233.224, 1743.335))); 
            }

            fixed4 frag (v2f i) : SV_Target
            {
                const fixed blurDirsX[4] = {1,-1,-1,1};
                const fixed blurDirsY[4] = {1,1,-1,-1};
                float2 uv = i.uv;
                float random = GetRandom(uv);
                float2 offset = 0;
                fixed4 col = 0;
                for(int t=0; t<=_IterationCount; t++) 
                { 
                    int k = t % 4; 
                    //float2 dir = 1;
                    offset.x = frac(random * 2145.459 * blurDirsX[k]);
                    offset.x = offset.x * 2 - 1; 
                    offset.y = frac(random * 2145.459 * blurDirsY[k]);
                    offset.y = offset.y * 2 - 1;

                    col += tex2D(_MainTex, uv + offset * _MainTex_TexelSize.x * 10);
                } 
                col = col/_IterationCount;

                return col;
            }
            ENDCG
        }
    }
}
