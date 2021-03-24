using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffectHalo : PosetEffectBase
{
    public const string SHADER_PATH = "X_Shader/PostEffect/HaloEffect";

    public Texture2D gradiantTexture;

    public Texture2D starBurst;

    public Material mat;

    public Color ColorDistortion;

    void Start()
    {
        mat = GetMaterial(SHADER_PATH);
        mat.SetTexture("_Gradient", gradiantTexture);
        mat.SetTexture("_StarBurst", starBurst);

    }

    protected override void OnRenderImage(RenderTexture source, RenderTexture destTexture)
    {
        //var mat = GetMaterial(SHADER_PATH);

        mat.SetColor("_ColorDistortion", ColorDistortion);
        mat.SetTexture("_SrcTex",source);
        RenderTexture tempDest = RenderTexture.GetTemporary(source.width,source.height);
        RenderTexture tempDest2 = RenderTexture.GetTemporary(source.width, source.height);
        Graphics.Blit(source, tempDest, mat, 0);
        Graphics.Blit(tempDest, tempDest2, mat, 1);
        tempDest.DiscardContents(); 
        PostEffectKawaseBlur.Blur(tempDest2, tempDest, 4,2);
        Graphics.Blit(tempDest, destTexture, mat, 2);

        RenderTexture.ReleaseTemporary(tempDest);
        RenderTexture.ReleaseTemporary(tempDest2);

        //tempDest = RenderTexture.GetTemporary(source.width, source.height);
    }

}
