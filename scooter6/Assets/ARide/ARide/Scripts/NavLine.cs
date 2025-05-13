 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavLine : MonoBehaviour
{

    public int startTime = 1;
    public int carSpeed = 35;

    private float sceneStartTimer = -1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(sceneStartTimer == -1f)
        {
            sceneStartTimer = Time.time;
        }

        if(startTime - sceneStartTimer > Time.time)
        {
            gameObject.GetComponent<AICar>().speed = carSpeed;
            Destroy(this);
        }
    }
}
