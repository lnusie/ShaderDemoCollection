using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MyProjector : MonoBehaviour
{
    public float m_NearPlane;
    public float m_FarPlane;
    public float m_FOV;
    public float m_AspectRatio;

    public Material m_Mat;

    private Matrix4x4 m_Object2ClipMatrix;

    public Camera MainCamera
    {
        get { return Camera.main; }
    }

    public void RenderObject(MeshRenderer meshRenderer)
    {
        CommandBuffer commandBuffer = new CommandBuffer();
        var meshFilter = meshRenderer.GetComponent<MeshFilter>();
        var mesh = meshFilter.sharedMesh;
        var trans = meshRenderer.transform;
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix.SetTRS(trans.position, trans.rotation, trans.localScale);
        commandBuffer.DrawMesh(mesh, matrix, m_Mat);
        MainCamera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, commandBuffer);
    }

    

}
