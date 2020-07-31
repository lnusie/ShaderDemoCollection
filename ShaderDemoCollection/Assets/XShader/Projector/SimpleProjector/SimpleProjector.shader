
Shader "X_Shader/Projector/SimpleProjector"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Pass
        {
            CGPROGRAM

            #pragma target 3.0

            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4x4 unity_Projector; // 由投影器输入此矩阵

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
                float4 projPos : TEXCOORD3;
            };
           
            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.projPos = mul(unity_Projector, v.vertex);
                return o;   
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2Dproj(_MainTex, i.projPos);
                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
