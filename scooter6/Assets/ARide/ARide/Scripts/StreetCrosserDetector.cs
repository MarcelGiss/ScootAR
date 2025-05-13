using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//if activate will scan for cars in the area to determine safe crossing
public class StreetCrosserDetector : MonoBehaviour
{
    public AIPedestrian pedestrianScript;
    private float waitTimer;
    private bool waiting = false;

    private void Start()
    {
        waitTimer = 0;
    }

    private void Update()
    {
        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer < 0.3f) waiting = false;
            if (waitTimer < 0)
            {   
                ChangeStatus(false);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("ManualCar"))
        {
            if (other.GetComponent<Rigidbody>().linearVelocity.magnitude <= 0.5f || waiting) return;
            if (Vector3.Dot(other.transform.forward,transform.position-other.transform.position) > -0.1) ChangeStatus(true);
        }
    }
    //stop walking if car is detected - resume walking once car passed
    private void ChangeStatus(bool changeTo)
    {
        waiting = changeTo;
        if (changeTo)
        {
            pedestrianScript.StopWalking();

            waitTimer = 0.6f;
        } else
        {
            pedestrianScript.ResumeWalking();
        }
    }
}
