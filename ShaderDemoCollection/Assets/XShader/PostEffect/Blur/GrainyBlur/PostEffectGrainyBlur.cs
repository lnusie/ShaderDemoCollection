using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 毛玻璃模糊效果
/// </summary>
public class PostEffectGrainyBlur : PosetEffectBase
{
    public const string SHADER_PATH = "X_Shader/PostEffect/GrainyBlur";

    [Range(1,5)]
    public int IterationCount = 4;

    protected override void OnRenderImage(RenderTexture source, RenderTexture destTexture)
    {
        GrainyBlur(source, destTexture, IterationCount);
    }

    protected static void GrainyBlur(RenderTexture source, RenderTexture destTexture, int iterationCount = 8)
    {
        var postMat = GetMaterial(SHADER_PATH);
        postMat.SetFloat("_IterationCount", iterationCount);

        Graphics.Blit(source,destTexture, postMat);
    }
}
