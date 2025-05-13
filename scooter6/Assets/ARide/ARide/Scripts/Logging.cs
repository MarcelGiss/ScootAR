using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.InputSystem;
//using VIVE.OpenXR.EyeTracker;
//using VIVE.OpenXR;
using UnityEngine.XR;
using InputDevice = UnityEngine.InputSystem.InputDevice;
using Eyes = UnityEngine.InputSystem.XR.Eyes;

//Logging script used for a study; more information on https://github.com/MarcelGiss/ScootAR
public class Logging : MonoBehaviour
{
    private float timeforSnapshots = 1f;
    private float lastSnapshot = 0f;

    private List<float> snapshotTime = new List<float>();
    private List<int> numberOfClosePedestrians = new List<int>();
    private List<float> smallestDistanceOfPedestrians = new List<float>();
    private List<int> numberOfCloseCars = new List<int>();
    private List<float> smallestDistanceOfCars = new List<float>();
    private List<int> currentSegmentNr = new List<int>();
    private List<bool> onSideWalk = new List<bool>();
    private List<float> currentSpeed = new List<float>();
    private List<float> currentSteer = new List<float>();
    private List<Vector3> scooterDirection = new List<Vector3>();
    private List<Vector3> scooterPosition = new List<Vector3>();
    private List<string> currentColliders = new List<string>();
    private List<string> currentTriggers = new List<string>();
    private List<int> pedestrianIn10M = new List<int>();

    private List<GameObject> cars = new List<GameObject>();

    public int personID;
    public int trafficID;
    public int visualID;
    public int directionID;
    public AIPedestrianSyncSystem syncSys;
    public GameObject eScooter;
    public InputActionReference eyeGazeAction;
    public InputActionAsset eyeGazeAsset;
    public GameObject vrCamera;
    public List<XRNodeState> nodeStates;
    public EScooterNew scooterControlls;

    public Navigator navigator;

    public GameObject trafficHigh;
    public DangerSignaling warning;
    public GameObject line;

    private XRNodeState single_xrnode;
    private Vector3 left_eye_pos, right_eye_pos;
    private Quaternion left_eye_rot, right_eye_rot;
    private bool idsLogged = false;

    private void Awake()
    {
        eyeGazeAsset.Enable();
        trafficHigh.SetActive(trafficID != 0);
    }

    private void OnEnable()
    {
        eyeGazeAsset.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light light in lights)
        {
            Debug.Log("Found light: " + light.name);
        }

        eyeGazeAsset.Enable();

        syncSys = FindFirstObjectByType<ExperimentDefinition>().AIPedestrians;
        if (eScooter == null)
        {
            eScooter = GameObject.Find("ElectricScooter");

        }

        nodeStates = new List<XRNodeState>();

        // Add left eye node to the nodeStates list, which will be represented by nodeStates[0]
        single_xrnode.nodeType = XRNode.LeftEye;
        nodeStates.Add(single_xrnode);

        // Add right eye node to the nodeStates list, which will be represented by nodeStates[1]
        single_xrnode.nodeType = XRNode.RightEye;
        nodeStates.Add(single_xrnode);



        // Set warning and line active state based on visualID
        if (visualID == 0)
        {
            warning.activelyWarning = false;
            line.SetActive(false);
            scooterControlls.showSpeed = false;
        }
        else if (visualID == 1)
        {
            warning.activelyWarning = false;
            line.SetActive(true);
            scooterControlls.showSpeed = true;
        }
        else if (visualID == 2)
        {
            warning.activelyWarning = true;
            line.SetActive(true);
            scooterControlls.showSpeed = true;
        }

        LogIDs();
    }

    void LogIDs()
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"testLog_{personID}_{trafficID}_{visualID}_{directionID}_{Time.realtimeSinceStartup}.csv");
        StringBuilder csvContent = new StringBuilder();

        // Add header for IDs
        csvContent.AppendLine("PlayerID;TrafficID;VisualisationID;DirectionID");

        // Add IDs
        csvContent.AppendLine($"{personID};{trafficID};{visualID};{directionID}");

        // Write the CSV content to the file
        File.WriteAllText(filePath, csvContent.ToString());

        Debug.Log($"IDs logged in CSV file at {filePath}");
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        if (vrCamera != null)
        {
            Ray ray = new Ray(vrCamera.transform.position, right_eye_rot.eulerAngles);
            if (Physics.Raycast(ray, out RaycastHit hit, 500f))
            {
                Debug.Log("Gaze hit: " + hit.collider.name);
            }
        }
        lastSnapshot = Time.realtimeSinceStartup;

        //saving time of snapshot
        snapshotTime.Add(Time.realtimeSinceStartup);

        // Log currentSpeed and currentSteer from EScooterNew
        currentSpeed.Add(scooterControlls.currentSpeed * 1.2f);
        currentSteer.Add(scooterControlls.currentSteer);

        // Log direction and position of EScooter
        scooterDirection.Add(eScooter.transform.forward);
        scooterPosition.Add(eScooter.transform.position);

        // Log current colliders and triggers
        Collider[] colliders = Physics.OverlapSphere(eScooter.transform.position, 0.3f);
        List<string> collidersList = new List<string>();
        List<string> triggersList = new List<string>();
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.name.Contains("Car Spawner") || collider.gameObject.name.Contains("Pedestrian"))
            {
                if (collider.isTrigger)
                {
                    triggersList.Add(collider.name);
                }
                else
                {
                    collidersList.Add(collider.name);
                }
            }
        }
        currentColliders.Add(string.Join(",", collidersList));
        currentTriggers.Add(string.Join(",", triggersList));

        int countedClosePedestrians = 0;
        float smallestPedestriandistance = 1000f;
        int pedestriansWithin10M = 0;
        foreach (AIPedestrianSyncSystem.PedestrianDesc pedestrianDesc in syncSys.AIPedestrians)
        {
            if (pedestrianDesc.Pedestrian == null) continue;
            GameObject pedestrian = pedestrianDesc.Pedestrian.gameObject;
            float distanceToUser = (pedestrian.transform.position - eScooter.transform.position).magnitude;
            if (distanceToUser < smallestPedestriandistance) smallestPedestriandistance = distanceToUser;
            if (distanceToUser < 10f) pedestriansWithin10M++;
            if (distanceToUser < 3.7f) countedClosePedestrians++;
        }
        numberOfClosePedestrians.Add(countedClosePedestrians);
        smallestDistanceOfPedestrians.Add(smallestPedestriandistance);
        pedestrianIn10M.Add(pedestriansWithin10M);

        int currentNumberOfCloseCars = 0;
        float smallestCarDistance = 1000;
        foreach (GameObject car in cars)
        {
            if (smallestCarDistance > (car.transform.position - eScooter.transform.parent.position).magnitude) smallestCarDistance = (car.transform.position - eScooter.transform.parent.position).magnitude;
            if ((car.transform.position - eScooter.transform.parent.position).magnitude < 10f)
            {
                currentNumberOfCloseCars++;
            }
        }
        numberOfCloseCars.Add(currentNumberOfCloseCars);
        smallestDistanceOfCars.Add(smallestCarDistance);

        currentSegmentNr.Add(navigator.GetSegment());

        if (eScooter.transform.parent) onSideWalk.Add(!eScooter.transform.parent.GetComponent<EScooterNew>().ReturnOnStreet());

    }

    // Write the data at the end of each run
    private void OnDestroy()
    {
        Debug.Log(cars.Count);
        // Define the path and name of the CSV file
        string filePath = Path.Combine(Application.persistentDataPath, $"testLog_{personID}_{trafficID}_{visualID}_{directionID}_{Time.realtimeSinceStartup}.csv");

        // Use StringBuilder for efficient string concatenation
        StringBuilder csvContent = new StringBuilder();

        // Add header
        csvContent.AppendLine("PlayerID;TrafficID;VisualisationID;DirectionID;SnapshotTime;CurrentSpeed;CurrentSteer;ScooterDirection;ScooterPosition;PedestrianIn3.7M;PedestrianIn10M;SmallestDistanceOfPedestrians;CarsIn10M;SmallestDistanceOfCars;CurrentSegmentNr;OnSideWalk;CurrentColliders;CurrentTriggers");

        // Iterate through each snapshot entry and write data to the CSV
        for (int i = 0; i < snapshotTime.Count; i++)
        {
            // Get the basic data
            string basicData = $"{personID};{trafficID};{visualID};{directionID};{snapshotTime[i]};{currentSpeed[i]};{currentSteer[i]};{scooterDirection[i]};{scooterPosition[i]};{numberOfClosePedestrians[i]};{pedestrianIn10M[i]};{smallestDistanceOfPedestrians[i]};{numberOfCloseCars[i]};{smallestDistanceOfCars[i]};{currentSegmentNr[i]};{onSideWalk[i]};{currentColliders[i]};{currentTriggers[i]}";

            // Combine everything into one CSV line
            csvContent.AppendLine(basicData);
        }

        // Write the CSV content to the file
        File.WriteAllText(filePath, csvContent.ToString());

        Debug.Log($"CSV file created at {filePath}");
    }

    public void AddCar(GameObject carToAdd)
    {
        cars.Add(carToAdd);
    }

}