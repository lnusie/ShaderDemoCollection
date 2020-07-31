using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDissolve : MonoBehaviour
{
    public float m_DissolveRadius;

    public float m_VisibleDistance = 5;

    public Transform m_PlayerTrans;

    public float m_DissolveHeight = 2;

    void Start()
    {
        Shader.SetGlobalFloat("_G_DissolveRadius", m_DissolveRadius);
    }

    void Update()
    {
        var distance = Vector3.Distance(m_PlayerTrans.position, transform.position);
        

        Debug.Log(" distance : " + distance);
        distance -= m_VisibleDistance;
        Shader.SetGlobalFloat("_G_DissolveDistance", distance);
        Shader.SetGlobalFloat("_G_DissolveHeight", m_PlayerTrans.position.y + m_DissolveHeight);
    }
}
