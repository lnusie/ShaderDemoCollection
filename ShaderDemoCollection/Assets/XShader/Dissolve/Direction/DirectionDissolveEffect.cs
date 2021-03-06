﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionDissolveEffect : MonoBehaviour
{
    public float m_DissolveMaxValue = 1;

    public float m_DissolveMinValue = 0;

    [Range(0,1)]
    public float m_DissolveValue = 0;

    public float m_DissolveSpeed = 1;

    public bool m_IsHorizontal;

    [Range(0, 1)]
    public float m_EdgeLength = 0;

    public Material m_DissolveMaterial;

    public Material DissolveMaterial
    {
        get
        {
            if (m_DissolveMaterial == null)
            {
                MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
                if(meshRenderer != null)
                {
                    m_DissolveMaterial = meshRenderer.material;
                }
            }
            return m_DissolveMaterial;
        }
    }

    void Start()
    {
        ResetMaterial();
        StartCoroutine(StartDissolve());
    }

    void GetModelInfo()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (!meshFilter) return;
        Vector3[] vertices = meshFilter.mesh.vertices;
        float maxPos = float.MinValue;
        float minPos = float.MaxValue;
        for (int i = 0; i < vertices.Length; i++)
        {
            float pos = m_IsHorizontal ? vertices[i].x : vertices[i].y;
            if (pos > maxPos)
            {
                maxPos = vertices[i].y;
            }
            if (pos < minPos)
            {
                minPos = vertices[i].y;
            }
        }
        m_DissolveMaxValue = maxPos;
        m_DissolveMinValue = minPos;
    }

    public void ResetMaterial()
    {
        if (DissolveMaterial == null) return;
        DissolveMaterial.SetFloat("_EdgeLength", 0);
        DissolveMaterial.SetFloat("_DissolveValue", 1);
        DissolveMaterial.SetFloat("_DissolveMinValue", float.MinValue);
        DissolveMaterial.SetFloat("_DissolveMaxValue", float.MaxValue);
    }

    IEnumerator StartDissolve()
    {
        yield return new WaitForSeconds(2);
        if (DissolveMaterial == null) yield break;
        GetModelInfo();
        DissolveMaterial.SetFloat("_DissolveMinValue", m_DissolveMinValue);
        DissolveMaterial.SetFloat("_DissolveMaxValue", m_DissolveMaxValue);
        DissolveMaterial.SetFloat("_EdgeLength", m_EdgeLength);
        DissolveMaterial.SetFloat("_IsHorizontal", m_IsHorizontal ? 1 : 0);

        m_DissolveValue = 1;
        while (m_DissolveValue > 0)
        {
            DissolveMaterial.SetFloat("_DissolveValue", m_DissolveValue);
            m_DissolveValue -= Time.deltaTime * m_DissolveSpeed;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield break;
    }
}
