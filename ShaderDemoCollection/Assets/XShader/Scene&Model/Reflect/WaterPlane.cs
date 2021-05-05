using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WaterPlane : MonoBehaviour
{
    public RawImage m_RawImage;
    public Camera m_SourceCam;
    public Camera m_ReflectCam;
    public int m_RTWidth = 1024;
    private RenderTexture m_ReflectRT;
    private Material m_Material;
    private Matrix4x4 m_ReflectMatrix;

    public int m_MaxWave;
    private List<WaveInfo> m_Waves;

    void Start()
    {
        GameObject go = new GameObject("ReflectCamera");
        m_ReflectCam = go.AddComponent<Camera>();
        m_ReflectCam.CopyFrom(m_SourceCam);
        m_ReflectCam.clearFlags = CameraClearFlags.SolidColor;

        RenderTexture rt = RenderTexture.GetTemporary(m_RTWidth, m_RTWidth * Screen.height / Screen.width);
        m_ReflectCam.targetTexture = rt;
        //m_RawImage.texture = rt;
        m_ReflectRT = rt;
        m_ReflectCam.transform.position = m_SourceCam.transform.position;
        m_Material = GetComponent<MeshRenderer>().sharedMaterial;
        m_Material.SetTexture("_MainTex", m_ReflectRT);

        m_Waves = new List<WaveInfo>();
        for (int i = 0; i < m_MaxWave; i++)
        {
            m_Waves.Add(new WaveInfo());
        }
    }

    void Update()
    {
        m_ReflectCam.enabled = true;
        Vector3 normal = transform.up;
        Vector3 pos = transform.position;
        float distance = -Vector3.Dot(normal, pos);
        m_ReflectMatrix = CalcReflectMatrix(normal, distance);

        m_ReflectCam.worldToCameraMatrix = m_SourceCam.worldToCameraMatrix * m_ReflectMatrix;

        GL.invertCulling = true;
        m_ReflectCam.Render();
        GL.invertCulling = false;
        m_ReflectCam.enabled = false;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                CreateWave(hitInfo.point);
            }
        }
        UpdateWaves();
    }

    void CreateWave(Vector3 centerPos)
    {
        for (int i = 0; i < m_Waves.Count; i++)
        {
            var wave = m_Waves[i];
            if (wave.m_State == 0)
            {
                wave.m_Vect = centerPos;
                wave.m_State = 1;
                wave.m_Range = 1;//Random.Range(0.2f, 2);
                wave.m_WaveSpeed = 1;// Random.Range(1, 1.2f);
                m_Material.SetFloat("_WaveRange" + i, wave.m_Range);
                return;
            }
        }
    }

    void UpdateWaves()
    {
        for (int i = 0; i < m_Waves.Count; i++)
        {
            var wave = m_Waves[i];
            if (wave.m_State == 1)
            {
                bool end = wave.Update();
                if (end)
                {
                    wave.m_Vect.w = 999;
                    m_Material.SetVector("_Wave" + i, wave.m_Vect);
                    wave.m_State = 0;
                    continue;
                } 
                m_Material.SetVector("_Wave" + i, wave.m_Vect);
            }
        }
    }

    Matrix4x4 CalcReflectMatrix(Vector3 normal, float d)
    {
        Matrix4x4 m = Matrix4x4.identity;
        m.m00 = 1 - 2 * normal.x * normal.x;
        m.m01 = -2 * normal.y * normal.x;
        m.m02 = -2 * normal.z * normal.x;
        m.m03 = -2 * d * normal.x;

        m.m10 = -2 * normal.x * normal.y;
        m.m11 = 1 - 2 * normal.y * normal.y;
        m.m12 = -2 * normal.z * normal.y;
        m.m13 = -2 * d * normal.y;

        m.m20 = -2 * normal.x * normal.z;
        m.m21 = -2 * normal.y * normal.z;
        m.m22 = 1 - 2 * normal.z * normal.z;
        m.m23 = -2 * d * normal.z;

        m.m30 = 0;
        m.m31 = 0;
        m.m32 = 0;
        m.m33 = 1;

        return m;
    }

    void OnDestroy()
    {
        if (m_ReflectRT != null)
        {
            RenderTexture.ReleaseTemporary(m_ReflectRT);
            m_ReflectRT = null;
        }
    }

    public class WaveInfo
    {
        public int m_State;
        public Vector4 m_Vect;
        public float m_WaveSpeed;
        public float m_Range;

        public bool Update()
        {
            m_Vect.w += m_WaveSpeed * Time.deltaTime;
            return m_Vect.w > m_Range;
        }
    }
       
}
