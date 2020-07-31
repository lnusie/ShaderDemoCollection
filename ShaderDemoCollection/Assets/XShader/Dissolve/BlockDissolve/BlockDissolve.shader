
Shader "X_Shader/Dissolve/BlockDissolve"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(1,255)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _NoiseTex("Noise Tex", 2D) = "white" {}
        _GradientTex("Gradient Tex", 2D) = "white" {}
        _EdgeColor1("EdgColor 1", Color) = (1,1,1,1)
        _EdgeColor2("EdgColor 2", Color) = (1,1,1,1)
        _EdgeWidth("EdgeWidth", Range(0.01,1)) = 0.1
        _FocusPos("Focus Pos", Vector) = (0.5,0.5,0,0)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Pass
        {
        	Tags{
        		"LightMode" = "ForwardBase"
        	}

            CGPROGRAM

            #pragma target 3.0

            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            sampler2D _GradientTex;
            half _Glossiness;
            half _Metallic;
            fixed4 _Color;
            float _DissolveStrength;
            fixed4 _EdgeColor1;
            fixed4 _EdgeColor2;
            fixed _EdgeWidth;
            fixed4 _FocusPos;

            fixed _G_DissolveRadius;
            float _G_DissolveDistance;
            float _G_DissolveHeight;

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float4 tangentPos : TEXCOORD4;
            };
           
            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                //根据裁剪坐标计算屏幕坐标:
                //1.先进行齐次除法，得到[-1,1]的NDC坐标，
                //2.再将其范围映射到[0,1]的视口空间下
                //3.再根据屏幕分辨率计算出屏幕坐标
                //必须先进行齐次除法再插值
                // o.screenPos.xy = o.vertex.xy * 0.5 / o.vertex.w + 0.5;
                // o.screenPos.zw = o.vertex.zw;
                o.screenPos = ComputeScreenPos(o.vertex);
                
                o.normal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
                o.uv = v.uv;
                float3 binormal = cross(normalize(v.normal), normalize(v.tangent.xyz)) * v.tangent.w;
                float3x3 obj2Tangent = float3x3(v.tangent.xyz, binormal, v.normal.xyz);
                o.tangentPos.xyz = mul(obj2Tangent, v.vertex);
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;//环境光

                //半兰伯特光照
                fixed4 c = 1;
                fixed4 albedo = tex2D (_MainTex, i.uv) * _Color;
                float3 worldNormal = normalize(i.normal);
                float3 lightDir = normalize(_WorldSpaceLightPos0);
                float h = dot(worldNormal, lightDir) * 0.5 + 0.5;
                fixed3 diffuse =_LightColor0.rgb * albedo.rgb * h;
                
                fixed3 reflectDir = normalize(reflect(-lightDir, worldNormal));
                fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
                fixed3 specular = _LightColor0 * pow(saturate(dot(reflectDir, viewDir)),_Glossiness);
                c.rgb = diffuse + ambient + specular; 
                
                //0~1
                float4 viewportPos = i.screenPos / i.screenPos.w;
                //viewportPos.x = _ScreenParams.x / _ScreenParams.y * viewportPos.y;
                float distance = length(viewportPos.xy - _FocusPos.xy);
                float dissolveScale = distance / max(0.001, _G_DissolveRadius);
                //c.rgb = lerp(0, c.rgb, saturate(dissolveScale));
                
                float viewDistance = length(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
                //不需要溶解时 needDissolve 为 1
                float needDissolve = step(0, viewDistance - _G_DissolveDistance);
                float heightScale = i.worldPos.y / (_G_DissolveHeight + 0.01);
                fixed4 noise = tex2D(_NoiseTex, i.tangentPos.xy);
                //return noise;
                //clip(noise.x * noise.y - (1 - dissolveScale));
                clip((dissolveScale - noise.x * heightScale) + needDissolve);

                return c;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
