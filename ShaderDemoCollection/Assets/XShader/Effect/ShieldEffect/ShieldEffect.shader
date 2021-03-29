Shader "X_Shader/Effect/ShieldEffect"

{
    Properties
    {
        _MainTex ("GridMask", 2D) = "white" {}
        _NoiseTex("NoiseTex", 2D) = "black" {}
        _Color ("Color", Color) = (1,1,1,1)

        _EdgeColor("EdgeColor", Color) = (1,1,1,1)
        _FresnelScale("FresnelScale",Range(0,1)) = 0.5
        _FresnelRange("FresnelRange",Range(0,10)) = 5

        _IntersectColor("IntersectColor", Color) = (1,1,1,1)
        _IntersectWidth("IntersectWidth", Range(0,10)) = 1

        _DisturbWidth("_DisturbWidth", Range(0.01,10)) = 1
        _DisturbStrength("DisturbStrength", Range(0.01,10)) = 1
        _DisturbSpeedFactor("DisturbSpeedFactor", Range(0,1)) = 1
        _DisturbColor ("Color", Color) = (1,1,1,1)

        _HitColor ("HitColor", Color) = (1,1,1,1)
        _HitRimWidth("HitRimWidth", Range(0,5)) = 1
        _DentRange("DentRange", Range(0,2)) = 0.1
        _Softness("Softness", Range(0, 2)) = 1

    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent" 
            "Queue" = "Transparent"
            "IgnoreProjector"="True" 
        }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off 

        CGINCLUDE

        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        
        sampler2D _MainTex;
        sampler2D _CameraDepthTexture;
        sampler2D _NoiseTex;

        float4 _MainTex_ST;
        half4 _Color;

        half4 _IntersectColor;
        float _IntersectWidth;

        half4 _EdgeColor;
        float _EdgeRange;

        float _FresnelScale;
        float _FresnelRange;
        
        half4 _DisturbColor;
        float _DisturbWidth;
        float _DisturbStrength;
        float _DisturbSpeedFactor;

        float _HitRimWidth;
        half4 _HitColor;

        float4 _HitPoint0;
        float4 _HitPoint1;
        float4 _HitPoint2;
     
        float _DentRange;
        float _Softness;
        
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
        
        float _GetHitStrength(float4 hitpoint, float3 worldPos,  float maxHitDist, float disturbOffset)
        {
            float hitPower = hitpoint.w;
            float hitDist = distance(worldPos, hitpoint.xyz) + disturbOffset;
            hitDist = clamp(hitDist, 0, maxHitDist);
             //越靠近中心强度越大 
            float hitStrength = (maxHitDist - hitDist) / (maxHitDist + 0.001);
            return hitStrength * hitPower * step(0, hitStrength);
        }

        float GetVertHitStrength(float3 vertLocalPos)
        {
            float4 localHitPos0 = mul(unity_WorldToObject, float4(_HitPoint0.xyz,1));
            localHitPos0.w = _HitPoint0.w;
            float4 localHitPos1 = mul(unity_WorldToObject, float4(_HitPoint0.xyz,1));
            localHitPos1.w = _HitPoint1.w;
            float4 localHitPos2 = mul(unity_WorldToObject, float4(_HitPoint0.xyz,1));
            localHitPos2.w = _HitPoint2.w;
            float hitStrength0 = _GetHitStrength(localHitPos0, vertLocalPos, _DentRange, 0);
            float hitStrength1 = _GetHitStrength(localHitPos1, vertLocalPos, _DentRange, 0);
            float hitStrength2 = _GetHitStrength(localHitPos2, vertLocalPos, _DentRange, 0);
            float totalStrength = hitStrength0 + hitStrength1 + hitStrength2;
            return totalStrength;
        }
        
      
        v2f BaseVert(appdata v)
        {
            v2f o;
            o.uv = v.uv;
            float hitStrength = GetVertHitStrength(v.vertex.xyz);
            v.vertex.xyz -= hitStrength * v.normal * _Softness * 0.1;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex);
            o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
            o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
            o.projPos = ComputeScreenPos(o.pos);
            COMPUTE_EYEDEPTH(o.projPos.z); 
            return o;
        }


        float GetFragHitStrength(v2f i, float3 disturbOffset)
        {
            float hitStrength0 = _GetHitStrength(_HitPoint0, i.worldPos.xyz, _HitRimWidth, disturbOffset.x);
            float hitStrength1 = _GetHitStrength(_HitPoint1, i.worldPos.xyz, _HitRimWidth, disturbOffset.y);
            float hitStrength2 = _GetHitStrength(_HitPoint2, i.worldPos.xyz, _HitRimWidth, disturbOffset.z);
            return hitStrength0 + hitStrength1 + hitStrength2;
        }

        float GetTransparency(v2f i, out float ndotv)
        {
            //视觉方向决定透明度
            float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos); 
            float3 worldNormal = normalize(i.worldNormal);
            ndotv = dot(worldNormal, viewDir);
            fixed t = step(0, ndotv);           
            return (1 - t) * lerp(0.5, 0.1, abs(ndotv)) + t * lerp(0.6, 0.1, ndotv);
        }

        fixed4 BaseFrag (v2f i) : SV_Target
        {   
            float hitStrength = GetFragHitStrength(i, 0) * 0.8;

            //扰动
            float4 disturbOffset = tex2D(_NoiseTex, i.uv * _DisturbWidth + _Time.x * _DisturbSpeedFactor * (1 + 0.01 * hitStrength));

            
            //基本色
            fixed4 baseColor = tex2D(_MainTex, i.uv + _Time.x * _DisturbSpeedFactor) * 1.3;
               
            float ndotv;
            float transparency = GetTransparency(i, ndotv);

            //深度决定相交程度
            float depth = i.projPos.z;
            float sceneDepth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos));
            sceneDepth = LinearEyeDepth(sceneDepth);
            float intersect = 1 - saturate((sceneDepth - depth)/_IntersectWidth);

            fixed4 color = baseColor;

            fixed4 addColor = lerp(_Color, _IntersectColor * 2, intersect);
            color.rgb += addColor.rgb + disturbOffset * _DisturbColor.rgb * _DisturbStrength * 1.5; 

            float fresnel = _FresnelScale + (1 - _FresnelScale) * pow(1 - ndotv, _FresnelRange);       
            color.rgb = lerp(color.rgb, _EdgeColor, saturate(fresnel));
            color.a = transparency;

            color = lerp(color, _HitColor, saturate(hitStrength));
            return color;
        }

        ENDCG
      
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex BaseVert
            #pragma fragment BaseFrag
            ENDCG
        }
    }
    
       
}
