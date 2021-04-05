Shader "X_Shader/Effect/GodRay"

{
    Properties
    {
        _MainTex ("GridMask", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Brightness("Brightness", Range(1, 5)) = 1
        _FadeNearDist("FadeNearDist", float) = 1  //当视野距离小于这个距离时，透明度由1->0淡出
        _FadeFarDist("FadeFarDist", float) = 2 //当从远处到这个距离时，透明度由0 -> 1淡入
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent" 
            "Queue" = "Transparent"
            "IgnoreProjector"="True" 
        }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off 

        CGINCLUDE

        #include "UnityCG.cginc"
        #include "Lighting.cginc"
        
        sampler2D _MainTex;
        sampler2D _CameraDepthTexture;
        sampler2D _NoiseTex;

        float4 _MainTex_ST;
        half4 _Color;
        float _Brightness;
        float _FadeNearDist;
        float _FadeFarDist;

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            float4 normal : NORMAL;
            float4 color : COLOR;

        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 pos : SV_POSITION;
            float4 color : TEXCOORD1;
        };
      
        v2f BaseVert(appdata v)
        {
            v2f o;
            o.uv = v.uv;
            //_FadeNearDist("FadeOutNearDist", float) = 1  //当视野距离小于这个距离时，透明度由1->0淡出
            //_FadeFarDist("FadeOutFarDist", float) = 2 //当从远处到这个距离时，透明度由0 -> 1淡入

            float4 viewPos = mul(UNITY_MATRIX_MV, v.vertex);
            float viewDist = length(viewPos);

            float alpha = 0, fadeFar, fadeNear;
            fadeFar = 1 - saturate((viewDist - _FadeFarDist) / (_FadeFarDist) * 0.2);
            fadeNear = saturate((viewDist) / (_FadeNearDist) * 0.8);
            fadeNear *= fadeNear;
            alpha = fadeNear * fadeFar;
            o.color = alpha * v.color * _Brightness;
            //mesh需要特殊处理，normal方向是沿着面片方向的，而非正常的垂直于面片
            //v.vertex -= v.normal * alpha * o.color.a * 10; //越透明的顶点越往上收缩
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
            return o;
        }

        fixed4 BaseFrag (v2f i) : SV_Target
        {   
            float4 color = tex2D(_MainTex, i.uv);
            color.a = color.r * i.color.a;
            color.rgb = _Color;
            return color; 
        }

        ENDCG
      
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex BaseVert
            #pragma fragment BaseFrag
            ENDCG
        }
    }
    
       
}
