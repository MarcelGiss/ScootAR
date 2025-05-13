using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerVisualiser : MonoBehaviour
{
    public bool warningSignActive = true;
    public bool iconActive = false;
    public bool floorWarning = false;
    public bool barrierActive = false;
    public GameObject prefab;

    public bool adjustHeight = true;
    public float targetHeight = 100.3f;

    private bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x, 100.3f, transform.position.z);
        GameObject newPrefab = Instantiate(prefab, transform);
        newPrefab.transform.position = transform.position + new Vector3(0, 0, 0);
        GameObject.Find("Managers").GetComponent<PedestrianWarningManager>().AddPedestrianWarning(this);
        GameObject.Find("PedestrianRenderManager").GetComponent<PedestrianLoadOnClose>().AddPedestrian(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (adjustHeight == false)
        {
            return;
        }
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 100.26f, 100.33f), transform.position.z);
    }

    //Send position for accurate display of danger Symbols - Signals are shown below maxDis - Signal strength increases with a decrease in distance to enemy - Singal strength max minDis
    public void VisualiseDanger(Vector3 enemyPosition, float minDis, float maxDis)
    {
        active = false;
        float strength = 0f;
        if (minDis > maxDis)
        {
            Debug.Log("minDistance should never be bigger than maxDistance!");
            minDis = maxDis;
        }

        float distance = (enemyPosition - transform.position).magnitude;

        if (distance <= maxDis)
        {
            active = true;
            strength = 1 - Mathf.Max(0, distance - minDis) / (maxDis - minDis);
        }
        else active = false;
        GameObject warnings = transform.GetChild(3).gameObject;
        if (!warnings.name.Contains("Warnings"))
        {
            warnings = transform.GetChild(4).gameObject;
        }
        //selecting visualization called sign as relevant warning-sign
        if (warningSignActive && active)
        {
            GameObject sign = warnings.transform.GetChild(0).gameObject;
            //activate sign
            sign.SetActive(true);
            sign.transform.position = transform.position + (enemyPosition - transform.position).normalized * 0.8f + Vector3.up;
            //rotate the sign towards pedestrian
            sign.transform.LookAt(enemyPosition);
            //change the size of sign
            sign.transform.localScale = Vector3.one * strength;
        }
        else warnings.transform.GetChild(0).gameObject.SetActive(false);
        //selecting icon as relevant sign
        if (iconActive && active)
        {
            //similar to Sign
            GameObject sign = warnings.transform.GetChild(1).gameObject;
            sign.SetActive(true);
            sign.transform.LookAt(enemyPosition + Vector3.up);
            Color iconBackgroundColor = new Color(strength, 1 - strength, 0, strength);
            //changecolor of icon background
            sign.transform.GetChild(0).GetChild(2).gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", iconBackgroundColor);
        }
        else warnings.transform.GetChild(1).gameObject.SetActive(false);
        //selecting floor triangle as relevant sign
        if (floorWarning && active)
        {
            //similar to Sign
            GameObject sign = warnings.transform.GetChild(2).gameObject;
            sign.SetActive(true);
            sign.transform.LookAt(enemyPosition);
            sign.transform.localPosition = Vector3.up * 0.3f;
            //changecolor of triangle
            sign.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = new Color(strength, 1 - strength, 0, strength);
        }
        else warnings.transform.GetChild(2).gameObject.SetActive(false);
        //select barrier as the relevant sign -- barrier shader will automatically adjust the size according to view distance
        if (barrierActive && active)
        {
            warnings.transform.GetChild(3).gameObject.SetActive(true);
        }
        else warnings.transform.GetChild(3).gameObject.SetActive(false);
    }

    public bool GetActive()
    {
        return active;
    }
}
