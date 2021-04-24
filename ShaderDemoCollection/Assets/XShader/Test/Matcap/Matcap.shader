Shader "X_Shader/Test/Matcap"

{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _NormalMap ("NormalMap", 2D) = "black" {}
        _BumpScale ("BumpScale", Range(1, 20)) = 1

        _MatcapTex ("MatcapTex", 2D) = "black" {}

        _Color ("Color", Color) = (1,1,1,1)
        _Gloss ("Gloss", Range(1, 20)) = 1
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
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 tangent : TEXCOORD3;
                float3 binormal : TEXCOORD4;
                float2 matcap_uv : TEXCOORD5;
            };

            sampler2D _MainTex;
            sampler2D _NormalMap;
            sampler2D _MatcapTex;
            
            fixed _BumpScale;
            float _Gloss;

            half4 _Color;

            float3 GetTangentSpaceNormal(v2f i)
            {
                // //Unity 使用DXT5nm存储法线图,wy分量存储法线的xy部分，z值则由xy计算
                // normal.xy = tex2D(_NormalMap, i.uv).wy * 2 - 1; //[0,1] -> [-1,1]
                // normal.xy *= _BumpScale;
                // normal.z = sqrt(1 - saturate(dot(i.normal.xy, i.normal.xy)));
                // normal = normalize(normal);
                //UnityStandardUtils定义了UnpackScaleNormal执行相同操作
                float3 normal = float3(0, 0, 1);
                //#if defined(_NORMAL_MAP)
                normal = UnpackScaleNormal(tex2D(_NormalMap,i.uv.xy),_BumpScale);
                //#endif 
                return normal;
            }

            float3 CreateBinormal(float3 normal, float3 tangent, float binormalSign)
            {
                return cross(normal,tangent.xyz) * binormalSign * unity_WorldTransformParams.w;        
            }
            
            void InitFragNormal(inout v2f i)
            {       
                float3 tangentSpaceNormal =  GetTangentSpaceNormal(i);
                float3 binormal = i.binormal;
                i.normal = normalize(
                    tangentSpaceNormal.x * i.tangent +
                    tangentSpaceNormal.y * binormal +
                    tangentSpaceNormal.z * i.normal
                );
            } 

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.normal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
                o.vertex = UnityObjectToClipPos(v.vertex);
                float3 worldTangent = normalize(mul(v.tangent.xyz, (float3x3)unity_WorldToObject));
                o.binormal = CreateBinormal(o.normal, worldTangent, v.tangent.w);
                o.tangent = worldTangent;
                o.uv = v.uv;
                
                
                //float3 viewspace_normal = mul((float3x3)UNITY_MATRIX_V, o.normal); //UNITY_MATRIX_V不含非等比例缩放，所以也可以直接用来变换方向

                float3 viewspace_normal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                o.matcap_uv = viewspace_normal.xy * 0.5 + 0.5;
                o.normal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = 1;
                float4 albedo = tex2D(_MainTex, i.uv) * _Color;
                InitFragNormal(i);
                float3 worldNormal = normalize(i.normal);
                //worldNormal = GetTangentSpaceNormal(i);

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
                
                float halfNdotL = dot(worldNormal, lightDir) * 0.5f + 0.5f;
                float3 diffuse = halfNdotL * albedo.rgb * _LightColor0.rgb;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;//环境光
                ambient = 0;
                
                float3 halfDir = normalize(viewDir + lightDir);
                float ndoth = max(0, dot(worldNormal, halfDir));

                float3 specular = pow(ndoth, _Gloss * 10) * _LightColor0.rgb;

                specular = tex2D(_MatcapTex, i.matcap_uv);

                col.rgb = specular;

                return col;
            }
            ENDCG
        }
    }
}
