using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTrans;

    public Vector3 distance;

    private void Start()
    {
        distance = playerTrans.position - transform.position;
    }

    void Update()
    {
        transform.LookAt(playerTrans.position + Vector3.one * 0.5f);
        transform.position = playerTrans.position - distance;
    }
}
