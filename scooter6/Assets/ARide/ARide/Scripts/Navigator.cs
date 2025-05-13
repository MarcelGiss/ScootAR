using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Navigator : MonoBehaviour
{
    
    public bool phoneNavigation;
    public GameObject arrowGameObject;
    public GameObject phoneGameObject;
    public float minimumTimeShown = 1f;

    public TMP_Text text;

    private float degree;
    private Vector3 direction;
    private Vector3 start;
    private Vector3 goal;
    private float startTime;
    private float smallestDistance;
    private float displayNavigationTime = 10f;

    private bool goalReached = false;
    private int segmentCount = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!phoneNavigation) phoneGameObject.SetActive(false);
        if (gameObject.activeSelf)
        {
            //update navigation phone
            float currentDistance = Vector3.Distance(transform.position, goal);
            
            if (currentDistance < 5){
                if ((currentDistance < 2 || currentDistance > smallestDistance) && Time.deltaTime - startTime < minimumTimeShown){
                    
                    goalReached = true;
                }
            }
            
            currentDistance = (int)Vector3.Distance(transform.position, goal);
            if (currentDistance > 100)
            {
                currentDistance = (int)(currentDistance / 50) * 50;
            }
            else currentDistance = (int)(currentDistance / 5) * 5;
            if (goalReached) currentDistance = 0;
            text.text = currentDistance + "m";
            arrowGameObject.transform.localEulerAngles = new Vector3(0,0,degree-90);
        }
        //fallback navigation if driver is lost
        if (Time.time - displayNavigationTime > 15 && !goalReached)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            Vector3 pathToGoal = goal - transform.position;
            float angle = Vector3.SignedAngle(pathToGoal, transform.forward, Vector3.up);
            arrowGameObject.transform.localEulerAngles = new Vector3(0, 0, angle);
        }
    }

    //call this method to update arrows and distance
    public void DisplayNavigation(float degree, Vector3 direction, Vector3 goal)
    {
        Debug.Log("Displaying new navigation.");
        segmentCount++;
        goalReached = false;
        if (!phoneNavigation)
        {
            arrowGameObject.SetActive(false);
            return;
        }
        this.degree = degree;
        this.direction = direction;
        this.goal = goal;
        start = transform.position;
        startTime = Time.deltaTime;
        displayNavigationTime = Time.time;
        phoneGameObject.SetActive(true);
        //choose corresponding arrow 
        if ((this.degree < 315 && this.degree > 225) ||(this.degree < -45 && this.degree > -135))
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
        }
        else if (this.degree < 135 && this.degree > 45)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            arrowGameObject.transform.rotation = Quaternion.Euler(arrowGameObject.transform.rotation.x, arrowGameObject.transform.rotation.y, this.degree);
        }

    }

    public int GetSegment()
    {
        return segmentCount;
    }
}
