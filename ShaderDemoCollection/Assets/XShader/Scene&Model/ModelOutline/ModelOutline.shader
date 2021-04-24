Shader "X_Shader/Model/ModelOutline"

{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _OutlineWidth ("OutlineWidth", Range(0, 10)) = 1
        [HideInInspector]_SrcBlend ("_SrcBlend", Float) = 1
		[HideInInspector]_DstBlend ("_DstBlend", Float) = 0
        [HideInInspector] _ZWrite ("_ZWrite", Float) = 1
    }
    SubShader
    {
        Pass
        {
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            Cull Back 

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
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            half4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = 1;
                float4 albedo = tex2D(_MainTex, i.uv) * _Color;
                float3 worldNormal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0);
                
                float h = dot(worldNormal, lightDir) * 0.5f + 0.5f;
                float3 diffuse = h * albedo.rgb * _LightColor0.rgb;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;//环境光
                col.rgb = diffuse + ambient;
                
                // float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                // float ndotv = dot(worldNormal, viewDir);
                // //float t = 1 - ndotv;
                // col.rgb *= step(0.2, ndotv);
                return col;
            }
            ENDCG
        }

        Pass
        {
            Name "OUTLINE"
            ZWrite On
            Cull front 

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
            };

            float _OutlineWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                //把法线转换到视图空间
                float3 vnormal = mul((float3x3)UNITY_MATRIX_IT_MV,v.normal);
                //把法线转换到投影空间,相当于 TransformViewToProjection(vnormal.xy)执行了相应的操作
                float2 extendDir = normalize(mul((float2x2)UNITY_MATRIX_P, vnormal.xy));
                
                //朝法线方向外扩

                
                o.pos.xy += extendDir.xy / _ScreenParams.xy * _OutlineWidth * o.pos.w;
 
                o.uv = v.uv;
                return o;
            }
            sampler2D _MainTex;
            half4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                return 0;
            }
            ENDCG
        }
    }
}
