Shader "X_Shader/Scene/WaterPlane"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _Wave0("Wave0", Vector) = (0,0,0,0)
        _WaveRange0("Wave0", float) = 1
        _RippleTex("Ripple Tex", 2D) = "black" {}
    }
    SubShader
    {

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
                float4 screenPos : TEXCOORD1;
                float4 worldPos : TEXCOORD2;

            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _RippleTex;
            
            float4 _Wave0;
            float _WaveRange0;
            float4 _RippleTex_ST;
            float _RippleScale;

            float GetWaveValue(float x, float y)
            {
                return ((sin(x * 20 + (3.14/2))+1) / max(1, abs(x *8))) * 0.5 * y;
            } 
            
            float GetWavePower(v2f i, float4 wave, float range, inout float3 offset)
            {
                float4 worldPos = i.worldPos;
                float3 dir = worldPos.xyz - wave.xyz;
                float dis = length(dir);
                float powerFactor = (dis / range);
                powerFactor = max(0, 1 - powerFactor);

                float waveRange = 0.3;
                float value = (dis - wave.w)/waveRange;
                value = GetWaveValue(value, powerFactor);
                offset = normalize(dir);
                return value;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float4 worldPos = i.worldPos;
                float4 screenPos = i.screenPos;

                //采样涟漪贴图
                float4 ripple = tex2D(_RippleTex, TRANSFORM_TEX(uv, _RippleTex));
                //frac: 取小数部分, 这里如果把+ ripple.a 放到后面，出现和消失则衔接不上
                float time = frac(_Time.y * 0.5 + ripple.a);
                //r通道的圆有渐变，从而使ripple_value随时间变化
                float ripple_value = (time - 1) + ripple.r;
                ripple_value = clamp(ripple_value * 9.0, 0.0, 5.0);
                
                float ripplePower = sin(ripple_value * 3.14) / (ripple_value + 1);
                ripplePower *= (1 - time);
                ripple.yz = ripple.yz * 2 - 1;
                screenPos.xy += ripple.yz  * ripplePower;
                fixed4 col = tex2Dproj(_MainTex, screenPos);
                return col;
            }
            ENDCG
        }
    }
}
