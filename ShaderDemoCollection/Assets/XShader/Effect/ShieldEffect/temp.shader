Shader "X_Shader/Effect/ShieldEffect"

{
    Properties
    {
        _MainTex ("GridMask", 2D) = "white" {}
        _NoiseTex("NoiseTex", 2D) = "black" {}
        _Color ("Color", Color) = (1,1,1,1)

        _EdgeColor("EdgeColor", Color) = (1,1,1,1)
        _EdgeRange("EdgeRange", Range(0,2)) = 1

        _IntersectColor("IntersectColor", Color) = (1,1,1,1)
        _IntersectWidth("IntersectWidth", Range(0,10)) = 1

        _DisturbWidth("_DisturbWidth", Range(0.01,10)) = 1
        _DisturbStrength("DisturbStrength", Range(0.01,10)) = 1
        _DisturbSpeedFactor("DisturbSpeedFactor", Range(0,1)) = 1
        _DisturbColor ("Color", Color) = (1,1,1,1)


        _HitMaxWaveWidth("HitMaxWaveWidth", Range(0,2)) = 1
        // _HitMinWaveWidth("HitMinWaveWidth", Range(0,2)) = 1
        _HitWaveWidth("HitWaveWidth", Range(0,2)) = 1
        _HitPoint1("HitPoint1", Vector) = (0,0,0,0)
        _HitWaveValue("HitWaveValue", Range(0,2)) = 1


    }
    SubShader
    {
        GrabPass
        {
            "_GrabTempTex"                
        }
        
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off 
            
            Tags
            {
                "RenderType" = "Transparent" 
                "Queue" = "Transparent"
            }

            CGPROGRAM

            #pragma target 3.0

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

           struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float4 worldPos : TEXCOORD2;
                float4 projPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            sampler2D _GrabTempTex;
            sampler2D _NoiseTex;

            float4 _MainTex_ST;
            half4 _Color;

            half4 _IntersectColor;
            float _IntersectWidth;

            half4 _EdgeColor;
            float _EdgeRange;
            
            half4 _DisturbColor;
            float _DisturbWidth;
            float _DisturbStrength;
            float _DisturbSpeedFactor;

            float _HitMaxWaveWidth;
            float _HitMinWaveWidth;
            float _HitWaveWidth;
            float4 _HitPoint1;

            float _HitWaveValue;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                o.projPos = ComputeScreenPos(o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {   
                //扰动
                float4 disturbOffset = tex2D(_NoiseTex, i.uv * _DisturbWidth + _Time.x * _DisturbSpeedFactor);

                float waveValue = _HitPoint1.w;

                //受击波动
                float hitDist = distance(i.worldPos.xyz, _HitPoint1.xyz) + disturbOffset.x * 0.3;
                hitDist = 1 - hitDist;

                //基本色
                fixed4 baseColor = tex2D(_MainTex, i.uv + disturbOffset * 0.00 + _Time.x * _DisturbSpeedFactor) * 1.3;
                
                //视觉方向决定透明度
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos); 
                float3 worldNormal = normalize(i.worldNormal);
                float ndotv = dot(worldNormal, viewDir);
                fixed t = step(0, ndotv);           
                float transparency = (1 - t) * lerp(0.5, 0.1, abs(ndotv)) + t * lerp(0.6, 0.1, ndotv);

                //深度决定相交程度
                float depth = i.projPos.z;
                float sceneDepth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos));
                sceneDepth = LinearEyeDepth(sceneDepth);
                float intersect = 1 - saturate((sceneDepth - depth)/_IntersectWidth);

                fixed4 color = baseColor;

                fixed4 addColor = lerp(_Color, _IntersectColor * 2, intersect);
                color.rgb += addColor.rgb + disturbOffset * _DisturbColor.rgb * _DisturbStrength;

                //边缘光
                float k = _EdgeRange - abs(ndotv);              
                color.rgb = lerp(color.rgb, _EdgeColor, saturate(k));
                color.a = transparency;

             
                return color;
            }
            ENDCG
        }

    }
}
