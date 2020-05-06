using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JimController : IPlayerControllable
{
    public float rotationSpeed;
    public float turnAroundThreshold;

    private Animator _jimAnimator;

    private Vector2 _leftStickInput;
    private Vector2 _rightStickInput;

    private bool _isPivoting;
    void Start()
    {
        _jimAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

   
    public override void LeftAnalogStick()
    {
        _leftStickInput.x = Input.GetAxis("Left Horizontal");
        _leftStickInput.y = Input.GetAxis("Left Vertical");

        _jimAnimator.SetFloat("leftInputX", _leftStickInput.x);
        _jimAnimator.SetFloat("leftInputY", _leftStickInput.y);
        _jimAnimator.SetFloat("leftInputMagnitude", _leftStickInput.magnitude, 0.15f, Time.deltaTime);

        if (_leftStickInput != Vector2.zero)
        {
            Vector3 leftStickInputVector3 = new Vector3(_leftStickInput.x, 0.0f, _leftStickInput.y);
            Quaternion targetRotation = Quaternion.LookRotation(leftStickInputVector3);
            float angle = Quaternion.Angle(targetRotation, transform.rotation);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
            
            
        }
    }

    public override void RightAnalogStick()
    {
        _rightStickInput.x = Input.GetAxis("Right Horizontal");
        _rightStickInput.y = Input.GetAxis("Right Vertical");
    }
}
