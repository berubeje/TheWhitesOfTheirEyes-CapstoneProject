using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    public enum controller
    {
        PS4,
        XBOX
    }

    public controller currentController;

    [SerializeField]
    private string[] controllers;

    private void Awake()
    {
        controllers = Input.GetJoystickNames();
        //Debug.Log(controllers[0]);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
