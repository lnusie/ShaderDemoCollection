using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffectIrisBlur : PosetEffectBase
{
    public const string SHADER_PATH = "X_Shader/PostEffect/IrisBlur";

    public Vector2 centerPos;

    [Range(0,1)]
    public float VisibleRadius = 0.5f;

    public float BrightFactor = 2;

    [Range(1, 5)]
    public int iterationCount = 4;

    protected override void OnRenderImage(RenderTexture source, RenderTexture destTexture)
    {
        IrisBlur(source,destTexture, centerPos, VisibleRadius, BrightFactor, iterationCount); 
    } 

    protected static void IrisBlur(RenderTexture source, RenderTexture destTexture,Vector2 centerPos = default(Vector2), float visibleRadius = 0.5f, float brightFactor = 0.2f,int blurSize = 8)
    {
        var postMat = GetMaterial(SHADER_PATH);  
        postMat.SetVector("_CenterPos",centerPos); 
        postMat.SetFloat("_VisibleRadius", visibleRadius); 
        postMat.SetFloat("_BrightFactor", brightFactor);

        var tempSource = source;
   
        int width = source.width;
        int height = source.height;
        int pixelOffset = 1;
        for (int i = 1; i < blurSize; i++)
        {
            RenderTexture tempDest = RenderTexture.GetTemporary(width, height);
            postMat.SetFloat("_PixelOffset", pixelOffset * i);
            Graphics.Blit(tempSource, tempDest, postMat);
            if (tempSource != source)//source不能回收，否则Unity会崩溃 
            {
                RenderTexture.ReleaseTemporary(tempSource);
            }
            tempSource = tempDest;
        }
        postMat.SetFloat("_PixelOffset", pixelOffset * blurSize);
        Graphics.Blit(tempSource, destTexture, GetMaterial(SHADER_PATH));
        if (tempSource != source)
        { 
            RenderTexture.ReleaseTemporary(tempSource);
        }

    }
}
