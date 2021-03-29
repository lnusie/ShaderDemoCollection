using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHitEffect : MonoBehaviour
{
    class HitInfo
    {
        public Transform m_Trans;
        public float m_HitStrength;
    }

    private Material m_Material;
    public float m_WaveValue;
    public float m_HitFadeSpeed = 1;
    public int m_MaxHitPoint = 3;

    private List<HitInfo> m_HitInfos = new List<HitInfo>();
    private TestShooter m_Shooter;

    public Transform m_TestTrans;


    void Start()
    {
        m_Material = GetComponent<MeshRenderer>().material;
        for (int i = 0; i < m_MaxHitPoint; i++)
        {
            GameObject go = new GameObject("HitPoint_" + i);
            go.transform.SetParent(transform);
            m_HitInfos.Add(new HitInfo() { m_Trans = go.transform});
        }
        m_Shooter = Camera.main.gameObject.GetComponent<TestShooter>();
    }

    void Update()
    {
       
       
        UpdateHitPoints();
        //var hitPos = m_TestTrans.position;
        //Vector4 v4 = new Vector4(hitPos.x, hitPos.y, hitPos.z, m_MaxHitPower);
        //m_Material.SetVector("_HitPoint0", v4);
    }

    void OnCollisionEnter(Collision collision)
    {
        AddHitPoint(collision.contacts[0].point);
    }

    void UpdateHitPoints()
    {
        for (int i = 0; i < m_HitInfos.Count; i++)
        {
            if (m_HitInfos[i].m_HitStrength <= 0) continue;
            var info = m_HitInfos[i];
            var hitPos = info.m_Trans.position;
            Vector4 v4 = new Vector4(hitPos.x, hitPos.y, hitPos.z, info.m_HitStrength);
            m_Material.SetVector("_HitPoint" + i, v4);
            info.m_HitStrength -= Time.deltaTime * m_HitFadeSpeed;
        }
    }

    void AddHitPoint(Vector3 hitPos)
    {
        float minStrength = float.MaxValue;
        HitInfo minStrengthInfo = m_HitInfos[0];
        foreach (var info in m_HitInfos)
        {
            if (info.m_HitStrength <= 0)
            {
                info.m_Trans.position = hitPos;
                info.m_HitStrength = m_Shooter.HitPower;
                return;
            }
            if (info.m_HitStrength < minStrength)
            {
                minStrength = info.m_HitStrength;
                minStrengthInfo = info;
            }
        }
        minStrengthInfo.m_Trans.position = hitPos;
        minStrengthInfo.m_HitStrength = m_Shooter.HitPower;
        return;
    }


}
