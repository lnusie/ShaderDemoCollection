using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffectDrunk : PosetEffectBase
{
    public const string SHADER_PATH = "X_Shader/PostEffect/DrunkEffect";

    public float OffsetFactor = 1;

    public float Intensity = 1;

    protected override void OnRenderImage(RenderTexture source, RenderTexture destTexture)
    {
        Material mat = GetMaterial(SHADER_PATH);
        mat.SetFloat("_OffsetFactor", OffsetFactor);

        Graphics.Blit(source, destTexture, mat); 
    }
}
