using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//When placed on a pedestrian, this pedestrian will now wait in front of streets and check if teh street is clear
//A Barrier with the Tag PedestrianStopCross or PedestrianStopCrossIsland is needed in front of the street to trigger this script
public class ChildRerouter : MonoBehaviour
{
    public PedestrianRerouting parentScript;


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Contains("PedestrianStopCross"))
        {
            float dotProduct = Vector3.Dot(other.transform.forward, transform.forward);
            if (other.gameObject.tag.Equals("PedestrianStopCrossIsland"))
            {
                //check for both sides when on a pedestrian island
                if (Vector3.Distance(other.ClosestPoint(transform.parent.position), transform.parent.position) > 0.05f) return;
                if (dotProduct > 0)
                {
                    parentScript.LookForCars(true, other.transform.forward);
                }
                else
                {
                    parentScript.LookForCars(true, other.transform.forward * -1);
                }
            }
            else if (dotProduct > 0.5f)
            {
                //A normal stop cross only requires to look in one direction
                parentScript.LookForCars(true, other.transform.forward);

            }
        }
        else
        {
            parentScript.CarHit(other);
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Contains("PedestrianStopCross")) parentScript.LookForCars(false);
    }
}
