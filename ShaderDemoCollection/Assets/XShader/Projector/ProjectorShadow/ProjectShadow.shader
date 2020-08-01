
Shader "X_Shader/Projector/ProjectShadow"
{
 
    Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_ShadowTex ("_ShadowTex", 2D) = "" {}
		_FalloffTex ("FallOff", 2D) = "white" {}
		_ShadowVisibleRange("ShadowVisibleRange", Range(0.01, 1)) = 0.8
	}
 
	Subshader 
	{
		Pass 
		{
			ZWrite Off
			Fog { Color (0, 0, 0) }
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal : NORMAL;
            };


			struct v2f 
			{
				float4 uvDecal : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				float4 pos : SV_POSITION;
                float3 normal : TEXCOORD2;
                float4 worldPos : TEXCOORD3;
				float4 screenPos : TEXCOORD4;
			};
			
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			fixed4 _Color;
			float4 _ProjectorPos;
			sampler2D _ShadowTex;
			sampler2D _FalloffTex;
			float _ShadowMaxHeight;
			float _ShadowVisibleRange;

			v2f vert (appdata v )
			{
				v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				//v.vertex += v.normal;
				o.uvDecal = mul(unity_Projector, v.vertex);
				o.uvFalloff = mul(unity_Projector, v.vertex);
				o.pos = UnityObjectToClipPos (v.vertex);
				//其实是变换到[0,-w]
				o.screenPos = ComputeScreenPos(o.pos);
                o.normal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 decal = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(i.uvDecal));
				decal *= _Color;
                fixed2 uv = i.uvDecal.xy / i.uvDecal.w;
                decal *= step(0.01, uv.x);
                decal *= step(0.01, uv.y);
                decal *= 1 - step(0.99, uv.x);
                decal *= 1 - step(0.99, uv.y);
				//fixed falloff = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff)).r;
				float2 viewportPos = i.screenPos.xy / i.screenPos.w;
				float viewportDistance = length(viewportPos - float2(0.5,0.5));
				fixed fadeScale = (_ShadowVisibleRange - viewportDistance) / _ShadowVisibleRange;
				decal.a = saturate(fadeScale) * step(0.01, decal.a);
                float3 worldNormal = normalize(i.normal);
                fixed3 viewDir = normalize(_ProjectorPos.xyz - i.worldPos.xyz);
                float vDotN = dot(viewDir, worldNormal);
                decal.a *= step(0.1, vDotN);

				//decal.a *= step(0.01, _ShadowMaxHeight - i.worldPos.y);
				return float4(0, 0, 0, decal.a);
			}   
			
			ENDCG
		}
	}
}
