using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct LocalTrafficLightEvent
{
    //miningful description of cycle event
    public string name;
    //time that has to pass since previous event before activating this event
    public float deltaTime;

    //sections on which state change will be applied on
    public CarSection[] carSections;
    public PedestrianSection[] pedestrianSections;

    //state to be set on the carSections and pedestrainSections
    public LightState state; 
}

public enum LocalLightState
{
    GREEN,
    BLINK_GREEN,
    RED,
    YELLOW,
    RED_AND_YELLOW,
    LOOP_BACK
}

//defines and executes traffic light cycle described as a sequence of TrafficLightEvents
public class LocalLightManager : MonoBehaviour
{

    [SerializeField]
    List<int> _initiallyTriggeredEventsBuffer = new List<int>();
    List<int> _triggeredEventsBuffer = new List<int>();
    private TrafficLightEvent[] initialStreetLightSetup;
    [SerializeField]
    private TrafficLightEvent[] streetLightEvents;
    public int CurrentIndex = 0;
    public float CurrentTimer = 0;
    bool initialized = false;

    void Start()
    {
        this.UpdateHost(_initiallyTriggeredEventsBuffer, _triggeredEventsBuffer);
        //this.TriggerEvent(1, true);
    } 

    public void UpdateHost(List<int> initiallyTriggeredEvents, List<int> triggeredEvents)
    {
        if (!initialized)
        {
            for(int i = 0; i < initialStreetLightSetup.Length; i++)
            {
                initiallyTriggeredEvents.Add(CurrentIndex);
                TriggerEvent(i, true);
            }
            initialized = true;
        }
        while (CurrentTimer >= streetLightEvents[CurrentIndex].deltaTime && CurrentIndex < streetLightEvents.Length)
        {
            triggeredEvents.Add(CurrentIndex);
            TriggerEvent(CurrentIndex, false);
            CurrentTimer -= streetLightEvents[CurrentIndex].deltaTime;
            CurrentIndex++;
            if (CurrentIndex >= streetLightEvents.Length)
            {
                CurrentIndex = 0;
            }
        }

        CurrentTimer += Time.deltaTime;
    }

    public void TriggerEvent(int idx, bool initialSetup)
    {
        if (initialSetup)
        {
            ChangeCarLightsState(ref initialStreetLightSetup[idx]);
            ChangePedestrianLightsState(ref initialStreetLightSetup[idx]);
        } else {
            ChangeCarLightsState(ref streetLightEvents[idx]);
            ChangePedestrianLightsState(ref streetLightEvents[idx]);
        }
    }

    private void ChangeCarLightsState(ref TrafficLightEvent streetLightEvent)
    {
        if (streetLightEvent.carSections.Length == 0)
        {
            return;
        }

        foreach (CarSection section in streetLightEvent.carSections)
        {
            foreach (CarTrafficLight light in section.carStreetLights)
            {
                switch (streetLightEvent.state)
                {
                    case LightState.GREEN:
                        light.TurnGreen();
                        break;

                    case LightState.YELLOW:
                        light.TurnYellow();
                        break;

                    case LightState.RED:
                        light.TurnRed();
                        break;

                    case LightState.RED_AND_YELLOW:
                        light.TurnRedAndYellow();
                        break;

                    case LightState.LOOP_BACK:
                        break;
                }
            }
        }
    }

    private void ChangePedestrianLightsState(ref TrafficLightEvent streetLightEvent)
    {
        if (streetLightEvent.pedestrianSections.Length == 0)
        {
            return;
        }
        foreach (PedestrianSection section in streetLightEvent.pedestrianSections)
        {
            foreach (PedestrianTrafficLight light in section.pedestrianLights)
            {
                switch (streetLightEvent.state)
                {
                    case LightState.GREEN:
                        light.TurnGreen();
                        break;
                    case LightState.RED:
                        light.TurnRed();
                        break;

                    case LightState.BLINK_GREEN:
                        light.TurnBlink();
                        break;
                    case LightState.LOOP_BACK:
                        break;
                }
            }
        }
    }
}
