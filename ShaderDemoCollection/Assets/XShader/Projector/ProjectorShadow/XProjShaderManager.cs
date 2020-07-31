using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XProjShaderManager : MonoBehaviour
{
    private Projector m_Projector;

    public int m_RenderTextureSize;

    private Material m_ProjectMat;

    public Material ProjectMat
    {
        get
        {
            if (m_ProjectMat == null)
            {
                m_ProjectMat = new Material(Shader.Find("X_Shader/Projector/ProjectShadow"));
            }
            return m_ProjectMat;
        }
    }

    public RenderTexture m_RenderTexture;

    public Camera m_ShadowCamera;

    public int m_RenderTextureWidth = 512;
    public int m_RenderTextureHeight = 512;

    void Start()
    {
        m_Projector = GetComponent<Projector>();
        if (m_Projector == null)
        {
            m_Projector = gameObject.AddComponent<Projector>();
        }
        m_Projector.material = ProjectMat;
        m_ShadowCamera = GetComponent<Camera>();
        m_Projector.nearClipPlane = m_ShadowCamera.nearClipPlane;
        m_Projector.farClipPlane = m_ShadowCamera.farClipPlane;
        m_Projector.fieldOfView = m_ShadowCamera.fieldOfView;
        m_Projector.ignoreLayers = ~LayerMask.GetMask("Ground");
        m_ShadowCamera.SetReplacementShader(Shader.Find("X_Shader/Projector/SimpleShadowCaster"), "");
        CheckRTSize();
    }

    void Update()
    {
        m_ProjectMat.SetVector("_ProjectorPos", this.transform.position);
    }

    void CheckRTSize()
    {
        if (m_RenderTexture == null || m_RenderTexture.width != m_RenderTextureWidth 
            || m_RenderTexture.height != m_RenderTextureHeight)
        {
            m_RenderTexture = new RenderTexture(m_RenderTextureWidth, m_RenderTextureHeight, 0);
            m_ShadowCamera.targetTexture = m_RenderTexture;
            m_ProjectMat.SetTexture("_ShadowTex", m_RenderTexture);
        }
        
    }

}
