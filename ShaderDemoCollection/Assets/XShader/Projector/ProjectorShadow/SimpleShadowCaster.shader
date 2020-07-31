
Shader "X_Shader/Projector/SimpleShadowCaster"
{
 
    Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
	}
 
	Subshader 
	{
		Pass 
		{
			ZWrite Off
 
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
				float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
			};
			
			v2f vert (appdata v )
			{
				v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return 1;
			}   
			
			ENDCG
		}
	}
}
