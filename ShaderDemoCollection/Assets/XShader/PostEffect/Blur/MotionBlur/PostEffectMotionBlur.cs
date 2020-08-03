using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 运动模糊
/// 原理：记录并传入上一帧与当前帧的矩阵（屏幕坐标->世界坐标的矩阵，该矩阵等于世界->观察空间，观察->裁剪空间两个矩阵相乘的逆矩阵）
/// 将屏幕坐标+深度值 转化为上一帧和当前帧的世界坐标，进而求出运动方向。
/// </summary>
public class PostEffectMotionBlur : PosetEffectBase
{
    public const string SHADER_PATH = "X_Shader/PostEffect/MotionBlur";

    public Camera m_Camera;

    public int BlurSize = 1;

    public Matrix4x4 previousViewProjectionMatrix;

    void Awake()
    {
        m_Camera = GetComponent<Camera>();
        m_Camera.depthTextureMode |= DepthTextureMode.Depth;
    }

    protected override void OnRenderImage(RenderTexture source, RenderTexture destTexture)
    {
        Material mat = GetMaterial(SHADER_PATH);
        mat.SetFloat("_BlurSize", BlurSize);
        mat.SetMatrix("_PreviousViewProjectionMatrix", previousViewProjectionMatrix);
        Matrix4x4 curViewProjectionMatrix = m_Camera.projectionMatrix * m_Camera.worldToCameraMatrix ;
        var curViewProjectionInverseMatrix = curViewProjectionMatrix.inverse;
        mat.SetMatrix("_CurViewProjectionInverseMatrix",curViewProjectionInverseMatrix);
        previousViewProjectionMatrix = curViewProjectionMatrix;
        Graphics.Blit(source, destTexture,mat);
    }

//    protected static void MotionBlur(RenderTexture source, RenderTexture destTexture,Matrix4x4 curMaterial, int blurSize)
//    {
//      
//
//
//    }
}