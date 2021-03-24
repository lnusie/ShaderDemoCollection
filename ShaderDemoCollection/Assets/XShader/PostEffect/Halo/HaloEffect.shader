Shader "X_Shader/PostEffect/HaloEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Gradient("Gradient",2D) = "white" {}
        _SrcTex("SrcTex",2D) = "white" {} 
        _ColorDistortion("ColorDistortion",Color) = (1,1,1,1)
        _StarBurst("StarBurst",2D) = "white"{}
    }

    CGINCLUDE

    sampler2D _MainTex;
    sampler2D _Gradient;
    sampler2D _SrcTex;
    sampler2D _StarBurst;
    float4 _ColorDistortion;

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

    v2f vert (appdata v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = v.uv;
        return o;
    }


    fixed4 filter_frag (v2f i) : SV_Target
    {
        //fixed4 col = tex2D(_MainTex, i.uv);
        fixed4 col = tex2D(_MainTex, i.uv);
        fixed brightness = max(col.b,max(col.r,col.g));
        col *= step(0.9,brightness);
        return col;
    }

    fixed4 GetDistoredColor(half2 uv, half2 dir)
    {
        return fixed4(
            tex2D(_MainTex, uv + dir * _ColorDistortion.r).r,
            tex2D(_MainTex, uv + dir * _ColorDistortion.g).g,
            tex2D(_MainTex, uv + dir * _ColorDistortion.b).b,
            1
            );
    }

    fixed4 ghost_frag(v2f i) : SV_Target
    {
        half2 _CenterPos = 0.5;
        float _GhostDispersal = 0.2;
        float _GhostCount = 3;
        half4 starCol = tex2D(_StarBurst, i.uv);
        half2 uv = 1 - i.uv;
        float angle = 3.14 / 6;
        half2 ghostVec = (_CenterPos - uv) * _GhostDispersal;

        fixed4 finalColor = 0;
        for(int t = 1;t<= _GhostCount;t++)
        {
            half2 offset = frac (uv + ghostVec * t);

            // float x2 = offset.x * cos(angle) - offset.y * sin(angle);
            // float y2 = offset.x * sin(angle);
            // offset.x = x2;
            // offset.y = y2;
            float weight = length((_CenterPos - offset)/length(_CenterPos));
            weight = pow(1 - weight,5);//越偏离中心点越暗
            //finalColor += tex2D(_MainTex, offset) * weight;
            finalColor += GetDistoredColor(uv,normalize(offset)) * weight;
        }
        return finalColor * tex2D(_Gradient,float2(0.5,0.5) + ghostVec) *starCol.r *10 ; 
    }

    fixed4 add_frag(v2f i) : SV_Target
    {
        fixed2 uv = i.uv;
        fixed4 col1 = tex2D(_MainTex, uv);
        fixed4 col2 = tex2D(_SrcTex,uv);
        half4 starCol = tex2D(_StarBurst, i.uv);
        fixed4 col = col1 + col2;
        return col;
    }

    ENDCG

    SubShader 
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment filter_frag

            #include "UnityCG.cginc"
           
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment ghost_frag

            #include "UnityCG.cginc"
           
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment add_frag

            #include "UnityCG.cginc"
           
            ENDCG
        }

    }
}
