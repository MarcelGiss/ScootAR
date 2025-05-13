using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//randomizes animation to give every standing pedestrian unique animation timings
public class RandomAnimator : MonoBehaviour
{
    public Animator animator;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        float newSpeed = Random.Range(0.1f, 1.2f);
        animator.speed = newSpeed;
        timer = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer + 5f < Time.realtimeSinceStartup)
        {
            float newSpeed = Random.Range(0.8f, 1.1f);
            animator.speed = newSpeed;
            Destroy(this);
        }
    }
}
