using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraSetting : MonoBehaviour
{
    public bool m_RenderDepth;

    void OnEnable()
    {
        Debug.LogError("OnEnable");
        GetComponent<Camera>().depthTextureMode = m_RenderDepth ? DepthTextureMode.Depth : DepthTextureMode.None;
    }
}
