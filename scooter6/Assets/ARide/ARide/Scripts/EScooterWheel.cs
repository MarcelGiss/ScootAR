using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using TMPro;
using UnityEngine;
using VIVE.OpenXR.Samples;

//alternative version of steering the e-scooter with WheelColliders
public class EScooterWheel: MonoBehaviour
{
    public bool testControlls = false;
    public bool vRControlls = false;
    public Transform steeringController;
    public Transform steeringBackup;
    public bool showSpeed;
    public List<BikeAxleInfo> axleInfos;
    public float maxSpeed = 50f;
    public float maxMotorTorque = 200f;
    public float maxSteeringAngle;
    public Transform centerOfMass;
    public Vector3 inertiaTensorVector;
    public GameObject textGO;
    private float speedRatio;
    private float motor;
    private float steeringInput;
    private float currentSteer;
    private Rigidbody bikeRigid;
    private float speed;
    private float totalDirectionRotation;
    //public SteamVR_Action_Boolean triggerAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");
    //public SteamVR_Input_Sources handType = SteamVR_Input_Sources.Any;
    public float arduinoAccelerate = 0;
    public float arduinoDeceleration = 0;
    private bool onStreet;
    private float onStreetTimer;

    //public SteamVR_Action triggerAnalog;

    [System.Serializable]
    public class BikeAxleInfo
    {
        public string name;
        public WheelCollider wheelCol;
        public Transform wheel;
        public bool motor;
        public bool steering;
    }

    void Start()
    {
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
        foreach (BikeAxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.wheelCol.steerAngle = currentSteer; /// Mathf.Max(0.5f,speed);
                axleInfo.wheel.parent.localEulerAngles = new Vector3(0, currentSteer, 0);
            }
            if (axleInfo.motor)
            {
                axleInfo.wheelCol.motorTorque = motor * Time.deltaTime * (1 - speed / maxSpeed);
            }
            //axleInfo.wheel.Rotate(new Vector3(speed * Time.deltaTime, 0, 0));
        }

        if (onStreetTimer + 0.2f < Time.realtimeSinceStartup)
        {
            if (ReturnWheelContact())
            {
                onStreet = true;
                onStreetTimer = Time.realtimeSinceStartup;
            } else
            {
                onStreet = false;
            }
        }
    }

    void Update()
    {
        //Debug.Log("TargetFramerate: " + Application.targetFrameRate);
        if (maxSpeed == 0) maxSpeed = 1;
        speed = bikeRigid.linearVelocity.magnitude;
        speedRatio = 1 - Mathf.InverseLerp(0, maxSpeed, speed);
        if (vRControlls) 
        {
            motor = maxMotorTorque * (Mathf.Max(arduinoAccelerate-200,0)/695-Mathf.Max(arduinoDeceleration-200,0)/380) * speedRatio;
        } else
        {
            motor = maxMotorTorque * Input.GetAxis("Vertical") * speedRatio;
        }
        if (vRControlls)
        {
            Vector3 universalForward = steeringController.forward;
            if (steeringController.forward.Equals(new Vector3(0,0,-1))) universalForward = steeringBackup.forward;
            currentSteer = Vector3.Angle(new Vector3(transform.forward.x, 0, transform.forward.z), new Vector3(-universalForward.x, 0, -universalForward.z));
            if (Vector3.Cross(new Vector3(transform.forward.x, 0, transform.forward.z), new Vector3(-universalForward.x, 0, -universalForward.z)).y < 0) currentSteer *= -1;
            
        }
        else
        {
            steeringInput = maxSteeringAngle * Input.GetAxis("Horizontal") * Mathf.Max(speedRatio, 0.1f);
            if (Time.deltaTime * 90f > Mathf.Abs(currentSteer - steeringInput)) currentSteer = steeringInput;
            else if (currentSteer >= steeringInput) currentSteer -= Time.deltaTime * 60f * (1 - Mathf.Max(speed / maxSpeed, 0.1f));
            else currentSteer += Time.deltaTime * 60f * (1 - Mathf.Max(speed / maxSpeed, 0.1f));
        }
        totalDirectionRotation = totalDirectionRotation + currentSteer * Time.deltaTime * speed * 0.2f;
        bikeRigid.MoveRotation(Quaternion.Euler(transform.eulerAngles.x, totalDirectionRotation, transform.eulerAngles.z));

        // Calculate the lean angle
        /*
        float leanAngle = Mathf.Clamp(-currentSteer * speed * 0.1f, -20f, 20f);
        float currentZRotation = transform.localRotation.eulerAngles.z;
        if (currentZRotation > 180) currentZRotation -= 360; // Convert to range -180 to 180
        float newZRotation = Mathf.Lerp(currentZRotation, leanAngle, Mathf.Min(Time.deltaTime * 5f, 0.2f));
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, newZRotation);
        //both wheels should not be tilted
        /*foreach (BikeAxleInfo axleInfo in axleInfos)
        {
            axleInfo.wheel.localRotation = Quaternion.Euler(axleInfo.wheel.localRotation.eulerAngles.x, axleInfo.wheel.localRotation.eulerAngles.y, -newZRotation);
        }
        */

        if (showSpeed)
        {
            textGO.SetActive(true);
            textGO.GetComponent<TextMeshProUGUI>().text = (int)speed + "km/h";
            float redValue = Mathf.Clamp((speed - 17) * 50 / 255f, 0, 1);
            textGO.GetComponent<TextMeshProUGUI>().color = new Color( redValue, 1-redValue, 1-redValue);
        } else
        {
            textGO.SetActive(false);
        }

    }

    private bool ReturnWheelContact()
    {
        foreach (BikeAxleInfo info in axleInfos)
        {
            WheelHit hit;
            if (info.wheelCol.GetGroundHit(out hit))
            {
                if (hit.collider.tag.Equals("LogStreet"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool ReturnOnStreet()
    {
        return onStreet;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("LogStreetTrigger"))
        {
            onStreet = true;
            onStreetTimer = Time.realtimeSinceStartup;
        }
    }

}


