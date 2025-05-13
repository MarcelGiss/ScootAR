using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Cars will wait for a set amount of time until driving
public class CarLauncher : MonoBehaviour
{
    [SerializeField, Tooltip("Amount of seconds after scene start cars will wait until they start driving")]
    private int startTime = 1;
    [SerializeField, Tooltip("The starting speed of cars")]
    private int carSpeed = 35;

    private float sceneStartTime = -1;

    // Start is called before the first frame update
    void Start()
    {
        //automatically add each car to the logging script (logging script was used for a specific study)
        Logging[] loggingScripts = FindObjectsByType<Logging>(FindObjectsSortMode.None);
        foreach (Logging logging in loggingScripts)
        {
            logging.AddCar(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (sceneStartTime == -1)
        {
            sceneStartTime = Time.time;
        }

        if (Time.time - sceneStartTime > startTime)
        {
            gameObject.GetComponent<AICar>().speed = carSpeed;
            Destroy(this);
        }
    }
}
