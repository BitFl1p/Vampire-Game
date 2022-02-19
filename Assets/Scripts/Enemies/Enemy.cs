using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using VladsUsefulScripts;
[RequireComponent(typeof(Seeker)), RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    public float timeBeforeCombo = 1f;
    float comboCount;
    internal Rigidbody rb;
    internal Animator anim;
    internal Transform target;
    internal float turnSmoothVelocity;
    public float turnSmoothTime;
    public float drag;
    private bool staggered;
    float staggerTimer;
    public bool Staggered
    {
        get { return staggered; }
        set
        {
            if (staggered == value) return;
            staggered = value;
            if (staggered == true)
            {
                comboCount = timeBeforeCombo;
                staggerTimer = 5;
                anim.SetBool("Staggered", true);
            }
            else
            {
                staggerTimer = 0;
                comboCount = timeBeforeCombo;
                anim.SetBool("Staggered2", false);
                anim.SetInteger("Attack", 0);
            }
        }
    }
    public Collider hitter;
    float speed;
    public float maxSpeed;
    public float nextWaypointDistance = 3f;
    internal Path path;
    internal Vector3 moveDir;
    internal int currentWaypoint = 0;
    internal bool reachedEndOfPath, playerSeen;

    internal Seeker seeker;

    internal virtual void Start()
    {
        comboCount = timeBeforeCombo;
        anim = GetComponent<Animator>();
        target = CharacterController3D.instance.transform;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody>();
        InvokeRepeating("UpdatePath", 0, 0.05f);
    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    void UpdatePath()
    {
        if (seeker.IsDone() && target) seeker.StartPath(rb.position, target.position, OnPathComplete);
    }
    protected void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerSeen = true;
            target = collision.transform;
        }
    }
    internal virtual void FixedUpdate()
    {
        if(staggerTimer >= 0)
        {
            staggerTimer -= Time.deltaTime;
        }
        else
        {
            Staggered = false;
        }
        if (playerSeen) FollowPlayer();
    }
    internal virtual void FollowPlayer()
    {
        Vector2 localVelocity = new Vector2(Vector3.Dot(rb.velocity, transform.right), Vector3.Dot(rb.velocity, transform.forward));
        anim.SetFloat("X", Mathf.Lerp(anim.GetFloat("X"), localVelocity.x, turnSmoothTime));
        anim.SetFloat("Y", Mathf.Lerp(anim.GetFloat("Y"), localVelocity.y, turnSmoothTime));
        if (comboCount > 0) comboCount -= Time.deltaTime;
        if(staggered)
        {
            moveDir = Vector3.zero;
            return;
        }
        if (path == null) return;
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else reachedEndOfPath = false;
        if (Vector3.Distance(target.position, transform.position) <= nextWaypointDistance)
        {
            if(anim.GetInteger("Attack") == 0 && comboCount <= 0) Attack();
            moveDir = Vector3.zero;
        }
        else moveDir = (path.vectorPath[currentWaypoint] - rb.position).normalized;
        moveDir.y = 0;
        Vector3 faceDir = (transform.position - target.transform.position).normalized;
        float targetAngle = Mathf.Atan2(faceDir.x, faceDir.z) * Mathf.Rad2Deg -180;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); 
        transform.eulerAngles = new Vector3(0, angle, 0);
        //rb.velocity = Clampers.ClampedDrag(rb.velocity + force, drag, -speed * maxSpeedMult, speed * maxSpeedMult);
        SpeedCalc(moveDir, maxSpeed);
        if (Vector3.Distance(rb.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance) currentWaypoint++;

    }
    void StaggerSwap()
    {
        anim.SetBool("Staggered", false);
        anim.SetBool("Staggered2", true);
    }
    void Attack()
    {
        anim.SetInteger("Attack", 1);
    }
    void ComboAttack()
    {
        anim.SetInteger("Attack", anim.GetInteger("Attack") + 1);
    }
    public void ComboDone()
    {
        comboCount = timeBeforeCombo;
        anim.SetInteger("Attack", 0);
    }
    void Vel()
    {
        speed += .2f;
        SpeedCalc(transform.forward, 1);
    }
    void ColliderOn()
    {
        hitter.enabled = true;
    }
    public void ColliderOff()
    {
        hitter.enabled = false;
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
