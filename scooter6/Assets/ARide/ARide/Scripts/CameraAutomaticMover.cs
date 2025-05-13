using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAutomaticMover : MonoBehaviour
{
    [SerializeField, Tooltip("ticking this box will reset the position to the start position and automatically reset itself once it's finished.")]
    private bool resetPosition = false;
    [SerializeField, Tooltip("ticking this box will start the camera movement forward.")]
    private bool beginMovement = false;
    [SerializeField, Tooltip("Amount of seconds the camera will move forward when activated.")]
    private float maxMovementTime = 2f;
    [SerializeField, Tooltip("Distance in meters the camera will move forward when activated.")]
    private float maxDistanceCovered = 4f;
    private Vector3 startPosition;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            transform.position = startPosition + transform.forward * maxDistanceCovered * (maxMovementTime - timer);
        }
        if (resetPosition)
        {
            resetPosition = false;
            transform.position = startPosition;
            timer = 0;
        }
        if (beginMovement)
        {
            beginMovement = false;
            timer = maxMovementTime;
        }

    }
}
