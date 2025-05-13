using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


//control panel for all warnings in the scene
public class PedestrianWarningManager : MonoBehaviour
{
    List<DangerVisualiser> dangerVisualiserList = new List<DangerVisualiser>();
    public bool barrierOn = true;
    public bool iconOn = true;
    public bool floorWarningOn = true;
    public bool signOn = true;

    private float updateInterval = 3.0f; // Intervall in Sekunden
    private float nextUpdateTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;

            foreach (DangerVisualiser visualiser in dangerVisualiserList)
            {
                visualiser.barrierActive = barrierOn;
                visualiser.iconActive = iconOn;
                visualiser.floorWarning = floorWarningOn;
                visualiser.warningSignActive = signOn;
            }
        }


    }

    public void AddPedestrianWarning(DangerVisualiser instance)
    {
        dangerVisualiserList.Add(instance);
    }

}
