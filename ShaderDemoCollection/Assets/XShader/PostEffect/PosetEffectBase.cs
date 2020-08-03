using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosetEffectBase : MonoBehaviour
{
    private static Dictionary<string, Material> materials = new Dictionary<string, Material>();

    public static Material GetMaterial(string shaderPath)
    {
        if (!materials.ContainsKey(shaderPath))
        {
           materials.Add(shaderPath,new Material(Shader.Find(shaderPath)));
        }
        return materials[shaderPath];
    }

    protected virtual void OnRenderImage(RenderTexture source, RenderTexture destTexture)
    {
        Graphics.Blit(source,destTexture);
    }

   
}
