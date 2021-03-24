// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "X_Shader/ModelEffect/ModelScanLine"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ScanLineTex("Scan Line Tex", 2D) = "white" {}
        _ScanSpeed("Scan Speed", Float) = 1
        _EdgeColor("Edge Color", Color) = (1,1,1,1)
        _EdgeWidth("Edge Width", Range(0,1)) = 0.1
    }
    SubShader
    {
        LOD 200
        Pass
        {
        	Tags{
        		"LightMode" = "ForwardBase"
        	}

            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma target 3.0

            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _ScanLineTex;

            half4 _Color;
            half4 _EdgeColor;
            float _ScanSpeed;
            half _EdgeWidth;
            
          
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
                float3 worldNormal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.objPos = v.vertex;
                //o.objPos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //半兰伯特光照
                fixed4 c = 1;
                fixed4 albedo = tex2D (_MainTex, i.uv) * _Color;
                float3 worldNormal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0);
                float h = dot(worldNormal, lightDir) * 0.5 + 0.5; 
                fixed3 diffuse =_LightColor0.rgb * albedo.rgb * h;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;//环境光
                c.rgb = diffuse + ambient;
                
                half2 uv = i.uv;
                uv.y = i.worldPos.y + _ScanSpeed * _Time.y;
                float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos);
                float vdotn = dot(worldNormal, viewDir);
                vdotn = abs(vdotn);
                
                c.rgb = lerp(c.rgb, _EdgeColor, saturate(_EdgeWidth - vdotn));

                half4 scanCol = tex2D(_ScanLineTex, uv);
                c.a = scanCol.r;
                return c;
            }
            ENDCG
        }
    }
}
