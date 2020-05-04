using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JimController : PlayerControllerBase
{
    private Animator jimAnimator;

    private Vector2 leftStickInput;
    private Vector2 rightStickInput;


    void Start()
    {
        jimAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void LeftAnalogStick()
    {
        leftStickInput.x = Input.GetAxis("Horizontal");
        leftStickInput.y = Input.GetAxis("Vertical");

        jimAnimator.SetFloat("inputX", leftStickInput.x);
        jimAnimator.SetFloat("inputY", leftStickInput.y);
        jimAnimator.SetFloat("inputMagnitude", leftStickInput.magnitude);
    }

    public override void RightAnalogStick()
    {
        rightStickInput.x = Input.GetAxis("Horizontal");
        rightStickInput.y = Input.GetAxis("Vertical");
    }
}
