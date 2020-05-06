using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JimController : PlayerControllerBase
{
    public float rotationSpeed; 

    private Animator jimAnimator;

    private Vector2 leftStickInput;
    private Vector2 rightStickInput;

    void Start()
    {
        jimAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

   
    public override void LeftAnalogStick()
    {
        leftStickInput.x = Input.GetAxis("Left Horizontal");
        leftStickInput.y = Input.GetAxis("Left Vertical");

        jimAnimator.SetFloat("leftInputX", leftStickInput.x);
        jimAnimator.SetFloat("leftInputY", leftStickInput.y);
        jimAnimator.SetFloat("leftInputMagnitude", leftStickInput.magnitude);

        if (leftStickInput != Vector2.zero)
        {
            Vector3 leftStickInputVector3 = new Vector3(leftStickInput.x, 0.0f, leftStickInput.y);
            Quaternion targetRotation = Quaternion.LookRotation(leftStickInputVector3);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
        }
    }

    public override void RightAnalogStick()
    {
        rightStickInput.x = Input.GetAxis("Right Horizontal");
        rightStickInput.y = Input.GetAxis("Right Vertical");
    }
}
