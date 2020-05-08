using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JimController : IPlayerControllable
{
    public float rotationSpeed;
    public float speedDampTime;
    public float directionDampTime;
    public float directionSpeed;
    public float leftStickDeadzone;

    private Animator _jimAnimator;
    private AnimatorStateInfo stateInfo;

    private Vector2 _leftStickInput;
    private Vector2 _rightStickInput;

    private Vector3 moveDirection;
    private Vector3 leftStickDirection;

    private int locomotionID;
    private int locomotionPivotLeftID;
    private int locomotionPivotRightID;
    void Start()
    {
        _jimAnimator = GetComponent<Animator>();

        locomotionID = Animator.StringToHash("Base Layer.Locomotion");
        locomotionPivotLeftID = Animator.StringToHash("Base Layer.LocomotionPivotLeft");
        locomotionPivotRightID = Animator.StringToHash("Base Layer.LocomotionPivotRight");
    }

    void Update()
    {
        stateInfo = _jimAnimator.GetCurrentAnimatorStateInfo(0);
    }

   
    public override void LeftAnalogStick()
    {
        _leftStickInput.x = Input.GetAxis("Left Horizontal");
        _leftStickInput.y = Input.GetAxis("Left Vertical");

        _jimAnimator.SetFloat("leftInputX", _leftStickInput.x);
        _jimAnimator.SetFloat("leftInputY", _leftStickInput.y);

        _jimAnimator.SetFloat("leftInputMagnitude", _leftStickInput.sqrMagnitude, speedDampTime, Time.deltaTime);

        leftStickDirection = new Vector3(_leftStickInput.x, 0.0f, _leftStickInput.y);

        // Get players forward and kill the y value
        Vector3 playerDirection = transform.forward;
        playerDirection.y = 0.0f;

        // Create rotation from the players forward to the direction the joystick is being held
        Quaternion referenceShift = Quaternion.FromToRotation(playerDirection, leftStickDirection);

        // Convert joystick input to world space
        moveDirection = referenceShift * leftStickDirection;

        // y value of this vector is used to figure out if the direction is left or right of the player
        Vector3 axisSign = Vector3.Cross(moveDirection, transform.forward);

        float angle = Vector3.Angle(transform.forward, moveDirection) * (axisSign.y > 0 ? -1.0f : 1.0f);

        if (!IsInPivot())
        {
            _jimAnimator.SetFloat("angle", angle);
        }
        
        float direction = (angle/180.0f) * directionSpeed;
        _jimAnimator.SetFloat("direction", direction, directionDampTime, Time.deltaTime);

        // Directly rotate the player if the joystick is moving 
        if (_leftStickInput.sqrMagnitude >= leftStickDeadzone)
        {
            Quaternion targetRotation = Quaternion.LookRotation(leftStickDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
        }

        // Set the direction and angle to 0 if the joystick is within deadzone
        if (_leftStickInput.sqrMagnitude < leftStickDeadzone)
        {
            _jimAnimator.SetFloat("direction", 0.0f);
            _jimAnimator.SetFloat("angle", 0.0f);
        }
    }

    public override void RightAnalogStick()
    {
        _rightStickInput.x = Input.GetAxis("Right Horizontal");
        _rightStickInput.y = Input.GetAxis("Right Vertical");
    }

    // Returns true if the animator is in either of the locomotion pivot states
    private bool IsInPivot()
    {
        return stateInfo.fullPathHash == locomotionPivotLeftID || stateInfo.fullPathHash == locomotionPivotRightID;
    }

    private bool IsInLocomotion()
    {
        return stateInfo.fullPathHash == locomotionID;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), moveDirection, Color.red);
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), leftStickDirection, Color.green);
    }

    
}
