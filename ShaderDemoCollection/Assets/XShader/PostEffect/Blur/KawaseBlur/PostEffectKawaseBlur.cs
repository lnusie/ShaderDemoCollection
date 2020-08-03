using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffectKawaseBlur : PosetEffectBase
{
    public const string SHADER_PATH = "X_Shader/PostEffect/KawaseBlur";

    public float pixelOffset = 1; 

    [Range(1,5)] 
    public int iterationCount = 4; 
    

    void OnRenderImage(RenderTexture source, RenderTexture destTexture)
    {
        Blur(source, destTexture, iterationCount, pixelOffset);
    }

    public static void Blur(RenderTexture source, RenderTexture destTexture, int iterationCount = 4,float pixelOffset = 1)
    {
        Material postMaterial = GetMaterial(SHADER_PATH); 
        RenderTexture tempSource = source;
        int width = source.width;
        int height = source.height;
        for (int i = 1; i < iterationCount; i++)
        {
            RenderTexture tempDest = RenderTexture.GetTemporary(width, height); 
            postMaterial.SetFloat("_PixelOffset", pixelOffset * i);
            Graphics.Blit(tempSource, tempDest, postMaterial);
            if (tempSource != source)//source不能回收，否则Unity会崩溃 
            {
                RenderTexture.ReleaseTemporary(tempSource);
            }
            tempSource = tempDest;
        }
        postMaterial.SetFloat("_PixelOffset", pixelOffset * iterationCount);
        Graphics.Blit(tempSource, destTexture, postMaterial);
        if (tempSource != source)
        {
            RenderTexture.ReleaseTemporary(tempSource);
        }
    }

}
