using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XProjShaderManager : MonoBehaviour
{
    private Projector m_Projector;

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

    public Transform m_playerTrans;

    public Vector3 m_ProjectorPosition; //相对于角色的位置，可以根据不同地图配置

    //entityTrans会根据玩家的交互圈进行增删
    public Transform[] entityTrans;

    private int m_RenderTextureWidth = 512;

    private int m_RenderTextureHeight = 512;

    private bool m_RenderTextureDirty = false;

    public float m_MaxShowDistance = 10;

    [Range(0.01f, 1)]
    private float m_ShadowVisibleRange = 0.8f; //阴影显示范围（视口坐标下）

    public float ShadowVisibleRange
    {
        get => m_ShadowVisibleRange;
        set
        {
            m_ShadowVisibleRange = value;
            ProjectMat.SetFloat("_ShadowVisibleRange", m_ShadowVisibleRange);
        }
    }

    public int RenderTextureWidth
    {
        get => m_RenderTextureWidth;
        set
        {
            if (m_RenderTextureWidth != value)
            {
                m_RenderTextureWidth = value;
                m_RenderTextureDirty = true;
            }
        }
    }

    public int RenderTextureHeight
    {
        get => m_RenderTextureHeight;
        set
        {
            if (m_RenderTextureHeight != value)
            {
                m_RenderTextureHeight = value;
                m_RenderTextureDirty = true;
            }
        }
    }

    public Transform test;

    void Start()
    {
        m_ShadowCamera = GetComponent<Camera>();
        m_Projector = GetComponent<Projector>();
        if (m_Projector == null)
        {
            m_Projector = gameObject.AddComponent<Projector>();
            m_Projector.nearClipPlane = m_ShadowCamera.nearClipPlane;
            m_Projector.farClipPlane = m_ShadowCamera.farClipPlane;
            m_Projector.fieldOfView = m_ShadowCamera.fieldOfView;
            m_Projector.ignoreLayers = ~LayerMask.GetMask("Ground");
            m_Projector.orthographic = m_ShadowCamera.orthographic;
            //只要orthographicSize相等就能保证阴影能够对齐
            m_Projector.orthographicSize = m_ShadowCamera.orthographicSize;
        }
        m_ShadowCamera.SetReplacementShader(Shader.Find("X_Shader/Projector/SimpleShadowCaster"), "");
        m_ShadowCamera.cullingMask = LayerMask.GetMask("Entity");
        var maskTex = Resources.Load<Texture>("Textures/Mask");
        ProjectMat.SetTexture("_FalloffTex", maskTex);
        m_Projector.material = ProjectMat;
        ProjectMat.SetFloat("_ShadowVisibleRange", m_ShadowVisibleRange);
        CheckRTSize();
    }

    void OnDestroy()
    {
        if (m_RenderTexture != null)
        {
            RenderTexture.ReleaseTemporary(m_RenderTexture);
        }
    }

    void CheckRTSize()
    {
        if (m_RenderTexture == null || m_RenderTexture.width != m_RenderTextureWidth
            || m_RenderTexture.height != m_RenderTextureHeight)
        {
            m_RenderTexture = RenderTexture.GetTemporary(m_RenderTextureWidth, m_RenderTextureHeight, 0, RenderTextureFormat.R8);
            m_ShadowCamera.targetTexture = m_RenderTexture;
            m_ProjectMat.SetTexture("_ShadowTex", m_RenderTexture);
        }
    }

    void Update()
    {
        if (m_RenderTextureDirty) //RT尺寸可能需要改变，如根据低中高配设置
        {
            CheckRTSize();
            m_RenderTextureDirty = false;
        }
        m_ProjectMat.SetVector("_ProjectorPos", this.transform.position);
        UpdateProjectorCamera();
    }

    /// <summary>
    /// 1.根据开启动态阴影的实体数量动态调整相机Size
    /// 2.根据实体位置确定包围位置，进而确定相机位置
    /// </summary>
    void UpdateProjectorCamera()
    {
        Bounds totalBounds = GetLocalBounds(m_playerTrans.gameObject);
        foreach (var trans in entityTrans) 
        {
            if (Vector3.Distance(m_playerTrans.position, trans.position) <= m_MaxShowDistance)
            {
                var entityBounds = GetLocalBounds(trans.gameObject);
                totalBounds.Encapsulate(entityBounds);
            }
        }
        transform.position = totalBounds.center + m_ProjectorPosition;
        transform.LookAt(totalBounds.center);
        float maxDistance = Mathf.Sqrt(Mathf.Pow(totalBounds.size.x, 2) + Mathf.Pow(totalBounds.size.y, 2) + Mathf.Pow(totalBounds.size.z, 2));
        m_ShadowCamera.orthographicSize = maxDistance * 0.5f;
        m_Projector.orthographicSize = maxDistance * 0.5f;
    }

    private void OnDrawGizmos()
    {
        if (!m_ShadowCamera) return;
        Bounds totalBounds = GetLocalBounds(m_playerTrans.gameObject);
        foreach (var trans in entityTrans)
        {
            if (Vector3.Distance(m_playerTrans.position, trans.position) <= m_MaxShowDistance)
            {
                var entityBounds = GetLocalBounds(trans.gameObject);
                totalBounds.Encapsulate(entityBounds);
                DrawBound(entityBounds, Color.green);
            }
        }
        DrawBound(totalBounds, Color.yellow);
    }

    void DrawBound(Bounds bounds,Color color)
    {
        var tempColor = Gizmos.color;
        Gizmos.color = color;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        Gizmos.color = tempColor;
    }

    public Bounds GetLocalBounds(GameObject target, bool includeChildren = true)
    {
        Bounds bounds = default(Bounds);
        if (includeChildren)
        {
            Renderer[] renderers = target.gameObject.GetComponentsInChildren<Renderer>();
            if (renderers.Length != 0)
            {
                Vector3 center = target.transform.position;
                bounds = new Bounds(center, Vector3.zero);
                foreach (Renderer r in renderers)
                {
                    if (r.bounds != null)
                    {
                        bounds.Encapsulate(r.bounds);
                    }
                }
            }
        }
        else
        {
            Renderer renderer = target.GetComponentInChildren<Renderer>();
            if (renderer != null && renderer.bounds != null)
            {
                bounds = renderer.bounds;
            }
        }
        return bounds;
    }
}
