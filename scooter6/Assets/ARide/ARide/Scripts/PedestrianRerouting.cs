using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityStandardAssets.Utility;

//Pedestrians will turn around if another pedestrian is standing in front
public class PedestrianRerouting : MonoBehaviour
{
    public GameObject sensorPrefab;
    public GameObject carSensorPrefab;
    WaypointProgressTracker tracker;
    Transform target;
    private GameObject watchForCars;
    private AIPedestrian pedestrianAI;
    private SphereCollider sphereCollider;
    private bool waiting;
    private float waitTimer;

    // Start is called before the first frame update
    void Start()
    {
        waitTimer = 0;
        tracker = gameObject.GetComponent<WaypointProgressTracker>();
        target = tracker.target;
        GameObject newSensor = Instantiate(sensorPrefab, transform);
        newSensor.GetComponent<ChildRerouter>().parentScript = this;
        watchForCars = Instantiate(carSensorPrefab, transform);
        watchForCars.GetComponent<StreetCrosserDetector>().pedestrianScript = gameObject.GetComponent<AIPedestrian>();
        watchForCars.SetActive(false);
        pedestrianAI = gameObject.GetComponent<AIPedestrian>();
        sphereCollider = gameObject.GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer < 0)
            {
                pedestrianAI.ResumeWalking();
                waiting = false;
                sphereCollider.enabled = true;
            }
        }
        
    }
    
    public void CarHit(Collider other)
    {
        if (other.gameObject.tag.Equals("ManualCar") && !tracker.IsTurningAround())
        {

            target.position = transform.position - (target.position - transform.position).normalized * 2;
            tracker.TurnAround();
        }
        else if (other.gameObject.tag.Contains("LostCitizens") && !other.gameObject.Equals(gameObject) && Vector3.Dot(other.transform.forward, transform.forward) > 0.9f && Vector3.Distance(other.transform.position, (transform.position + transform.forward)) < Vector3.Distance(transform.position, (transform.position + transform.forward)))
        {
            if(!waiting)
            {
                waiting = true;
                waitTimer = 1f;
                pedestrianAI.StopWalking();
                sphereCollider.enabled = false;
            }
        }
    }

    public void LookForCars(bool activate, Vector3 turnTowards = default(Vector3))
    {
        if (activate)
        {
            watchForCars.SetActive(true);
            watchForCars.transform.forward = turnTowards;
            watchForCars.transform.position = transform.position + turnTowards.normalized * 3;
            sphereCollider.enabled = false;
        } else
        {
            watchForCars.SetActive(false);
            sphereCollider.enabled = true;
        }
    }
}
