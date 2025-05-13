using UnityEngine;
using UnityEngine.Rendering;

public class WheelLoggingHelper : MonoBehaviour
{
    private EScooterNew[] eScooterScripts;
    private WheelCollider wheelCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eScooterScripts = FindObjectsByType<EScooterNew>(FindObjectsSortMode.None);
        wheelCollider = GetComponent<WheelCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (wheelCollider != null)
        {
            WheelHit hit;
            if (wheelCollider.GetGroundHit(out hit))
            {
                if (hit.collider.CompareTag("LogStreet"))
                {
                    foreach (var eScooterScript in eScooterScripts)
                    {
                        if (eScooterScript.gameObject.activeSelf)
                        {
                            eScooterScript.OnStreetDetected();
                        }
                    }
                }
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("LogStreet"))
        {
            foreach (var eScooterScript in eScooterScripts)
            {
                if (eScooterScript.gameObject.activeSelf)
                {
                    eScooterScript.OnStreetDetected();
                }
            }
        }
    }
}
