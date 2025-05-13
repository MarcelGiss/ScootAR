using UnityEngine;
using System.Collections;

//can be used for cars to wait for each other at an intersection
public class TrafficMerger : MonoBehaviour
{
    public SpeedSettings mainTraffic;
    public SpeedSettings sideObject1;
    public SpeedSettings sideObject2;
    public SpeedSettings sideObject3;
    private SpeedSettings[] sideObjects;
    private int currentIndex = 0;
    private bool isMainObjectActive = true;

    void Start()
    {
        sideObjects = new SpeedSettings[] { sideObject1, sideObject2, sideObject3 };
        StartCoroutine(SwitchTraffic());
    }

    IEnumerator SwitchTraffic()
    {
        while(true) {
            if (!isMainObjectActive){
                SpeedSettingsOn(mainTraffic);
                isMainObjectActive = true;

                SpeedSettingsOn(sideObjects[currentIndex]);
                currentIndex = (currentIndex + 1) % 3;
                SpeedSettingsOff(sideObjects[currentIndex]);
            } else {
                SpeedSettingsOff(mainTraffic);
                isMainObjectActive = false;
            }
            yield return new WaitForSeconds(5f);
        }
    }

    void SpeedSettingsOn(SpeedSettings traffic){
        traffic.speed = 30;
        traffic.acceleration = 2f;
    }

    void SpeedSettingsOff(SpeedSettings traffic){
        traffic.speed = 0;
        traffic.acceleration = -5;
    }
}
