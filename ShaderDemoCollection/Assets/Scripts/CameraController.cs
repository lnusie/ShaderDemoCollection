using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera m_Camera;
    public float m_ZoomSpeed = 1;

    void Start()
    {
        m_Camera = GetComponent<Camera>();
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01)//这个是鼠标滚轮响应函数 
        {
            m_Camera.transform.position += m_Camera.transform.forward * m_ZoomSpeed * (scroll > 0 ? 1 : -1);
        }
    }


}
