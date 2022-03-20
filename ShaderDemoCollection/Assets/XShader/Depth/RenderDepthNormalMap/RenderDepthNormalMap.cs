using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class RenderDepthNormalMap : MonoBehaviour
{
    private Camera m_Camera;

    private RenderTexture m_RenderTexture;

    private Shader m_Shader;

    public RawImage img;

    private Material m_PostImgMaterial;

    public bool m_OnlyDepth;

    void Start()
    {
        m_Camera = gameObject.AddComponent<Camera>();
        m_Camera.CopyFrom(Camera.main);
        m_Camera.backgroundColor = new Color(0, 0, 0, 0);
        m_Camera.clearFlags = CameraClearFlags.SolidColor; 
        //m_Camera.SetReplacementShader();
        m_RenderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        m_Camera.targetTexture = m_RenderTexture;
        if (m_OnlyDepth)
        {
            m_Shader = Shader.Find("X_Shader/Test/RenderDepthMap");
        }
        else
        {
            m_Shader = Shader.Find("X_Shader/Test/RenderDepthNormalMap");
        }
        //m_Camera.SetReplacementShader(m_Shader, "RenderType");//SetReplacementShader 只需设置一次会一直用指定shader渲染，调用ResetReplacementShader可重置
        img.texture = m_RenderTexture;

        if (m_OnlyDepth)
        {
            m_PostImgMaterial = new Material(Shader.Find("X_Shader/Test/DecodeDepthMap"));
        }
        else
        {
            m_PostImgMaterial = new Material(Shader.Find("X_Shader/Test/DecodeDepthNormalMap"));
        }
    }

    void OnPreRender()//在渲染场景前调用
    {
        m_Camera.RenderWithShader(m_Shader, "RenderType");//RenderWithShader调用一次则渲染一次
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, m_PostImgMaterial);
    }
}
