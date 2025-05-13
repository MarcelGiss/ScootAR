using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script deactivates distance pedestrians to save ressources
public class PedestrianLoadOnClose : MonoBehaviour
{
    //change to adjust distance at which pedestrians will be activated
    public int renderDistance = 50;

    public Transform targetCamera;
    public Transform secondaryTargetCamera;
    public Transform testingCamera;
    GameObject body;
    List<GameObject> bodyList = new List<GameObject>();
    private int frameCount = 0;
    bool testRun;
    // Start is called before the first frame update
    void Start()
    {
        if (targetCamera.gameObject.activeSelf) testRun = false;
        else testRun = true;
        if (!targetCamera.gameObject.activeInHierarchy) targetCamera = secondaryTargetCamera;
    }

    // Update is called once per frame
    void Update()
    {
        frameCount++;
        if (frameCount <= 20) return;
        frameCount = 0;
        if (targetCamera.gameObject.activeSelf) testRun = false;
        else testRun = true;
        Vector3 cameraPosition;
        if (!testRun)
        {
            cameraPosition = targetCamera.position;
        }
        else
        {
            if (!testingCamera.gameObject.activeSelf) return;
            cameraPosition = testingCamera.position;
        }
        int deactivatedGameobjects = 0;
        foreach (GameObject gO in bodyList)
        {
            if ((cameraPosition - gO.transform.position).magnitude > renderDistance)
            {
                if (gO.activeSelf)
                {
                    gO.SetActive(false);
                }
                deactivatedGameobjects++;
            }
            else if (!gO.activeSelf) gO.SetActive(true);
        }
        //Debug.Log("DeactivatedPedestrians: " + deactivatedGameobjects + "/" + bodyList.Count);
    }

    public void AddPedestrian(GameObject pedestrian)
    {
        bodyList.Add(pedestrian);

    }
}
