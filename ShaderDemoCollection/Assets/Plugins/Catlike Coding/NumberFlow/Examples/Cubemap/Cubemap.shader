// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Copyright 2016, Catlike Coding, http://catlikecoding.com
// Simple shader to demonstrate how to sameple a cubemap.
Shader "Catlike Coding/NumberFlow/Examples/Cubemap" {
	Properties {
		_MainTex ("Cubemap", Cube) = "" {}
		[Enum(Both, 0, Inside, 1, Outside, 2)] _Culling ("Direction", Float) = 1
	}
	SubShader {
		Tags {
			"RenderType" = "Opaque"
		}
		LOD 100
		Cull [_Culling]

		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			samplerCUBE _MainTex;

			struct appdata {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float3 uv : TEXCOORD0;
			};

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.vertex;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				return texCUBE(_MainTex, i.uv);
			}

			ENDCG
		}
	}
}