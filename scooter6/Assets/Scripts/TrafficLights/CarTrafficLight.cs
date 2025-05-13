using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//controls single car trafficlight 
//set renderers to match state set by trafficlight cycle
public class CarTrafficLight : MonoBehaviour
{
    public LightState State;

    [SerializeField]
    protected Material redMaterial;
    [SerializeField]
    protected Material greenMaterial;
    [SerializeField]
    protected Material yellowMaterial;
    [SerializeField]
    protected Material turnOffMaterial;
    [SerializeField]
    protected MeshRenderer downLightRenderer;
    [SerializeField]
    protected MeshRenderer middleLightRenderer;
    [SerializeField]
    protected MeshRenderer upRenderer;
    [SerializeField]
    protected GameObject stopper;

    public void TurnGreen()
    {
        State = LightState.GREEN;
        downLightRenderer.material = greenMaterial;
        middleLightRenderer.material = turnOffMaterial;
        upRenderer.material = turnOffMaterial;
        if (stopper != null )
        {

            stopper.SetActive(false);
        }
    }

    public void TurnRed()
    {
        State = LightState.RED;
        downLightRenderer.material = turnOffMaterial;
        middleLightRenderer.material = turnOffMaterial;
        upRenderer.material = redMaterial;
        if (stopper != null)
        {

            stopper.SetActive(true);

            stopper.GetComponent<SpeedSettings>().speed = 0;
            stopper.GetComponent<SpeedSettings>().acceleration = -6;
        }
    }

    public void TurnYellow()
    {
        State = LightState.YELLOW;
        downLightRenderer.material = turnOffMaterial;
        middleLightRenderer.material = yellowMaterial;
        upRenderer.material = turnOffMaterial;
        if (stopper != null)
        {

            stopper.SetActive(false);
        }
    }

    public void TurnRedAndYellow()
    {
        State = LightState.RED_AND_YELLOW;
        downLightRenderer.material = turnOffMaterial;
        middleLightRenderer.material = yellowMaterial;
        upRenderer.material = redMaterial;
        if (stopper != null)
        {

            stopper.SetActive(true);
            stopper.GetComponent<SpeedSettings>().speed = 20;
            stopper.GetComponent<SpeedSettings>().acceleration = 3;
        }
    }

}

