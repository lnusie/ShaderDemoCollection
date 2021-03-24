using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
public class PostWaveEffect : MonoBehaviour
{

    public Vector4 InputParams;

    [Range(0, 1)]
    public float WaveWidth = 0.1f;

    [Range(0, 1)]
    public float MaxWaveDistance = 0.5f;

    public float MinWaveDistance = 0.1f;

    public float curWaveDistance = 0.1f;

    private Material m_PostMaterial;

    public float WaveMoveSpeed = 0.01f;

    public bool playWaveEffect = true;

    public bool loopPlay = false;

    public Material PostMaterial
    {
        get
        {
            if (!m_PostMaterial)
            {
                m_PostMaterial = new Material(Shader.Find("X_Shader/PostEffect/PostWaveEffect"));
            }
            return m_PostMaterial;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destTexture)
    {
        if (!playWaveEffect)
        {
            Graphics.Blit(source, destTexture);
        }
        else
        {
            curWaveDistance = curWaveDistance + WaveMoveSpeed;
            if (curWaveDistance > MaxWaveDistance)
            {
                if (loopPlay)
                {
                    curWaveDistance = MinWaveDistance;
                }
                else
                {
                    playWaveEffect = false;
                }
            }
            PostMaterial.SetVector("_InputParams", InputParams);
            PostMaterial.SetFloat("_WaveWidth", WaveWidth);
            PostMaterial.SetFloat("_CurWaveDis", curWaveDistance);
            Graphics.Blit(source, destTexture, PostMaterial);
        } 
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            var pos = Input.mousePosition;
            pos.x /= Screen.width;
            pos.y /= Screen.height;
            InputParams.x = pos.x;
            InputParams.y = pos.y;
            PlayWaveEffect();
        }
    }

    void PlayWaveEffect()
    {
        playWaveEffect = true;
        curWaveDistance = MinWaveDistance; 
    }

}
