#if !defined(SHADOW_INCLUDE)
#define SHADOW_INCLUDE
#endif

#include "UnityCG.cginc"

float4 _Tint;
sampler2D _MainTex;
float4 _MainTex_ST;
float _AlphaCutoff;
sampler3D _DitherMaskLOD;

#if defined(_RENDERING_FADE) || defined(_RENDERING_TRANSPARENT)
		#if defined(_SEMITRANSPARENT_SHADOWS)
		#define SHADOWS_SEMITRANSPARENT 1
	#else
		#define _RENDERING_CUTOUT
	#endif
#endif

#if SHADOWS_SEMITRANSPARENT || defined(_RENDERING_CUTOUT)
	#if !defined(_SMOOTHNESS_ALBEDO)
		#define SHADOWS_NEED_UV 1
	#endif
#endif


struct VertexData {
	float4 position : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD0;
};

struct InterpolatorsVertex {
	float4 position : SV_POSITION;
	#if SHADOWS_NEED_UV
		float2 uv : TEXCOORD0;
	#endif
	#if defined(SHADOWS_CUBE)
		float3 lightVec : TEXCOORD1;
	#endif
};

struct Interpolators {
	#if SHADOWS_SEMITRANSPARENT
		UNITY_VPOS_TYPE vpos : VPOS;
	#else
		float4 positions : SV_POSITION;
	#endif

	#if SHADOWS_NEED_UV
		float2 uv : TEXCOORD0;
	#endif
	#if defined(SHADOWS_CUBE)
		float3 lightVec : TEXCOORD1;
	#endif
};

//UnityCG中UnityApplyLinearShadowBias的实现
//主要是增加裁剪坐标的z值。但它使用的是齐次坐标，必须补偿透视投影
//以使偏移量不会随着相机的距离而变换，还必须确保结果不会超出范围
float4 _UnityApplyLinearShadowBias(float4 clipPos)
{
	clipPos.z += saturate(unity_LightShadowBias.x / clipPos.w);
	float clamped = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
	clipPos.x = lerp(clipPos.z, clamped, unity_LightShadowBias.y);

}

//UnityCG中UnityClipSpaceShadowCasterPoss的实现
//它将位置转换为世界空间，应用法向偏差，
//然后转换为裁剪空间。确切的偏移量取决于法线和光方向之间的角度以及阴影纹素大小。
float4 _UnityClipSpaceShadowCasterPos (float3 vertex, float3 normal) {
	float4 clipPos;
    // Important to match MVP transform precision exactly while rendering
    // into the depth texture, so branch on normal bias being zero.
    if (unity_LightShadowBias.z != 0.0) {
		float3 wPos = mul(unity_ObjectToWorld, float4(vertex,1)).xyz;
		float3 wNormal = UnityObjectToWorldNormal(normal);
		float3 wLight = normalize(UnityWorldSpaceLightDir(wPos));

	// apply normal offset bias (inset position along the normal)
	// bias needs to be scaled by sine between normal and light direction
	// (http://the-witness.net/news/2013/09/shadow-mapping-summary-part-1/)
	//
	// unity_LightShadowBias.z contains user-specified normal offset amount
	// scaled by world space texel size.
		float shadowCos = dot(wNormal, wLight);
		float shadowSine = sqrt(1 - shadowCos * shadowCos);
		//法线与光线夹角越大，偏移值越大
		float normalBias = unity_LightShadowBias.z * shadowSine;
		wPos -= wNormal * normalBias;
		clipPos = mul(UNITY_MATRIX_VP, float4(wPos, 1));
    }
    else {
        clipPos = UnityObjectToClipPos(vertex);
    }
	return clipPos;
}

//UnityCG中EncodeFloatRGBA的实现
//将0~1的浮点数转换成8位的RGBA通道值
//Note that : >= 1不能编码
inline float4 _EncodeFloatRGBA (float v) {
	float4 kEncodeMul = float4(1.0, 255.0, 65025.0, 16581375.0);
	float kEncodeBit = 1.0 / 255.0;
	float4 enc = kEncodeMul * v;
	enc = frac(enc);
	enc -= enc.yzww * kEncodeBit;
	return enc;
}

//将0~1的数值转为8位的RGBA值
float4 _UnityEncodeCubeShadowDepth (float z) {
	#ifdef UNITY_USE_RGBA_FOR_POINT_SHADOWS
		return EncodeFloatRGBA(min(z, 0.999));
	#else
		return z;
	#endif
}

float GetAlpha (Interpolators i) {
	float alpha = _Tint.a;
	#if SHADOWS_NEED_UV
		alpha *= tex2D(_MainTex, i.uv.xy).a;
	#endif
	return alpha;
}

InterpolatorsVertex ShadowVertex(VertexData v)
{
	InterpolatorsVertex i;
	//点光源生成的阴影贴图是CubeMap，需要在片元函数中进行特殊的深度计算
	#if defined(SHADOW_CUBE)
		i.position = UnityObjectToClipPos(v.position);
		//_LightPositionRange : xyz存储光源位置 z存储其范围的倒数
		i.lightVec = mul(unity_objectToWorld, v.position).xyz - _LightPositionRange.xyz;
	#else
		//float4 position = UnityObjectToClipPos(v.position);
		float4 position = UnityClipSpaceShadowCasterPos(v.position.xyz, v.normal);
		//加入深度偏差,为了解决阴影失真问题
		//阴影失真的根本原因 1是精度问题，2是Shadow depth map 分辨率不够，因此多个相邻的Pixel会对应map上的一个点，
		//在对比场景深度和灯光深度的时候就会出现场景深度图上相邻的几个点对应灯光深度图的一个点，相邻几个点的比较结果有大有小（selfShadow）
		//https://blog.csdn.net/lawest/article/details/106364935
		i.position = UnityApplyLinearShadowBias(position);
	#endif 
	#if SHADOWS_NEED_UV
		i.uv = TRANSFORM_TEX(v.uv, _MainTex);
	#endif
	return i;
}	

float4 ShadowFragment(Interpolators i) : SV_TARGET{
	float alpha = GetAlpha(i);
	#if defined(_RENDERING_CUTOUT)
		clip(alpha - _AlphaCutoff);
	#endif

	#if SHADOWS_SEMITRANSPARENT 
		float dither =
			tex3D(_DitherMaskLOD, float3(i.vpos.xy * 0.25, alpha * 0.9375)).a;
			clip(dither - 0.01);
	#endif

	#if defined(SHADOW_CUBE)
		float depth = length(i.lightVec) + unity_LightShadowBias.x;
		depth *= _LightPositionRange.w;//depth最终被限制在0~1之间，超过这个范围物体不在点光源范围内
		//UnityEncodeCubeShadowDepth将深度值编码存储在立方体贴图RGBA通道中
		return UnityEncodeCubeShadowDepth(depth);
	#else
		return 0;
	#endif
}
