Shader "X_Shader/PostEffect/PostWaveEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
       	_InputParams("InputParams",Vector) = (0.5,0.5,1,1) //xy : centerPos z : Frequency w : Amplitude 
		_WaveWidth("WaveWidth",Range(0,1)) = 0.1
		_CurWaveDis("CurWaveDis",Range(0,1)) = 0.1

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

            sampler2D _MainTex;
            fixed4 _MainTex_TexelSize;
           	float4 _InputParams;
           	float _WaveWidth;
           	float _CurWaveDis;
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {		
            	float texelUnit = _MainTex_TexelSize.y;
            	#if UNITY_UV_STARTS_AT_TOP
            		if(_MainTex_TexelSize.y < 0)
            		{
            			texelUnit = -texelUnit;	
            		} 
            	#endif

            	float2 dv = i.uv - _InputParams.xy;
            	//将椭圆缩放为圆形 
            	dv = dv * float2(_ScreenParams.x/_ScreenParams.y,1);
            	float dis = length(dv);
            	float sinFactor = sin(dis * _InputParams.y + _Time.y);
            	sinFactor = abs(sinFactor);
            	sinFactor *= _InputParams.w * texelUnit;
            	fixed discardFactor = clamp((_WaveWidth - abs(dis - _CurWaveDis)),0,1);
                //越偏离中心越平缓 
            	discardFactor *= (-0.2 * (dis - _CurWaveDis) + 1); 
            	dv = normalize(dv);  
            	fixed2 uvOffset = dv * sinFactor * discardFactor; 
            	fixed4 col = tex2D(_MainTex, i.uv + uvOffset);
            	return col; 
            }
            ENDCG
        }
    }
}
