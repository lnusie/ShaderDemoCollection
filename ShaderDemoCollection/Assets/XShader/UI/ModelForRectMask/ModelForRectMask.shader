// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "X_Shader/UI/ModelForRectMask"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_ClipRect("ClipRect", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Pass
        {
        	Tags{
        		"LightMode" = "ForwardBase"
        	}

            Blend SrcAlpha OneMinusSrcAlpha


            CGPROGRAM

            #pragma target 3.0

            #pragma multi_compile __ UI_CLIP_RECT


            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;

		    float4 _ClipRect;

            half4 _Color;
          
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityUI.cginc"

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
                float4 screenPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;

            };

            float Get2DClipping(float2 pos, float4 rect)
            {
                float r1 = step(rect.x, pos.x);
                float r2 = step(rect.y, pos.y);
                float r3 = step(pos.x, rect.z);
                float r4 = step(pos.y, rect.w);
                return r1 * r2 * r3 * r4;
            }

            v2f vert (appdata v)
            {
                v2f o;
                //o.worldPos = mul(unity_ObjectToWorld, v.vertex);

                //o.objPos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
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
                #if defined(UI_CLIP_RECT)
                    // //0~1
                    float2 viewportPos = i.screenPos.xy / i.screenPos.w;
                    float2 screenPos = float2(viewportPos.x * _ScreenParams.x, viewportPos.y * _ScreenParams.y);
                    //要实现半透明效果：调整渲染序列，设置混合模式
                    c.a *= Get2DClipping(screenPos, _ClipRect);
                    //clip(Get2DClipping(screenPos, _ClipRect) - 0.1);
                #endif 

                return c;
            }
            ENDCG
        }
    }
}
