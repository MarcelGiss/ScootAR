using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script will update the phone navigation arrow and distances
public class CheckpointScript : MonoBehaviour
{
    public bool active;
    public string checkPointName;
    public int degreeToTurn;
    public float duration = 2.0f;
    public Vector3 goal;
    public bool start = false;



    // Start is called before the first frame update
    void Start()
    {
        goal = transform.GetChild(0).position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Once a checkpoint is touched the data will be forwarded to the navComponent on the phone
    private void OnTriggerEnter(Collider other)
    {

        Navigator navComponent;
        if (other.transform.TryGetComponent<Navigator>(out navComponent))
        {
            //A checkpoint with the end tag will end the game
            if ((gameObject.name.Contains("End")))
            {
                UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();
            }
            navComponent.DisplayNavigation(degreeToTurn, transform.forward, goal);
            //Destroy to prevent bugs when turning around
            Destroy(this);
        }
    }
}
