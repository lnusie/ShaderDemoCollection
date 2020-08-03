Shader "X_Shader/PostEffect/MotionBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize("BlurSize",Float) = 1



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

            sampler2D _MainTex;
            half4 _MainTex_TexelSize;
            float _BlurSize;
            sampler2D _CameraDepthTexture;
            float4x4 _PreviousViewProjectionMatrix;
            float4x4 _CurViewProjectionInverseMatrix;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv_depth : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv_depth = v.uv;
                //类似DirectX的平台坐标原点在左上
                #if UNITY_UV_STARTS_AT_TOP 
                if(_MainTex_TexelSize.y < 0) 
                {
                    o.uv_depth.y = 1 - o.uv_depth.y; 
                }

                #endif 

                return o;
            }

            //[0,1] -> [-1,1]
            float TransferNum(float num)
            {
                return num * 2 - 1;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv_depth);
                float4 ndcPos = float4(i.uv.x*2-1,i.uv.y*2-1,depth*2-1,1);

                float4 D = mul(_CurViewProjectionInverseMatrix, ndcPos);

                float4 worldPos = D / D.w;// why？ 

                float4 previousNdcPos = mul(_PreviousViewProjectionMatrix,worldPos);

                previousNdcPos /= previousNdcPos.w;

                float2 v = (ndcPos.xy - previousNdcPos.xy) * 0.5;
                // v.x = clamp(0,0.2,v.x);
                // v.y = clamp(0,0.2,v.y);

                fixed4 col = 0;
                int _Interaton = 3; 
                for(int t = 0;t<_Interaton;t++)
                {
                    col += tex2D(_MainTex, i.uv + t * v * _MainTex_TexelSize.x * _BlurSize);
                } 
                col /= _Interaton;

                // just invert the colors 
                return fixed4(col.rgb,1);
            }
            ENDCG
        }
    }
}
