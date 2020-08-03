Shader "X_Shader/PostEffect/RadialBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _InputParams("InputParams",Vector) = (0.5,0.5,1,1) //xy ： 中心点  z : 模糊半径
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Off

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

            sampler2D _MainTex;
            float4 _InputParams;
            float4 _MainTex_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 uv = i.uv;
                fixed2 dir = uv - _InputParams.xy;
                dir.x *= _ScreenParams.x/_ScreenParams.y;
                dir = normalize(dir) * _InputParams.z * _MainTex_TexelSize.x;
                fixed4 col = 0;
                for(int i =0;i<4;i++)
                {
                    col += tex2D(_MainTex, uv);
                    uv += dir;
                }
                col /= 4;
                return col;
            }
            ENDCG
        }
    }
}
