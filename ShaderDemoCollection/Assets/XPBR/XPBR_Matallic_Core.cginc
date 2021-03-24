

    #if !defined(PBR_RUSH_LIGHTING_INCLUDED)
        #define PBR_RUSH_LIGHTING_INCLUDED
        
        #include "UnityPBSLighting.cginc" 
        #include "AutoLight.cginc" 
    #endif 
    
    float4 _Color;
    sampler2D _MainTex;
    float4 _MainTex_ST;
    sampler2D _MetallicGlossMap;
    sampler2D _BumpMap;
    sampler2D _OcclusionMap;
    float _MetallicStrength;
    float _GlossStrength;
    float _BumpScale;
    float4 _EmissionColor;
    sampler2D _EmissonMap;


    //计算环境光照或光照贴图uv坐标,这个函数也可以在UnityStandardCore.cginc中找到，是VertexGIForward函数，不同的是unity对其进行了更多的处理，比如对不同平台的优化
    inline half4 VertexGI(float2 uv1, float2 uv2, float3 worldPos, float3 worldNormal)
    {
        half4 ambientOrLightMapUV = 0;

        //如果开启光照贴图，计算光照贴图的uv坐标
        #ifdef LIGHTMAP_ON
            ambientOrLightMapUV.xy = uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
            // 仅对动态物体采样光照探头，定义在UnityCG.cginc
        #elif UNITY_SHOULD_SAMPLE_SH

            #ifdef VERTEXLIGHT_ON 
                ambientOrLightMapUV.rgb = Shade4PointLights(
                    unity_4LightPosX0,unity_4LightPosY0,unity_4LightPosZ0,
                    unity_LightColor[0].rgb,unity_LightColor[1].rgb,unity_LightColor[2].rgb,unity_LightColor[3].rgb,
                    unity_4LightAtten0,worldPos,worldNormal);
            #endif
            //计算球谱光照，定义在UnityCG.cginc
            ambientOrLightMapUV.rgb += ShadeSH9(half4(worldNormal,1));

        #endif

        //如果开启了动态光照贴图，计算动态光照贴图的uv坐标    
        #ifdef DYNAMICLIGHTMAP_ON
            ambientOrLightMapUV.zw = uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
        #endif 
        return ambientOrLightMapUV;

    }


	struct a2v
    {   
        float4 vertex : POSITION;
        float3 normal : NORMAL;
        float4 tangent : TANGENT;
        float2 texcoord : TEXCOORD0;
        float2 texcoord1 : TEXCOORD1;
        float2 texcoord2 : TEXCOORD2;

    };

    struct v2f
    {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
        half4 ambientOrLightMapUV : TEXCOORD1; //存储环境光或光照贴图的UV坐标
        float4 TtoW0 : TEXCOORD2;
        float4 TtoW1 : TEXCOORD3;
        float4 TtoW2 : TEXCOORD4;//xyz 存储从切线空间到世界空间的坐标。 w存储着世界坐标
        SHADOW_COORDS(5)  //定义阴影所需要的变量，定义在AutoLight.cginc
        UNITY_FOG_COORDS(6) //定义雾效所需要的变量，定义在UnityCG.cginc

    };




    v2f vert (a2v v)
    {
        v2f o;
        UNITY_INITIALIZE_OUTPUT(v2f,o);//初始化结构体数据，定义在HLSLSupport.cginc                                   

        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);

        float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
        float3 worldNormal = UnityObjectToWorldNormal(v.normal);
        half3 worldTangent = UnityObjectToWorldDir(v.tangent);
        half3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

        //计算环境光照或光照贴图uv坐标
        o.ambientOrLightMapUV = VertexGI(v.texcoord1,v.texcoord2,worldPos,worldNormal);

        o.TtoW0 = float4(worldTangent.x,worldBinormal.x,worldNormal.x,worldPos.x);
        o.TtoW1 = float4(worldTangent.y,worldBinormal.y,worldNormal.y,worldPos.y);
        o.TtoW2 = float4(worldTangent.z,worldBinormal.z,worldNormal.z,worldPos.z);

        //填充阴影所需要的参数,定义在AutoLight.cginc
        TRANSFER_SHADOW(o);

        //填充雾效所需要的参数，定义在UnityCG.cginc
        UNITY_TRANSFER_FOG(o,o.pos);

        return o;
    }


    half3 ComputeIndirectDiffuse(float2 ambientOrLightMapUV, half occlusion)
    {
        half3 indirectDiffuse = 0;

        //如果是动态物体，间接光漫反射为在顶点函数中计算的非重要光源
        #if UNITY_SHOULD_SAMPLE_SH
            indirectDiffuse = ambientOrLightMapUV.rgb;
        #endif

        //对于静态物体，则采样光照贴图或动态光照贴图
        #ifdef LIGHTMAP_ON
            //对光照贴图进行采样和解码
            //UNITY_SAMPLE_TEX2D定义在HLSLSupport.cginc
            //DecodeLightmap定义在UnityCG.cginc
            indirectDiffuse = DecodeLightmap(UNITY_SAMLE_TEX2D(unity_Lightmap,ambientOrLightMapUV.xy));
        #endif

        #ifdef DYNAMICLIGHTMAP_ON 
            //对动态光照贴图进行采样和解码
            //DecodeRealtimeLightmap定义在UnityCG.cginc
            indirectDiffuse += DecodeRealtimeLightmap(UNITY_SAMPLE_TEX2D(unity_DynamicLightmap,ambientOrLightMapUV.zw));
        #endif
        return indirectDiffuse * occlusion;
    }

    //重新映射反射方向
    inline half3 BoxProjectedDirection(half3 worldRefDir,float3 worldPos,float4 cubemapCenter,float4 boxMin,float4 boxMax)
    {
        //使下面的if语句产生分支，定义在HLSLSupport.cginc中
        UNITY_BRANCH
        if(cubemapCenter.w > 0.0)//如果反射探头开启了BoxProjection选项，cubemapCenter.w > 0
        {
            half3 rbmax = (boxMax.xyz - worldPos) / worldRefDir;
            half3 rbmin = (boxMin.xyz - worldPos) / worldRefDir;

            half3 rbminmax = (worldRefDir > 0.0f) ? rbmax : rbmin;

            half fa = min(min(rbminmax.x,rbminmax.y),rbminmax.z);

            worldPos -= cubemapCenter.xyz;
            worldRefDir = worldPos + worldRefDir * fa;
        }
        return worldRefDir;
    }

    //采样反射探头
    //UNITY_ARGS_TEXCUBE定义在HLSLSupport.cginc,用来区别平台
    inline half3 SamplerReflectProbe(UNITY_ARGS_TEXCUBE(tex),half3 refDir,half roughness,half4 hdr)
    {

        //在光照探头中存储的是一组图像，内容逐渐模糊，之所以这样做是因为当我们的物体比较粗糙的时候，
        //反射的内容也是比较模糊的，如果我们实时模糊运算时，性能肯定是特别费的。
        //所以unity烘焙成一组这样的图像也就是mipmap,然后我们来对其进行不同等级的插值采样。
        //级数越高越模糊，乘以6，也是因为6是我们这个的总级数。
        //因为物体的粗糙度和反射图像的清晰度并不是成线性的，所以会有第一个公式，
        //这个公式是一个近似公式，主要是为了节省性能，
        //也可以在UnityStandardBRDF.cginc中Unity_GlossyEnvironment函数找到对应的公式。
        roughness = roughness * (1.7 - 0.7 * roughness);
        half mip = roughness * 6;
        //对反射探头进行采样
        //UNITY_SAMPLE_TEXCUBE_LOD定义在HLSLSupport.cginc，用来区别平台
        half4 rgbm = UNITY_SAMPLE_TEXCUBE_LOD(tex,refDir,mip);
        //采样后的结果包含HDR,所以我们需要将结果转换到RGB
        //定义在UnityCG.cginc
        return DecodeHDR(rgbm,hdr);
    }

    //在计算间接镜面反射时，会比较麻烦一些，这里处理了两个反射探头，
    //首先对第一个反射探头反射方向进行重新映射和采样，
    //然后判断是否使用了第二个反射探头，如果使用了第二个反射探头，则对其进行采样并混合，
    //最后乘上遮罩对其的影响
    inline half3 ComputeIndirectSpecular(half3 refDir,float3 worldPos,half roughness,half occlusion)
    {
        half3 specular = 0;

        half3 refDir1 = BoxProjectedDirection(refDir,worldPos,
            unity_SpecCube0_ProbePosition,unity_SpecCube0_BoxMin,
            unity_SpecCube0_BoxMax);

        //对第一个反射探头进行采样
        half3 ref1 = SamplerReflectProbe(UNITY_PASS_TEXCUBE(unity_SpecCube0),
        refDir1,roughness,unity_SpecCube0_HDR);
        //如果第一个反射探头的权重小于1的话，我们将会采样第二个反射探头，进行混合
        
        //UNITY_BRANCH他会让下面的if语句产生一个分支，而与其对立的是UNITY_FLATTEN，
        //他始终会运行if的两个结果的所有语句，并在完成后选择一个正确的结果。
        //他们定义在HLSLSupport.cginc中，他们其实就是HLSL平台上的[branch]和[flatten],在其他平台上什么事都不做。
        UNITY_BRANCH
        if(unity_SpecCube0_BoxMin.w < 0.99999)
        {
            //重新映射第二个反射探头的方向
            half3 refDir2 = BoxProjectedDirection(refDir,worldPos,
                unity_SpecCube1_ProbePosition,
                unity_SpecCube1_BoxMin,unity_SpecCube1_BoxMax);
            //对第二个反射探头进行采样
            half3 ref2 = SamplerReflectProbe(UNITY_PASS_TEXCUBE_SAMPLER(unity_SpecCube1,unity_SpecCube0),
                refDir2,roughness,unity_SpecCube1_HDR);
            //进行混合
            specular = lerp(ref2,ref1,unity_SpecCube0_BoxMin.w);
        }
        else
        {
            specular = ref1;
        }
        return specular * occlusion;

    }

    //计算菲涅尔反射
    inline half3 ComputeFresnelLerp(half3 c0,half3 c1,half nv)
    {
        half t = pow(1 - nv,5);
        return lerp(c0,c1,t);
    }

    //计算Smith-Joint阴影遮掩函数，返回的是除以镜面反射项分母的可见性项V
    //阴影遮掩函数G除以BRDF镜面反射分母(n·l)(n·v)部分，
    //在这里为了方便我们也把系数4也加了进去。我们会在分母的后面加上1e-5f,来防止分母为0
    inline half ComputeSmithJointGGXVisibilityTerm(half nl,half nv,half roughness)
    {
        half ag = roughness * roughness;
        half lambdaV = nl * (nv * (1 - ag) + ag);
        half lambdaL = nv * (nl * (1 - ag) + ag);
        
        return 0.5f/(lambdaV + lambdaL + 1e-5f);
    }
    //计算法线分布函数
    inline half ComputeGGXTerm(half nh,half roughness)
    {
        half a = roughness * roughness;
        half a2 = a * a;
        half d = (a2 - 1.0f) * nh * nh + 1.0f;
        //UNITY_INV_PI定义在UnityCG.cginc  为1/π
        return a2 * UNITY_INV_PI / (d * d + 1e-5f);
    }
    //计算菲涅尔
    inline half3 ComputeFresnelTerm(half3 F0,half cosA)
    {
        return F0 + (1 - F0) * pow(1 - cosA, 5);
    }

    //计算漫反射项
    inline half3 ComputeDisneyDiffuseTerm(half nv,half nl,half lh,half roughness,half3 baseColor)
    {
        half Fd90 = 0.5f + 2 * roughness * lh * lh;
        return baseColor * UNITY_INV_PI * (1 + (Fd90 - 1) * pow(1-nl,5)) * (1 + (Fd90 - 1) * pow(1-nv,5));
    }


    //在片元函数中，先将声明的材质属性进行处理，转换成能够直接使用的变量。再计算灯光，观察,反射方向
    //以及BRDF需要用到的一些数量积等

 	fixed4 frag (v2f i) : SV_Target
    {
        float3 worldPos = float3(i.TtoW0.w,i.TtoW1.w,i.TtoW2.w);
        half3 albedo = tex2D(_MainTex,i.uv).rgb * _Color.rgb;//反射率
        half2 metallicGloss = tex2D(_MetallicGlossMap,i.uv).ra; //r通道存储金属度分布,a通道存储光滑度分布
        half metallic = metallicGloss.x * _MetallicStrength;//金属度
        half roughness = 1 - metallicGloss.y *  _GlossStrength; //粗糙度
        half occlusion = tex2D(_OcclusionMap,i.uv).g;//环境光遮挡


        half3 normalTangent = UnpackNormal(tex2D(_BumpMap,i.uv));
        normalTangent.xy *= _BumpScale;
        normalTangent.z = sqrt(1 - saturate(dot(normalTangent.xy,normalTangent.xy)));
        half3 worldNormal = 0;
        worldNormal.x = dot(i.TtoW0.xyz,normalTangent);
        worldNormal.y = dot(i.TtoW1.xyz,normalTangent);
        worldNormal.z = dot(i.TtoW2.xyz,normalTangent);
        worldNormal = normalize(worldNormal);

        //世界空间下的灯光方向,定义在UnityCG.cginc
        half3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
        //世界空间下的观察方向,定义在UnityCG.cginc
        half3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));

        //世界空间下的反射方向
        half3 refDir = reflect(-viewDir,worldNormal);

        half3 emission = tex2D(_EmissonMap,i.uv).rgb * _EmissionColor;//自定义颜色

        UNITY_LIGHT_ATTENUATION(atten,i,worldPos);//计算阴影和衰减,定义在AutoLight.cginc

        half3 halfDir = normalize(lightDir + viewDir);
        half nv = saturate(dot(worldNormal,viewDir));
        half nl = saturate(dot(worldNormal,lightDir));
        half nh = saturate(dot(worldNormal,halfDir));
        half lv = saturate(dot(lightDir,viewDir));
        half lh = saturate(dot(lightDir,halfDir));

        //计算BRDF需要用到的镜面反射率，漫反射率。
        half3 specColor = lerp(unity_ColorSpaceDielectricSpec.rgb,albedo,metallic);

        // 1 - 反射率，漫反射总比率
        half oneMinusReflectivity = (1-metallic) * unity_ColorSpaceDielectricSpec.a;

        //计算漫反射率
        half3 diffColor = albedo * oneMinusReflectivity;

        //unity_ColorSpaceDielectricSpec.rgb 定义物体的基础镜面反射率
        //unity_ColorSpaceDielectricSpec.a 存储的是 1-dielectricSpec
        //oneMinusReflectivity计算公式推导可以从UnityStandardUtils.cginc中 OneMinusReflectivityFromMetallic函数中找到


        //计算间接光照，间接光照包含进间接漫反射和间接镜面反射
        half3 indirectDiffuse = ComputeIndirectDiffuse(i.ambientOrLightMapUV,occlusion);//计算间接光漫反射
        half3 indirectSpecular = ComputeIndirectSpecular(refDir,worldPos,roughness,occlusion);//计算间接光镜面反射
        
        //上面只是计算了间接光的直接颜色，这并满足能量守恒，所以我们需要对其物理对其的影响
        //计算掠射角时反射率
        half grazingTerm = saturate((1 - roughness) + (1-oneMinusReflectivity));
        
        //计算间接光镜面反射
        //在这里镜面反射需要满足菲涅尔反射，因为不同视角他的反射率也不相同。
        //grazingTerm则是我们之前计算的掠射角时反射率
        indirectSpecular *= ComputeFresnelLerp(specColor,grazingTerm,nv);
        //计算间接光漫反射
        indirectDiffuse *= diffColor;

        //最后只剩下计算直接光照了
        
        half V = ComputeSmithJointGGXVisibilityTerm(nl,nv,roughness);//计算BRDF高光反射项，可见性V
        half D = ComputeGGXTerm(nh,roughness);//计算BRDF高光反射项,法线分布函数D
        half3 F = ComputeFresnelTerm(specColor,lh);//计算BRDF高光反射项，菲涅尔项F

        half3 specularTerm = V * D * F;//计算镜面反射项

        //然后是BRDF漫反射部分

        half3 diffuseTerm = ComputeDisneyDiffuseTerm(nv,nl,lh,roughness,diffColor);//计算漫反射项

        //这样所有的光也就全部都计算完了，就剩最后一步了,全部加起来就好了
        //计算最后的颜色
        half3 color = UNITY_PI * (diffuseTerm + specularTerm) * _LightColor0.rgb * nl * atten
                        + indirectDiffuse + indirectSpecular + emission;

        //设置雾效,定义在UnityCG.cginc
        UNITY_APPLY_FOG(i.fogCoord, color.rgb);

        return float4(color,1);
    }