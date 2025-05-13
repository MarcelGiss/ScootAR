using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VIVE.OpenXR;
using VIVE.OpenXR.EyeTracker;

public class UpdateLeftGaze : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        XR_HTC_eye_tracker.Interop.GetEyeGazeData(out XrSingleEyeGazeDataHTC[] out_gazes);
        XrSingleEyeGazeDataHTC leftGaze = out_gazes[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];
        if (leftGaze.isValid)
        {
            transform.position = leftGaze.gazePose.position.ToUnityVector();
            transform.rotation = leftGaze.gazePose.orientation.ToUnityQuaternion();
        }
        
    }
}
