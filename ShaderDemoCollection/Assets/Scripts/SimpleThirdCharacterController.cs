using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleThirdCharacterController : MonoBehaviour
{
    public CharacterController m_CharacterController;

    private Animator m_Animator;

    public float m_MoveSpeed = 1;

    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
    }

    Vector3 velocity = Vector3.zero;

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        velocity.x = horizontalInput * m_MoveSpeed;
        velocity.z = verticalInput * m_MoveSpeed;
        velocity.y = 0; 
        m_CharacterController.SimpleMove(velocity);
        transform.LookAt(transform.position + velocity, Vector3.up);
        m_Animator.SetFloat("MoveSpeed", velocity.magnitude);
    }

}
