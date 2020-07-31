// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Copyright 2016, Catlike Coding, http://catlikecoding.com
// Simple shader to demonstrate how to use an object-space normal cubemap.
Shader "Catlike Coding/NumberFlow/Examples/Cubemap Bumped" {
	Properties {
		_MainTex ("Cubemap", Cube) = "" {}
		_NormalMap ("Normalmap", Cube) = "" {}
		_Specular ("Specular Color", Color) = (0.5, 0.5, 0.5)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	}
	SubShader {
		Tags {
			"RenderType" = "Opaque"
		}
		LOD 100

		Pass {
			Tags {
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			samplerCUBE _MainTex;
			samplerCUBE _NormalMap;
			
			half3 _Specular;
			half _Shininess;

			half4 _LightColor0;

			struct appdata {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half3 worldPos : TEXCOORD0;
				float3 uv : TEXCOORD1;
			};

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.uv = v.vertex;
				return o;
			}
			
			half4 frag (v2f i) : SV_Target {
				half3 diffuse = texCUBE(_MainTex, i.uv).rgb;
				half3 objectNormal = texCUBE(_NormalMap, i.uv) * 2 - 1;
				half3 worldNormal = UnityObjectToWorldNormal(objectNormal);
				
				half3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
				half3 lightDir = _WorldSpaceLightPos0.xyz;
				half3 halfVector = normalize(lightDir + viewDir);
				half normalDotHalf = max(dot(worldNormal, halfVector), 0);
				
				half diffuseFactor = max(dot(worldNormal, lightDir), 0);
				half specularFactor = pow(normalDotHalf, _Shininess * 128);
				
				half3 color = diffuseFactor * diffuse + specularFactor * _Specular;
				return half4(color * _LightColor0, 1);
			}

			ENDCG
		}
	}
}