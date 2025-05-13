using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DangerSignaling : MonoBehaviour
{
    //List with all Pedestrians with respective warnings
    List<GameObject> dangerList;
    List<GameObject> closeDangerList;

    public float maxDetectionDistance = 3.7f;
    public float maxVisualisationDistance = 1.2f;
    public bool activelyWarning = true;
    private int frameCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        dangerList = new List<GameObject>();
        closeDangerList = new List<GameObject>();
    }


    void Update()
    {
        //perserving ressources
        frameCounter++;
        if (frameCounter >= 60)
        {
            //Collect all AR-warning Objects
            GameObject[] dangerArray = GameObject.FindGameObjectsWithTag("ARWarnings");
            dangerList = dangerArray.ToList();
            frameCounter = 0;

        }
        Vector3 thisPosition = transform.position;
        //pass in a seperate list for perserving resources; adjust frame counter (e.g. divisable by 5 to call more often)
        if (frameCounter == 0)
        {
            closeDangerList.Clear();
            foreach (GameObject gO in dangerList)
            {
                float pedestrianDistance = (gO.transform.position - thisPosition).magnitude;
                if (pedestrianDistance < maxDetectionDistance * 3)
                {
                    closeDangerList.Add(gO);
                }

            }
        }
        //Activate every relevant warning
        foreach (GameObject gO in dangerList)
        {

            try
            {
                float pedestrianDistance = (gO.transform.position - thisPosition).magnitude;
                if (pedestrianDistance < maxDetectionDistance * 2f)
                {
                    DangerVisualiser visualiser = gO.transform.parent.gameObject.GetComponent<DangerVisualiser>();
                    visualiser.VisualiseDanger(transform.position - Vector3.down * 0.5f, maxVisualisationDistance, maxDetectionDistance);
                }
            }
            catch
            {
                Debug.Log(gO.name + " is at: " + gO.transform.position);
            }


        }
    }
}
