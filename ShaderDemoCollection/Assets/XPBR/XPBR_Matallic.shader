Shader "X_Shader/PBR/XPBR_Matallic"
{
    Properties
    {
        _Color("Color",color) = (1,1,1,1) 
        _MainTex("Albedo",2D) = "white"{}
        _MetallicGlossMap("Metallic",2D) = "white"{}//金属图，r通道存储金属度，a通道存储光滑度
        _BumpMap("Normal Map",2D) = "bump" {}//法线贴图
        _OcclusionMap("Occlusion",2D) = "white"{}//环境光遮蔽纹理
        _MetallicStrength("MetallicStrength",Range(0,1)) = 1 //金属强度
        _GlossStrength("Smoothness",Range(0,1)) = 0.5 //光滑强度
        _BumpScale("Noraml Scale",float) = 1 //法线缩放因子
        _EmissionColor("Color",color) = (0,0,0) //自发光颜色
        _EmissonMap("Emission Map",2D) = "white"{}

    }
    SubShader
    {
        // No culling or depth
        Cull Back 


        Pass
        {
        	Tags{
        		"LightMode" = "ForwardBase"

        	}

            CGPROGRAM

            #pragma target 3.0

            #pragma vertex vert
            #pragma fragment frag

            #include "XPBR_Matallic_Core.cginc"
 
            ENDCG
        }

        Pass
        {
            Tags
            {
                "LightMode" = "ForwardAdd"
            }

            //与其他光照叠加
            Blend One One
            //第一个Pass已经写入Z值
            ZWrite Off

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            //本Pass可能会用于各种光源的渲染，所以定义多个变体
            //带cookie的平行光有自己的光衰减宏，因此Unity将其视为一种不同的光类型
            //#pragma multi_compile DIRECTIONAL DIRECTIONAL_COOKIE POINT SPOT 

            //multi_compile_fwdadd 完成了和以上相同的操作
            #pragma multi_compile_fwdadd

            //顶点光源宏，顶点光源只支持点光源
            #pragma multi_compile _ VERTEXLIGHT_ON

            #include "XPBR_Matallic_Core.cginc"

            ENDCG

        }

    }
}
