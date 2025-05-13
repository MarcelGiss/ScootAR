using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This scipt uses the position of the controler to automatically reset the position of the VR camera.

public class VRAutomaticAdjustment : MonoBehaviour
{
    public GameObject realController;
    public GameObject backupController;
    public GameObject VROffset;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AdjustVROffsetAfterDelay(3.0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private IEnumerator AdjustVROffsetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!VROffset.activeSelf) yield break;
        if (realController.transform.localRotation == Quaternion.Euler(Vector3.zero)){
            Vector3 distance = transform.position - realController.transform.position;
            VROffset.transform.position += distance;
        } else if (backupController.transform.localRotation == Quaternion.Euler(Vector3.zero)){
            Vector3 distance = transform.position - backupController.transform.position;
            VROffset.transform.position += distance;
        }
        
    }

}
