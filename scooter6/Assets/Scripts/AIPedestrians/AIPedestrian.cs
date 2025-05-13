using System.Collections;
using System.Threading;
using UnityEngine;
using UnityStandardAssets.Utility;

public class AIPedestrian : MonoBehaviour
{
    WaypointProgressTracker _tracker;
    public float moveSpeed = 1.6f;
    public float animationBlendFactor = 1f;
    float currentBlendFactor = 1f;
    public float HeightDampingFactor = 0.05f;
    public float SpeedDampingFactor = 0.05f;
    public Animator animator;
    public int frameCount = 0;
    public float targetSpeed;
    private float standingTimer;

    public void Init(WaypointCircuit circuit)
    {
        enabled = true;
        _tracker = GetComponent<WaypointProgressTracker>();
        _tracker.enabled = true;
        _tracker.Init(circuit);
        foreach (var waypoint in circuit.Waypoints)
        {
            var speedSettings = waypoint.GetComponent<PedestrianWaypoint>();
            if (speedSettings != null)
            {
                speedSettings.target = this;
            }
        }
        currentBlendFactor = animationBlendFactor;
        targetSpeed = moveSpeed;
    // animator.SetBool("Walking", moveSpeed > 0.01f);
    }

    // Smoothing rate dictates the proportion of source remaining after one second
    public static float Damp(float source, float target, float smoothing, float dt)
    {
        return Mathf.Lerp(source, target, 1 - Mathf.Pow(smoothing, dt));
    }

    private void Update()
    {
        
        if (targetSpeed != moveSpeed)
        {
            standingTimer = 0;
            if (targetSpeed < moveSpeed)
            {
                moveSpeed -= Time.deltaTime * 4;
                if (moveSpeed < 0.1f && targetSpeed < 0.1f)
                {
                    moveSpeed = targetSpeed;
                }
            } else
            {
                moveSpeed += Time.deltaTime * 1;
            }
        } else if (moveSpeed < 0)
        {
            return;
        }
        if (moveSpeed < 0.3f)
        {
            standingTimer += Time.deltaTime;
            if (standingTimer > 3) ResumeWalking();
        }
        //Code needed on every frame
        var pos = transform.position;
        pos += transform.forward * moveSpeed * Time.deltaTime;
        transform.position = pos;

        var steer = Quaternion.LookRotation(_tracker.target.position - transform.position, Vector3.up).eulerAngles;
        var rot = transform.eulerAngles;
        rot.y = steer.y;
        //Code needed on every other frame
        
        frameCount = 0;
        currentBlendFactor = Damp(currentBlendFactor, animationBlendFactor, SpeedDampingFactor, Time.deltaTime);
        animator.SetFloat("Speed", currentBlendFactor);
        transform.eulerAngles = rot;
        if (frameCount >= 40)
        {
            frameCount = Random.Range(0, 10);
            if (Physics.Raycast(pos + Vector3.up, Vector3.down, out RaycastHit hitInfo, 2))
            {
                pos.y = hitInfo.point.y;
            }
        } else
        {
            frameCount++;
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        PedestrianWaypoint speedSettings = other.GetComponent<PedestrianWaypoint>();
        if (speedSettings == null || (speedSettings.target != null && speedSettings.target != this))
        {
            return;
        }
        moveSpeed = speedSettings.targetSpeed;
        animationBlendFactor = speedSettings.targetBlendFactor;
    }
    public void StopWalking()
    {
        targetSpeed = 0.00f;
        //moveSpeed = 0.5f;
        //animator.SetBool("Walking", false);
        animator.enabled = false;
        _tracker.enabled = false;
    }

    public void ResumeWalking()
    {
        targetSpeed = 1.6f;
        moveSpeed = -0.2f;
        animator.enabled = true;
        //animator.SetBool("Walking", true);
        _tracker.enabled = true;
    }

    //returns false if pedestrian speed is lower than 0.03f or WaypointProgressTRacker is indicating that the pedestrian turned around)
    public bool IsWalking(){
        if (moveSpeed < 0.03f) return false;
        if (_tracker.IsTurningAround()) return false;
        return true;
    }
}
