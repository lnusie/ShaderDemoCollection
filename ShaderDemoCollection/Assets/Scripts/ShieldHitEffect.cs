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

    public GameObject m_Bullet;
    public float m_ThrowForce = 1;

    private Material m_Material;
    public float m_WaveValue;
    public float m_HitFadeSpeed = 1;
    public int m_MaxHitPoint = 3;



    private List<HitInfo> m_HitInfos = new List<HitInfo>();

    void Start()
    {
        m_Material = GetComponent<MeshRenderer>().material;
        for (int i = 0; i < m_MaxHitPoint; i++)
        {
            GameObject go = new GameObject("HitPoint_" + i);
            go.transform.SetParent(transform);
            m_HitInfos.Add(new HitInfo() { m_Trans = go.transform});
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var startPos = Camera.main.transform.position - Camera.main.transform.up;
                var bullect = GameObject.Instantiate(m_Bullet);
                bullect.transform.position = startPos;
                var rigidbody = bullect.GetComponent<Rigidbody>();
                var dir = hit.point - startPos;
                rigidbody.AddForce(dir * m_ThrowForce);
            }
          
        }
        UpdateHitPoints();
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
                info.m_HitStrength = Random.Range(0.3f, 1.2f);
                return;
            }
            if (info.m_HitStrength < minStrength)
            {
                minStrength = info.m_HitStrength;
                minStrengthInfo = info;
            }
        }
        minStrengthInfo.m_Trans.position = hitPos;
        minStrengthInfo.m_HitStrength = Random.Range(0.3f, 1.2f);
        return;
    }


}
