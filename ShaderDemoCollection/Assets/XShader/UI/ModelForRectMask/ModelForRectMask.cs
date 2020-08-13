using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UI;

public class ModelForRectMask : MonoBehaviour
{
    public RectMask2D mask;
    public Camera camera;

    void Start()
    {
        var renderers = transform.GetComponentsInChildren<Renderer>();
        RectTransform maskRectTrans = mask.transform as RectTransform;
        Vector3[] corners = new Vector3[4];
        maskRectTrans.GetWorldCorners(corners);//左下，左上，右上，右下
        for(int i = 0; i < corners.Length; i++)
        {
            corners[i] = camera.WorldToScreenPoint(corners[i]);
        }
        Vector4 clipRect = Vector4.zero;
        clipRect.x = corners[0].x;
        clipRect.y = corners[0].y;
        clipRect.z = corners[2].x;
        clipRect.w = corners[2].y;
        for (int i = 0; i < renderers.Length; i++)
        {
            var mat = renderers[i].material;
            mat.renderQueue = 3000;
            mat.SetVector("_ClipRect", clipRect);
            mat.EnableKeyword("UI_CLIP_RECT");
        }
    }

    void Update()
    {
        if(!Input.GetKeyDown(KeyCode.T)) return;;
        var renderers = transform.GetComponentsInChildren<Renderer>();
        RectTransform maskRectTrans = mask.transform as RectTransform;
        Vector3[] corners = new Vector3[4];
        maskRectTrans.GetWorldCorners(corners);//左下，左上，右上，右下
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = camera.WorldToScreenPoint(corners[i]);
        }
    
        Vector4 clipRect = Vector4.zero;
        clipRect.x = corners[0].x;
        clipRect.y = corners[0].y;
        clipRect.z = corners[2].x;
        clipRect.w = corners[2].y;
        Debug.Log("clipRect" + clipRect);

    }
}
