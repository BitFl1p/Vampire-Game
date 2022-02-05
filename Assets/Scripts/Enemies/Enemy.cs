using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using VladsUsefulScripts;
[RequireComponent(typeof(Seeker)), RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    internal Rigidbody rb;
    internal Transform target;
    internal float lastMove = 1;
    public float speed, drag;
    public float maxSpeedMult;
    public float nextWaypointDistance = 3f;
    internal Path path;

    internal int currentWaypoint = 0;
    internal bool reachedEndOfPath, playerSeen;

    internal Seeker seeker;

    internal virtual void Start()
    {
        target = CharacterController3D.instance.transform;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody>();
        InvokeRepeating("UpdatePath", 0, 0.5f);
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
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerSeen = true;
            target = collision.transform;
        }
    }
    internal virtual void FixedUpdate()
    {
        if (playerSeen) FollowPlayer();
        if (rb.velocity.x != 0) lastMove = rb.velocity.x;
        if (lastMove < 0) transform.eulerAngles = new Vector2(0, 180);
        else transform.eulerAngles = new Vector2(0, 0);
    }
    internal virtual void FollowPlayer()
    {
        //base.FixedUpdate();
        if (path == null) return;
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else reachedEndOfPath = false;
        Vector3 force;
        if (reachedEndOfPath) force = (target.position - rb.position).normalized * speed;
        else force = (path.vectorPath[currentWaypoint] - rb.position).normalized * speed;
        force.y = 0;
        rb.velocity = Clampers.ClampedDrag(rb.velocity + force, drag, -speed * maxSpeedMult, speed * maxSpeedMult);

        if (Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance) currentWaypoint++;

    }
}
