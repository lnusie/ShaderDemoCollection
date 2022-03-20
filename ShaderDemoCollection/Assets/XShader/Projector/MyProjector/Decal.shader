Shader "X_Shader/Projector/Decal"
{
    Properties
    {
        _DecalTex ("Decal Texture", 2D) = "white" {}
        _DepthTex ("Depth Texture", 2D) = "white" {}

    }
    SubShader
    {
		ZWrite On
		ZTest On
		Fog { Color (0, 0, 0) }
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha
		Offset -1, -1

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
				float4 uvDecal : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				float4 vertex : SV_POSITION;
                float viewDepth : TEXCOORD4;
            };

            sampler2D _DecalTex;
            sampler2D _DepthTex;

            float4 _DecalTex_ST;
			float4x4 _ProjectorMatrix;
			float4x4 _WorldToViewMatrix;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.uvDecal = mul(_ProjectorMatrix, worldPos);
				o.uvDecal = ComputeScreenPos(o.uvDecal);
                o.viewDepth = -(mul(_WorldToViewMatrix, worldPos).x);

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, decalColor);
				fixed2 uv = i.uvDecal.xy / i.uvDecal.w;//齐次除法，转化为ndc坐标
				//fixed4 decalColor = tex2Dproj(_DecalTex, UNITY_PROJ_COORD(i.uvDecal));
                fixed depth = tex2D(_DepthTex, uv);
				fixed4 decalColor = tex2D(_DecalTex, uv);
               

                return decalColor;
            }
            ENDCG
        }
    }
}
