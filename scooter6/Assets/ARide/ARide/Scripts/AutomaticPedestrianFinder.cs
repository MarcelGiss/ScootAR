using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Utility;


//This script will find all pedestrians with the Tags "LostCitizen" or "LostCitizen2" and assigns them a random route from a slection of pedestrian paths
public class AutomaticPedestrianFinder : MonoBehaviour
{
    [SerializeField, Tooltip("Place pedestrian path objects as children of this transform to be used for pedestrians.")]
    private Transform pedestrianPathsParent;

    //the scene is measured to detect when a new scene is loaded via the coupledSim LevelManager
    public Scene startScene;

    private bool sceneChanged;

    [SerializeField, Tooltip("If true, all pedestrians with tag 'LostPedestrian' are added. If false, 'LostPedestrian2' is used.")]
    private bool mainPedestrianFinder = true;

    // Start is called before the first frame update
    void Awake()
    {

        if (pedestrianPathsParent == null)
        {
            Debug.LogError("PedestrianPathsParent is not assigned in the inspector!", this);
        }

        // remove to make sure there are not two Listeners
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        startScene = SceneManager.GetActiveScene();
    }


    private void OnLevelWasLoaded()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        //add all pedestrians with the corresponding tag to citizenGameObjects
        GameObject[] citizenGameObjects;
        if (mainPedestrianFinder)
        {
            citizenGameObjects = GameObject.FindGameObjectsWithTag("LostCitizens");
        }
        else
        {
            citizenGameObjects = GameObject.FindGameObjectsWithTag("LostCitizens2");
        }
        List<AIPedestrianSyncSystem.PedestrianDesc> citizenList = new List<AIPedestrianSyncSystem.PedestrianDesc>();
        Debug.Log("Starting with collecting PedestrianDesc. Total number of Citizens to add: " + citizenGameObjects.Length);

        //add a route to every pedestrian
        foreach (GameObject citizenGO in citizenGameObjects)
        {
            AIPedestrianSyncSystem.PedestrianDesc citizen = new AIPedestrianSyncSystem.PedestrianDesc();
            citizen.Pedestrian = citizenGO.GetComponent<AIPedestrian>();
            if (citizenGO.GetComponent<WaypointProgressTracker>().Circuit != null)
            {
                citizen.Path = citizenGO.GetComponent<WaypointProgressTracker>().Circuit;
            }
            else
            {
                int numberofPaths = pedestrianPathsParent.childCount;
                citizen.Path = pedestrianPathsParent.GetChild(Random.Range(0, numberofPaths - 1)).GetComponent<WaypointCircuit>();
            }
            citizenList.Add(citizen);
        }
        AIPedestrianSyncSystem.PedestrianDesc[] citizensToDeliver = citizenList.ToArray();
        GameObject.FindAnyObjectByType<ExperimentDefinition>().AIPedestrians.AddCitizens(citizensToDeliver);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene == startScene)
        {
            OnLevelWasLoaded();
        }
    }
}
