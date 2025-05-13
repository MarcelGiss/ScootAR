using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//drive a track and place prefabs -- manually safe before ending a scene
public class WPCreator : MonoBehaviour
{
    public GameObject prefabToCreate;
    public Transform newParent;

    private float timeFromLastCreation = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timeFromLastCreation + 0.25f < Time.deltaTime)
        {
            Instantiate(prefabToCreate, transform);
            timeFromLastCreation = Time.time;
        }
    }
}
