using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 径向模糊效果
/// </summary>
public class PostEffectRadialBlur : PosetEffectBase
{
    public const string SHADER_PATH = "X_Shader/PostEffect/RadialBlur";

    public Vector4 InputParams;

    public int MoveDir = 0;

    public Camera m_Camera;

    void Start()
    {
        m_Camera = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            m_Camera.fieldOfView = 60;
            InputParams.z = -5;
            MoveDir = 1;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            m_Camera.fieldOfView = 20;
            InputParams.z = 5;
            //InputParams.z = 1;          
            MoveDir = -1;
        }
        if (MoveDir == 1)
        {
            m_Camera.fieldOfView -= 1;
            //InputParams.z = InputParams.z - 1;
            if (m_Camera.fieldOfView <= 20)
            {
                MoveDir = 0;
            }
        }
        else if (MoveDir == -1)
        {
            m_Camera.fieldOfView += 1;
           // InputParams.z = InputParams.z + 1;
            if (m_Camera.fieldOfView >= 60)
            {
                MoveDir = 0; 
            }
        }


    }

    void MoveForward()
    {
        MoveDir = 1;

    }

    void MoveBack()
    {
        MoveDir = -1;


    }

    protected override void OnRenderImage(RenderTexture source, RenderTexture destTexture)
    {
        if (MoveDir != 0)
        {
            RadialBlur(source, destTexture, InputParams);
        }
        else
        {
            Graphics.Blit(source,destTexture);
        }
    }

    protected static void RadialBlur(RenderTexture source, RenderTexture destTexture, Vector4 inputParams)
    {
        var postMat = GetMaterial(SHADER_PATH);
        postMat.SetVector("_InputParams", inputParams);
        Graphics.Blit(source, destTexture, postMat);

    }
}
