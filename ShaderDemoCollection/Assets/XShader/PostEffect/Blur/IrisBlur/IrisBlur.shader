Shader "X_Shader/PostEffect/IrisBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CenterPos("CenterPos",Vector) = (0.5,0.5,1,1)
        _VisibleRadius("VisibleRadius",Float) = 1
        _BrightFactor("BrightFactor",Float) = 1
        _PixelOffset("_PixelOffset",Float) = 1
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _CenterPos;
            float _VisibleRadius;
            float _BrightFactor;
            float4 _MainTex_TexelSize;
            float _PixelOffset;

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


            fixed GetCircleMask(fixed2 uv)
            {
                //fixed2 pos = uv * 2 - 1;// [0,1] -> [-1,1]
                fixed2 dir = uv - _CenterPos.xy;
                dir.x *= _ScreenParams.x/_ScreenParams.y;
                fixed dis = length(dir); 
                fixed intension = clamp((_VisibleRadius - dis)*_BrightFactor,0,1); 
                return intension;
            }

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
                // just invert the colors
                half maskValue = 1 - GetCircleMask(i.uv);
                fixed4 col = KawaseSample(i.uv,_MainTex_TexelSize.x, maskValue * _PixelOffset * 2);
                return col; 
            }
            ENDCG
        }
    }
}
