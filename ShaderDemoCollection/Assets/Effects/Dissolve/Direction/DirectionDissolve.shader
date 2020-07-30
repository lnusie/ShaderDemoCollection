// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "X_Shader/Effects/DirectionDissolve"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _NoiseTex("Noise Tex", 2D) = "white" {}
        _GradientTex("Gradient Tex", 2D) = "white" {}
        _EdgeColor1("EdgColor 1", Color) = (1,1,1,1)
        _EdgeColor2("EdgColor 2", Color) = (1,1,1,1)
        _EdgeLength("_EdgeLength", Range(0.01,1)) = 0.1
        _DissolveMinValue("DissolveMinValue", Float) = 0
        _DissolveMaxValue("DissolveMaxValue", Float) = 0
        _DissolveValue("DissolveValue", Range(0,1)) = 0
        _IsHorizontal("IsHorizontal", Float) = 0
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

            float _DissolveMinValue;
            float _DissolveMaxValue;
            float _DissolveValue;

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
                float4 objPos : TEXCOORD1;
                float4 normal : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.objPos = v.vertex;
                //o.objPos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = mul(v.normal, unity_WorldToObject);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //半兰伯特光照
                fixed4 c = 1;
                fixed4 albedo = tex2D (_MainTex, i.uv) * _Color;
                float4 worldNormal = normalize(i.normal);
                float4 lightDir = normalize(_WorldSpaceLightPos0);
                float h = dot(worldNormal, lightDir) * 0.5 + 0.5;
                fixed3 diffuse =_LightColor0.rgb * albedo.rgb * h;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;//环境光
                c.rgb = diffuse + ambient;

                fixed4 noise = tex2D(_NoiseTex, i.uv);
                float pos = lerp(i.objPos.y, i.objPos.x, _IsHorizontal);

                fixed diff = (pos - _DissolveMinValue) / (_DissolveMaxValue - _DissolveMinValue);
                diff = _DissolveValue - diff;
                diff *= noise.y * noise.x * 2;
                fixed dissolveValue = diff / _EdgeLength;
                dissolveValue = saturate(dissolveValue);
                
                //用渐变纹理提供更丰富的细节
                //edgColor = lerp(_EdgeColor1, _EdgeColor2, saturate(t));
                fixed4 edgeColor = tex2D(_GradientTex, dissolveValue);
               
                c = lerp(edgeColor, c, dissolveValue);
            
                clip(diff - 0.01);
                return c;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
