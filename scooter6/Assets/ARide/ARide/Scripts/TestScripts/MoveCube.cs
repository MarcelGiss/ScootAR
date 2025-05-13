using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveCube : MonoBehaviour
{
    [SerializeField] private InputActionReference JoyStitckR;

    [SerializeField] private InputActionAsset ActionAsset;

    // Start is called before the first frame update
    void Start()
    {
        if (ActionAsset != null)
        {
            ActionAsset.Enable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (JoyStitckR.action.ReadValue<bool>()) transform.position += Vector3.up * Time.deltaTime;
    }
}
