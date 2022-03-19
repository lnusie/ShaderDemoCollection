﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public class MyProjector : MonoBehaviour
{
    public float m_NearPlane = 2;
    public float m_FarPlane = 30;
    public float m_FOV = 20;
    public float m_AspectRatio = 1;
    public float m_Size = 10;
    public Rect m_ViewportRect = new Rect(0, 0, 1, 1);

    public bool m_IsOrthographic = false;

    public GameObject go1;
    public GameObject go2;

    public Material Material => m_Material;
    [SerializeField]
    private Material m_Material;

    public Camera MainCamera
    {
        get { return Camera.main; }
    }

    public List<MeshRenderer> m_Obj2Render;

    private Matrix4x4 m_ProjectorMatrix;
    private CommandBuffer m_CommandBuffer;

    void Start()
    {
    }

    void Update()
    {
        CalcProjectorMatrix();
        Material.SetMatrix("_ProjectorMatrix", m_ProjectorMatrix);
        var renders = GetRenderObject();
        RenderObject(renders);
    }

    List<MeshRenderer> GetRenderObject()
    {
        //正常应该是获取视椎体内的物体进行渲染
        return m_Obj2Render;
    }

    public void CalcProjectorMatrix()
    {
        if (m_IsOrthographic)
        {
            //先计算世界->观察空间
            var matrixView2World = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            var matrixWorld2View = matrixView2World.inverse;
            Matrix4x4 m = Matrix4x4.identity;
            m.SetRow(2, new Vector4(0,0,-1,0));
            matrixWorld2View = m * matrixWorld2View;

            //再计算观察空间->裁剪空间
            Matrix4x4 matrixView2Clip = Matrix4x4.identity;
            float width = m_Size;
            float height = m_Size;
            matrixView2Clip.SetRow(0, new Vector4(2 / width, 0, 0, 0));
            matrixView2Clip.SetRow(1, new Vector4(0, 2 / height, 0, 0));
            matrixView2Clip.SetRow(2, new Vector4(0, 0, 2/(m_NearPlane - m_FarPlane), (m_NearPlane + m_FarPlane)/(m_NearPlane - m_FarPlane)));
            matrixView2Clip.SetRow(3, new Vector4(0, 0, 0, 1));

            Camera cam = GetComponent<Camera>();
            var matrixView2Clip2 = Matrix4x4.Ortho(-m_Size * 0.5f, m_Size * 0.5f, -m_Size * 0.5f, m_Size * 0.5f, m_NearPlane, m_FarPlane);
            m_ProjectorMatrix = matrixView2Clip2 * matrixWorld2View;
        }
    }

    public void RenderObject(List<MeshRenderer> renders)
    {
        if (m_CommandBuffer != null)
        {
            MainCamera.RemoveCommandBuffer(CameraEvent.AfterForwardAlpha, m_CommandBuffer);
        }
        else
        {
            m_CommandBuffer = new CommandBuffer();
            m_CommandBuffer.name = "My Projector";
        }
        m_CommandBuffer.Clear();
        foreach (var render in renders)
        {
            var meshFilter = render.GetComponent<MeshFilter>();
            var mesh = meshFilter.sharedMesh;
            var trans = render.transform;
            Matrix4x4 matrix = Matrix4x4.TRS(trans.position, trans.rotation, trans.localScale);
            m_CommandBuffer.DrawMesh(mesh, matrix, Material);
        }
        MainCamera.depthTextureMode |= DepthTextureMode.Depth;
        MainCamera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, m_CommandBuffer);
    }

    void OnDrawGizmos()
    {
        var matrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(1,1,2));
        Gizmos.DrawWireCube(-Vector3.back * 1f, new Vector3(0.5f, 0.5f, 0.5f));

        Gizmos.color = Color.yellow;
        if (!m_IsOrthographic)
        {
            Gizmos.DrawFrustum(Vector3.zero, m_FOV, m_FarPlane, m_NearPlane, m_AspectRatio);
        }
        else
        {
            //Gizmos.DrawCube(transform.position + transform.forward * (m_NearPlane + (m_NearPlane * m_FarPlane) * 0.5f), new Vector3(m_Size, m_Size, m_FarPlane - m_NearPlane));
            Gizmos.DrawWireCube(-Vector3.back * (m_NearPlane + (m_NearPlane + m_FarPlane) * 0.5f), new Vector3(m_Size, m_Size, m_FarPlane - m_NearPlane));
        
        }
        Gizmos.matrix = matrix;
    }



}
