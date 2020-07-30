using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDissolveEffect : MonoBehaviour
{
    [Range(0, 1)]
    public float m_DissolveValue = 0;

    public float m_DissolveSpeed = 1;

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
                if (meshRenderer != null)
                {
                    m_DissolveMaterial = meshRenderer.material;
                }
            }
            return m_DissolveMaterial;
        }
    }

    private void Start()
    {
        ResetMaterial();
        StartCoroutine(StartDissolve());
    }

    public void ResetMaterial()
    {
        if (DissolveMaterial == null) return;
        DissolveMaterial.SetFloat("_EdgeLength", 0);
        DissolveMaterial.SetFloat("_DissolveValue", 1);
    }

    IEnumerator StartDissolve()
    {
        yield return new WaitForSeconds(2);
        if (DissolveMaterial == null) yield break;
        DissolveMaterial.SetFloat("_EdgeLength", m_EdgeLength);

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
