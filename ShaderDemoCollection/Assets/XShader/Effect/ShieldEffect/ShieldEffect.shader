Shader "X_Shader/Effect/ShieldEffect"

{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
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
                float4 screenPos : TEXCOORD3;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.screenPos = ComputeScreenPos(o.pos);
                COMPUTE_EYEDEPTH(o.screenPos.z);
                return o;
            }

            sampler2D _MainTex;
            half4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {   
                fixed4 col = _Color;                 
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos); 
                float3 worldNormal = normalize(i.worldNormal);
                float ndotv = dot(worldNormal, viewDir);
                fixed t = step(0, ndotv);           
                col.a = (1 - t) * lerp(0.1, 0.3, abs(ndotv)) + t * lerp(0.8, 0.1, ndotv);
                return col;
            }
            ENDCG
        }
    }
}
