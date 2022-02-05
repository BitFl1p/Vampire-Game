using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CharacterController3D : MonoBehaviour
{
    public GameObject honk;
    public GameObject trail;
    public int studs;
    public float maxSpeed, maxSprintSpeed;
    //public CinemachineTargetGroup tGroup;
    public Transform cam, enemy;
    public InputActionReference inputFile;
    public float turnSmoothTime;
    float dashCount;
    public bool lockedOn;
    bool lockLastFrame;
    [HideInInspector] public float speed;
    float turnSmoothVelocity;
    bool dashed, dashed2;
    Vector3 moveDir;
    Vector3 direction3;
    Rigidbody rb;
    Animator anim;
    InputMaster input;
    #region Singleton Shit
    public static CharacterController3D instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(instance);
    }
    #endregion
    #region Setup
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }
    void OnEnable()
    {
        input = new InputMaster();
        input.Enable();
    }
    void OnDisable()
    {
        input.Disable();
    }
    #endregion
    void Update()
    {
        UI.instance.studCount.text = studs.ToString().PadLeft(10 - studs.ToString().ToCharArray().Count(), '0');
        Vector3 direction = new Vector3(input.Player.Move.ReadValue<Vector2>().x, 0, input.Player.Move.ReadValue<Vector2>().y); //get directional input from player and assign it to the right axes
        if (!lockedOn)
        {
            if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, Mathf.Infinity, cam.gameObject.layer))
            {
                if (hit.collider.TryGetComponent(out Enemy enemy))
                {
                    this.enemy = enemy.transform;
                }
                else enemy = null;

            }
            else enemy = null;
        }

        if (enemy && !lockLastFrame && input.Player.LockOn.controls.Any(c => c.IsPressed())) lockedOn = !lockedOn;
        if (!enemy) lockedOn = false;
        if (lockedOn) 
        {
            CinemachineFreeLook freelook = VirtualCameraSingleton.instance.GetComponent<CinemachineFreeLook>();
            Vector3 toEnemy = (transform.position - enemy.position).normalized;
            float targetAngle = Mathf.Lerp(freelook.m_XAxis.Value, (Mathf.Atan2(-toEnemy.x, -toEnemy.z) * Mathf.Rad2Deg), turnSmoothTime);
            targetAngle = targetAngle > 180 ? targetAngle - 360 : targetAngle < -180 ? targetAngle + 360 : targetAngle;
            freelook.m_XAxis.Value = targetAngle;
            freelook.m_YAxis.Value = .6f;
            VirtualCameraSingleton.instance.GetComponent<CinemachineInputProvider>().XYAxis = null;
            enemy.GetComponent<Outline>().enabled = true;
            enemy.GetComponent<Outline>().OutlineWidth = 10 - Vector3.Distance(enemy.position, transform.position);
        }
        else
        {
            if (enemy)
            {
                enemy.GetComponent<Outline>().OutlineWidth = 0;
                enemy.GetComponent<Outline>().enabled = false;
            }
            VirtualCameraSingleton.instance.GetComponent<CinemachineInputProvider>().XYAxis = inputFile;
        }
        //else tGroup.RemoveMember(enemy);
        if (direction.magnitude >= 0.1f && anim.GetInteger("Attack") == 0)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            if(!lockedOn)transform.eulerAngles = new Vector3(0, angle, 0);
            else
            {
                transform.LookAt(enemy);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
            moveDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            // I'm not even gonna try with this one. figure out angles for character to move using weird maths I found on brackeys
        }
        lockLastFrame = input.Player.LockOn.controls.Any(c => c.IsPressed());
    }
    void FixedUpdate()
    {
        Vector2 localVelocity = new Vector2(Vector3.Dot(rb.velocity, transform.right), Vector3.Dot(rb.velocity, transform.forward)); //figure out velocity relative to the player
        if (dashCount < .6f && anim.GetInteger("Attack") == 0)
        {
            if (dashed2) 
            { 
                speed = 0; 
                dashed2 = false; 
            }
            Vector2 direction = input.Player.Move.ReadValue<Vector2>(); //get directional input from player
            direction3 = new Vector3(direction.x, 0, direction.y);
            direction3 = Quaternion.Euler(0, cam.eulerAngles.y, 0) * direction3;
        }
        if (input.Player.Dash.controls.Any(c => c.IsPressed()) && !dashed && direction3.magnitude > 0) 
        {
            dashed2 = true;
            dashed = true;
            dashCount = .8f;
            speed = maxSprintSpeed * 3;
            SpeedCalc(direction3, maxSprintSpeed * 4);
        }
        else
        {
            if (input.Player.Sprint.controls.Any(c => c.IsPressed()) && !anim.GetBool("Block")) SpeedCalc(direction3, maxSprintSpeed); //Sprint
            else SpeedCalc(direction3, maxSpeed); //Walk
        }
        if (dashCount >= .6f) 
        {
            trail.GetComponent<TrailRenderer>().time = 0.2f;
            var emission = trail.GetComponent<ParticleSystem>().emission;
            emission.enabled = true;
            honk.SetActive(false);
        }
        else
        {
            trail.GetComponent<TrailRenderer>().time = -0.2f;
            var emission = trail.GetComponent<ParticleSystem>().emission;
            emission.enabled = false;
            honk.SetActive(true);
        }
        if (dashCount >= 0) dashCount -= Time.deltaTime;
        else dashed = false;



        anim.SetFloat("X", Mathf.Lerp(anim.GetFloat("X"), localVelocity.x, turnSmoothTime)); //animation shit
        anim.SetFloat("Y", Mathf.Lerp(anim.GetFloat("Y"), localVelocity.y, turnSmoothTime));
    }

    public void SpeedCalc(Vector3 direction, float maxSpeed)
    {
        float beforeSpeed = speed;
        if (anim.GetInteger("Attack") != 0) direction = moveDir;
        if (direction.magnitude >= 0.1f) speed += maxSpeed / 10; //if input exists, increase speed
        speed = Drag(speed, maxSpeed / 20); //decrease speed by drag
        speed = SmoothClamp(speed, -maxSpeed, maxSpeed, maxSpeed / 5); //clamp speed to max speed so it doesn't go over
        speed = Mathf.Lerp(beforeSpeed, speed, 0.4f);
        rb.velocity = new Vector3(direction.x * speed, rb.velocity.y, direction.z * speed); //set velocity to speed
    }
    float Drag(float val, float drag)
    {
        if (val >= 0) val -= drag * .8f;
        else val += drag * .8f;
        if (val > -drag && val < drag) val = 0;
        return val;
    }
    float SmoothClamp(float val, float min, float max, float drag)
    {
        if (val > max) val -= drag;
        if (val < min) val += drag;
        return val;
    }
}
