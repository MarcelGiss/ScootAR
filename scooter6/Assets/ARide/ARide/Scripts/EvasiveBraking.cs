using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Cars will break when detecting any obstructing traffic participant in their way
public class EvasiveBraking : MonoBehaviour
{
    [SerializeField]
    protected GameObject selfBrakingCollider;
    [SerializeField]
    protected EvasiveBraking otherBrakingScript;

    private float timeOfLastActivation = -2f;
    private float timeOfLastManualCarTouch = -2f;
    private float speedAtStart = -1;
    public bool detectingForPedestrians = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timeOfLastActivation + 0.4f < Time.realtimeSinceStartup && selfBrakingCollider.activeSelf)
        {
            selfBrakingCollider.GetComponent<SpeedSettings>().acceleration = 2f;
            selfBrakingCollider.GetComponent<SpeedSettings>().speed = Mathf.Min(GetSpeed() + 0.09f,21f);
        }
        if (timeOfLastActivation + 1f < Time.realtimeSinceStartup)
        {
            selfBrakingCollider.GetComponent<SpeedSettings>().speed = Mathf.Clamp(selfBrakingCollider.GetComponent<SpeedSettings>().speed + 1, 16f, 22f);
        }
        if (timeOfLastActivation + 2f < Time.realtimeSinceStartup)
        {
            selfBrakingCollider.SetActive(false);
            speedAtStart = -1;
        }
    }

    public float GetSpeed()
    {
        return transform.parent.parent.GetComponent<AICar>().speed;
        //return current speed from this car [retired method]
        return transform.parent.parent.GetComponent<Rigidbody>().linearVelocity.magnitude;
    }

    // if detecting object in front: brake;   uses seperate triggers for cars and pedestrians
    void OnTriggerStay(Collider other)
    {

        if ((other.tag.Equals("Car") || other.tag.Equals("ManualCar") || other.tag.Equals("Player")) && !detectingForPedestrians && !other.transform.Equals(transform.parent.parent) && !other.name.Contains("CarDetector") && transform.parent.parent.gameObject.GetComponent<CarLauncher>() == null)
        {
            timeOfLastActivation = Time.realtimeSinceStartup;
            selfBrakingCollider.SetActive(true);
            float otherSpeed = 0f;
            if (other.tag.Equals("Car")) otherSpeed = other.transform.parent.GetChild(0).GetComponent<EvasiveBraking>().GetSpeed();
            else if (other.tag.Equals("ManualCar"))
            {
                //slow down depending on the speed of car ahead
                otherSpeed = other.GetComponent<AICar>().speed -2f;
                timeOfLastManualCarTouch = Time.realtimeSinceStartup;
            }
            else if (other.tag.Equals("Player"))
            {
                otherSpeed = 3;
            }
            if (speedAtStart <= 0) speedAtStart = GetSpeed();
            speedAtStart -= 2f;
            float newSpeed = Mathf.Max(speedAtStart, 0f, otherSpeed - 3f);
            selfBrakingCollider.GetComponent<SpeedSettings>().acceleration = -9f;
            selfBrakingCollider.GetComponent<SpeedSettings>().speed = Mathf.Min(17,newSpeed);

        }

        //extra careful for pedestrians
        if (other.tag.Contains("LostCitizens") && detectingForPedestrians){
            if (other.TryGetComponent<AIPedestrian>(out AIPedestrian aiPEd) && !aiPEd.IsWalking()) return;
            timeOfLastActivation = Time.realtimeSinceStartup;
            selfBrakingCollider.SetActive(true);
            float newSpeed = Mathf.Max(GetSpeed() - 2f, 0f);
            selfBrakingCollider.GetComponent<SpeedSettings>().acceleration = -9f;
            selfBrakingCollider.GetComponent<SpeedSettings>().speed = Mathf.Min(15,newSpeed);
        }
        
        //sinc both braking triggers
        if (otherBrakingScript.selfBrakingCollider.activeSelf && selfBrakingCollider.activeSelf && detectingForPedestrians)
        {
            selfBrakingCollider.GetComponent<SpeedSettings>().speed = Mathf.Min(otherBrakingScript.selfBrakingCollider.GetComponent<SpeedSettings>().speed - 3f,selfBrakingCollider.GetComponent<SpeedSettings>().speed - 3f);
            selfBrakingCollider.GetComponent<SpeedSettings>().acceleration = -9f;
        }
        

    }
}
