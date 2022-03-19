Shader "X_Shader/Projector/MyProjector"
{
    Properties
    {
        _DecalTex ("Decal Texture", 2D) = "white" {}
    }
    SubShader
    {
		ZWrite Off
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
            };

            sampler2D _DecalTex;
            float4 _DecalTex_ST;
			float4x4 _ProjectorMatrix;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

				o.uvDecal = mul(_ProjectorMatrix, worldPos);
				o.uvDecal = ComputeScreenPos(o.uvDecal);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 decalColor = tex2Dproj(_DecalTex, UNITY_PROJ_COORD(i.uvDecal));
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, decalColor);
				fixed2 uv = i.uvDecal.xy / i.uvDecal.w;//齐次除法，转化为ndc坐标
				fixed4 decalColor2 = tex2D(_DecalTex, uv);
                return decalColor2;
				float a = decalColor.a;
				a *= step(0.01, uv.x);
				a *= step(0.01, uv.y);
				a *= 1 - step(0.99, uv.x);
				a *= 1 - step(0.99, uv.y);
                decalColor.a = a;
                return decalColor;
            }
            ENDCG
        }
    }
}
