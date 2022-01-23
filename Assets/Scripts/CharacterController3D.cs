using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladsUsefulScripts;

public class CharacterController3D : MonoBehaviour
{
    public float maxSpeed;
    public Transform cam;
    public float turnSmoothTime;

    float speed;
    float turnSmoothVelocity;
    Vector3 moveDir;
    Rigidbody rb;
    Animator anim;
    InputMaster input;

    #region Setup
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        input = new InputMaster();
        input.Enable();
    }
    private void OnDisable()
    {
        input.Disable();
    }
    #endregion
    void Update()
    {
        Vector3 direction = new Vector3(input.Player.Move.ReadValue<Vector2>().x, 0, input.Player.Move.ReadValue<Vector2>().y);
        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.eulerAngles = new Vector3(0, angle, 0);
            moveDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        }
    }
    void FixedUpdate()
    {
        
        Vector2 direction = input.Player.Move.ReadValue<Vector2>();
        if (direction.magnitude >= 0.1f) speed += maxSpeed / 10; 
        speed -= rb.drag;
        speed = Mathf.Clamp(speed, 0, maxSpeed);
        rb.velocity = moveDir * speed;
        Vector2 localVelocity = new Vector2(Vector3.Dot(rb.velocity, transform.right), Vector3.Dot(rb.velocity, transform.forward));
        anim.SetFloat("X", localVelocity.x);
        anim.SetFloat("Y", localVelocity.y);
    }
}
