using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


//steering the e-scooter or any other vehicle --- used for simulator input
public class EScooterNew : MonoBehaviour
{
    //Add the controller tracking rotation of the real handlebar
    public Transform steeringController;
    //Add the second controller if you are not sure which one will be active
    public Transform steeringBackup;
    public bool showSpeed;
    public bool tiltingScooter = true;
    //maximum Speed used for calculation -- actual maxSpeed will be lower
    public float maxSpeed = 50f;
    //Acceleration
    public float maxMotorTorque = 10f;
    //only needed for traveling with rigidbody
    public Transform centerOfMass;
    public Vector3 inertiaTensorVector;
    //phonetext
    public GameObject textGO;
    public Collider eScooterCollider;
    private float speedRatio;
    private float motor;
    //you can use this as a backup steering method - disable steering Controller input
    public float currentSteer;
    //Add the own rigidbody
    private Rigidbody bikeRigid;
    private float speed;
    private float totalDirectionRotation;
    //input acceleration -- manipulate this value
    public float arduinoAccelerate = 0;
    //input deceleration -- manipulate this value
    public float arduinoDeceleration = 0;
    private bool onStreet;
    private float onStreetTimer;
    public float currentSpeed = 0;
    private float transformTilt = 0;

    public Transform steeringMovable;
    public float raycastDistance = 1.0f; // Distance for raycasting -- ucan be used for staying close to the ground

    void Start()
    {
        Resources.UnloadUnusedAssets();
        bikeRigid = transform.GetComponent<Rigidbody>();
        bikeRigid.centerOfMass = centerOfMass.localPosition;
        bikeRigid.inertiaTensorRotation = Quaternion.identity;
        bikeRigid.inertiaTensor = inertiaTensorVector * bikeRigid.mass;

        currentSteer = 0;
        totalDirectionRotation = transform.eulerAngles.y;
        onStreetTimer = Time.realtimeSinceStartup - 2f;
        onStreet = false;
        
    }

    private void FixedUpdate()
    {
        
        //logg if the e-scooter is on the street
        if (onStreetTimer + 0.2f > Time.realtimeSinceStartup)
        {
            onStreet = true;
        } else {
            onStreet = false;
        }
    }

    void Update()
    {

        
        //calculating current acceleration
        if (maxSpeed == 0) maxSpeed = 1;
        speedRatio = 1 - Mathf.InverseLerp(0, maxSpeed * 1.2f, currentSpeed);
        motor = maxMotorTorque * (Mathf.Max(arduinoAccelerate-200,0)/695-Mathf.Max(arduinoDeceleration-200,0)/380) * speedRatio;
        
        //calculating new speed
        currentSpeed *= 1 - 0.15f * Time.deltaTime;

        //steering via controller
        Vector3 universalForward = steeringController.forward;
        if (steeringController.forward.Equals(new Vector3(0,0,-1))) universalForward = steeringBackup.forward;
        currentSteer = Vector3.Angle(new Vector3(transform.forward.x, 0, transform.forward.z), new Vector3(-universalForward.x, 0, -universalForward.z));
        if (Vector3.Cross(new Vector3(transform.forward.x, 0, transform.forward.z), new Vector3(-universalForward.x, 0, -universalForward.z)).y < 0) currentSteer *= -1;
        
        steeringMovable.localEulerAngles = new Vector3(0, currentSteer, 0);
        //rotating the transform according to steering
        totalDirectionRotation = totalDirectionRotation + currentSteer * Time.deltaTime * currentSpeed * 0.2f;
        
        transform.Rotate(0, currentSteer * Time.deltaTime * currentSpeed * 0.2f, 0);
        if ((currentSpeed < 0 && motor > 0) ||( currentSpeed > 0 && motor < 0)) motor *= 1.8f;
        if (currentSpeed < 0 && motor < 0) motor *= 0.05f;
        currentSpeed += motor * Time.deltaTime;
        bikeRigid.transform.position = transform.position + transform.forward * currentSpeed * Time.deltaTime;
        if (tiltingScooter) {
            float leanAngle = 0;
            try {
                leanAngle = Mathf.Atan(Mathf.Pow(currentSpeed*1.2f,2) / (9.81f * (0.8f / Mathf.Tan(currentSteer * 0.01745f))));
            } catch (ArgumentNullException exception){
                leanAngle = 0;
            }
            transformTilt = Mathf.Lerp(transformTilt, leanAngle, Time.deltaTime * 5f);
            gameObject.transform.localEulerAngles = new Vector3(0, gameObject.transform.localEulerAngles.y, -transformTilt);
        }
        
        if (showSpeed)
        {
            textGO.SetActive(true);
            textGO.GetComponent<TextMeshProUGUI>().text = (int)(currentSpeed*1.2f) + "km/h";
            float redValue = Mathf.Clamp((currentSpeed*1.2f - 17) * 50 / 255f, 0, 1);
            textGO.GetComponent<TextMeshProUGUI>().color = new Color( redValue, 1-redValue, 1-redValue);
        } else
        {
            textGO.SetActive(false);
        }

    }

    private bool ReturnWheelContact()
    {
        //detect if collider is touching the street for logging

        return false;
    }

    public bool ReturnOnStreet()
    {
        return onStreet;
    }

    public void OnStreetDetected()
    {
        onStreet = true;
        onStreetTimer = Time.realtimeSinceStartup;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("LogStreetTrigger") || other.CompareTag("LogStreet"))
        {
            onStreet = true;
            onStreetTimer = Time.realtimeSinceStartup;
            //Debug.Log("On street");
        }
    }

    //set e-scooter close to the ground
    void AdjustPositionWithRaycast()
    {
        RaycastHit hit;
        Vector3 raycastOrigin = transform.position + transform.forward * 0.5f + Vector3.up * 0.5f; // Adjust the origin to be in front of the collider
        if (Physics.Raycast(raycastOrigin, Vector3.down, out hit, raycastDistance))
        {
            if (hit.point.y > transform.position.y + 0.4f) return;
            // Don't adjust the position if the hit point is above the collider
            Vector3 targetPosition = hit.point + Vector3.up * 0.5f;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
        } 
    }

}