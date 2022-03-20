Shader "X_Shader/Test/DecodeDepthNormalMap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_DepthNormalMap("Texture", 2D) = "white" {}

    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
			sampler2D _DepthNormalMap;
            
            sampler2D _CameraDepthTexture;//unity自动传入的深度图
            sampler2D _CameraDepthNormalsTexture;//unity自动传入的深度法线图


            fixed4 frag (v2f i) : SV_Target
            {
                // fixed4 col = tex2D(_MainTex, i.uv);
                // // just invert the colors
                // col.rgb = 1 - col.rgb;


                // float depth;
                // float3 normal;
                // DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), depth, normal);

                //单纯获取深度值
                // float depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screen));  
                // float depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, float4(depth_uv, 0, 0)));  
                // depth = LinearEyeDepth(d2); [N,F]
                // depth = Linear01Depth (depth); [0,1]

                float depth;
                float3 normal;
                DecodeDepthNormal(tex2D(_MainTex, i.uv), depth, normal);
                return float4(normal.x, normal.y, normal.z, depth);
            }
            ENDCG
        }
    }
}
