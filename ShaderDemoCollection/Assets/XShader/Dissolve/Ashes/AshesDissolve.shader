// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "X_Shader/Dissolve/AshesDissolve"
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
        _EdgeLength("_EdgeLength", Range(0.01,1)) = 0.1
        _DissolveMinValue("DissolveMinValue", Float) = 0
        _DissolveMaxValue("DissolveMaxValue", Float) = 1
        _IsHorizontal("IsHorizontal", Float) = 0
        _DissolveValue("DissolveValue", Range(0,1)) = 0
        _AshesWidth("AshesWidth",Range(0,1)) = 0.2
        _AshesFloatDirection("Float Direction", Vector) = (0,0,0,0) 
        _AshesFloatSpeed("AshesFloatSpeed",Float) = 1
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
            fixed _EdgeLength;
            fixed4 _AshesFloatDirection;

            float _DissolveMinValue;
            float _DissolveMaxValue;
            float _DissolveValue;
            float _AshesWidth;
            float _AshesFloatSpeed;
            float _AshesDensity;

            float _IsHorizontal;

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
                float3 worldPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };
           
            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
               
                float4 localFloatDir = mul(unity_WorldToObject, _AshesFloatDirection);
                localFloatDir = normalize(localFloatDir);
                float pos = lerp(v.vertex.x, v.vertex.y, _IsHorizontal);
                float4 distanceScale = length(pos - _DissolveMinValue) / length(_DissolveMaxValue - _DissolveMinValue);
                float floatStrength = saturate(distanceScale - _DissolveValue - _EdgeLength);
                floatStrength = floatStrength * _AshesFloatSpeed;
                v.vertex += localFloatDir * floatStrength; 
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
                o.uv = v.uv;
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

                float4 objPos = mul(unity_WorldToObject, i.worldPos);
                fixed4 noise = tex2D(_NoiseTex, i.uv);
                float2 pos = lerp(objPos.y, objPos.x, _IsHorizontal);
                fixed diff = length(pos - _DissolveMinValue) / length(_DissolveMaxValue - _DissolveMinValue);
                diff = _DissolveValue - diff;
                diff *= noise.y * noise.x * 2;
                fixed dissolveValue = diff / _EdgeLength;
                dissolveValue = saturate(dissolveValue);
                
                //避免取到颜色很奇怪的边界值
                fixed gradientUV = clamp(dissolveValue,0.1,0.9); 
                fixed4 edgeColor = tex2D(_GradientTex, gradientUV);

                c = lerp(edgeColor, c, dissolveValue + 0.1);
              
                fixed4 ashesCol = 0; 

                float ashesValue = saturate(-diff/_AshesWidth) * 2;
                c = lerp(c, ashesCol, ashesValue);
                
                if(ashesValue > 0.99)
                {
                    fixed4 ashesNoise = tex2D(_NoiseTex, i.uv);
                    clip(ashesNoise.y * ashesNoise.z - 0.1);
                }

                clip(diff + _AshesWidth);
                return c;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
