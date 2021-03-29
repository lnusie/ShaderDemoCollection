using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShooter : MonoBehaviour
{
    public int HitPower
    {
        get { return Random.Range(m_MinHitPower, m_MaxHitPower); }
    }

    public GameObject m_Bullet;

    public float m_ShootForce = 1;

    public int m_MaxHitPower = 3;

    public int m_MinHitPower = 1;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0.2f;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var startPos = Camera.main.transform.position - Camera.main.transform.up;
                var bullect = GameObject.Instantiate(m_Bullet);
                bullect.transform.position = startPos;
                var rigidbody = bullect.GetComponent<Rigidbody>();
                var dir = hit.point - startPos;
                rigidbody.AddForce(dir * m_ShootForce);
            }

        }
    }
}
