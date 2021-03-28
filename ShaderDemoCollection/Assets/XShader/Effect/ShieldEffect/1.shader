// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "My/Demo/Shield"
{
	Properties
	{
		_ScanMap("ScanMap",2D)="white"{}
		_ScanCol("ScanColor",color)=(1,1,1,1)
		_ScanScale("ScanScale",range(0,0.2))=0.1
		_ScanFrequency("ScanFrequency",range(0,1))=0.5
		_ScanFresnelScale("ScanFresnelScale",range(0,1))=0.5
		_ScanFresnelPow("ScanFresnelPow",range(0,10))=1
 
		_ShieldMap("ShieldMap",2D)="white"{}
		_ShieldCol("ShieldColor",color)=(1,1,1,1)
		_ShieldFresnelScale("ShieldFresnelScale",range(0,1))=0.5
		_ShieldFresnelPow("ShieldFresnelPow",range(0,10))=1
 
 
	}
	SubShader
	{
		Tags{"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off 
		Blend SrcAlpha OneMinusSrcAlpha
		CGINCLUDE
		#include "UnityCG.cginc"
		sampler2D _ScanMap;
		float4 _ScanMap_ST;
		fixed4 _ScanCol;
		fixed _ScanScale;
		fixed _ScanFrequency;
		fixed _ScanFresnelScale;
		float _ScanFresnelPow;
		
 
		sampler2D _ShieldMap;
		float4 _ShieldMap_ST;
		fixed4 _ShieldCol;
		fixed _ShieldFresnelScale;
		float _ShieldFresnelPow;
 
 
		struct a2v
		{
			float4 vertex:POSITION;
			float4 shield:TEXCOORD0;
			float4 scan:TEXCOORD1;
			fixed4 color:COLOR;
			float3 normal:NORMAL;
		};
		struct v2f
		{
			float4 pos:SV_POSITION;
			float4 uv:TEXCOORD0;
			float2 uv_Line:TEXCOORD4;
			float3 WorldNormal:TEXCOORD1;
			float3 WorldPos:TEXCOORD2;
			float4 color:TEXCOORD3;
		};
 
		//ScanVert是修改顶点位置，然护盾有向外扩张的效果
		v2f ScanVert(a2v v)
		{
			v2f o;
			o.pos=mul(UNITY_MATRIX_MV,v.vertex);
			float3 ViewNormal=mul((float3x3)UNITY_MATRIX_IT_MV,v.normal);
			o.pos+=float4(ViewNormal,0)*_ScanScale;
			o.pos=mul(UNITY_MATRIX_P,o.pos);
			//把顶点位置转换到view位置然后向外扩张。
 
 
			o.uv.xy=TRANSFORM_TEX(v.scan,_ScanMap);
			o.uv.zw=TRANSFORM_TEX(v.shield,_ShieldMap);
			o.WorldNormal=UnityObjectToWorldNormal(v.normal);
			o.WorldPos=mul(unity_ObjectToWorld,v.vertex);
			o.color=v.color;
			return o;
 
		}
 
		fixed4 ScanFrag(v2f i):SV_Target
		{
			fixed scanAlpha=tex2D(_ScanMap,i.uv.xy+float2(0,1)*_Time.y*_ScanFrequency).a;
			//判断scanAlpha是否等于0，等于0剔除
			if(scanAlpha==0)
			{
				discard;
			}
			fixed3 WorldNormalDir=normalize(i.WorldNormal);
			fixed3 WorldViewDir=normalize(_WorldSpaceCameraPos.xyz-i.WorldPos.xyz);
			//fresnel 菲涅尔公式，边缘比较实，里面比较虚那种感觉，可以 直接 return backCol； 看一下效果
			fixed fresnel=_ScanFresnelScale+(1-_ScanFresnelScale)*pow(1-saturate(dot(WorldNormalDir,WorldViewDir)),_ScanFresnelPow);
			fixed fresnleAlpha=lerp(0,1,fresnel);
			fixed4 backCol=fixed4(_ScanCol.rgb,fresnleAlpha);
			fixed4 shieldCol=tex2D(_ShieldMap,i.uv.zw)*_ScanCol;
			//shieldCol.a*scanAlpha是为了看起来中间实两边虚的效果（ps：纹理是渐变纹理来的）
			shieldCol=fixed4(shieldCol.rgb,shieldCol.a*scanAlpha);
			fixed4 finalCol=backCol+shieldCol;
			return finalCol;
 
		}
 
		v2f ShiedlVert(a2v v)
		{
			v2f o;
			o.pos=UnityObjectToClipPos(v.vertex);
			o.uv.xy=TRANSFORM_TEX(v.scan,_ScanMap);
			o.uv.zw=TRANSFORM_TEX(v.shield,_ShieldMap);
			o.WorldNormal=UnityObjectToWorldNormal(v.normal);
			o.WorldPos=mul(unity_ObjectToWorld,v.vertex);
			o.color=v.color;
			return o;
 
		}
		fixed4 ShieldFrag(v2f i):SV_Target
		{
			fixed scanAlpha=tex2D(_ScanMap,i.uv.xy+float2(0,1)*_Time.y*_ScanFrequency).a;
			if(scanAlpha>0)
			{
				discard;
			}
			fixed3 WorldNormalDir=normalize(i.WorldNormal);
			fixed3 WorldViewDir=normalize(_WorldSpaceCameraPos.xyz-i.WorldPos.xyz);
			fixed fresnel=_ShieldFresnelScale+(1-_ShieldFresnelScale)*pow(1-saturate(dot(WorldNormalDir,WorldViewDir)),_ShieldFresnelPow);
			fixed fresnleAlpha=lerp(0,1,fresnel);
			fixed4 backCol=fixed4(_ShieldCol.rgb,fresnleAlpha);
			fixed4 shieldCol=tex2D(_ShieldMap,i.uv.zw)*i.color*_ShieldCol;
			fixed4 finalCol=backCol+shieldCol;
 
 
 
 
			return finalCol;
 
 
		}
 
		ENDCG
		Pass
		{
			CGPROGRAM
			#pragma vertex ScanVert
			#pragma fragment ScanFrag
			ENDCG
		}
		Pass
		{
			CGPROGRAM
			#pragma vertex ShiedlVert
			#pragma fragment ShieldFrag
			ENDCG
		}
 
 
	}
}
