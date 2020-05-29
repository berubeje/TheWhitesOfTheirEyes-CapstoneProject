using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JimController : ControllableBase
{
    public float rotationSpeed;
    public float speedDampTime;

    public float directionDampTime;
    public float directionSpeed;
    
    public float leftStickDeadzone;

    public float jumpHeight;
    public float jumpDistance;

    public float swingArcWidth;
    public float swingArcHeight;
    public float swingSpeed;

    private Vector2 _leftStickInput;

    private Vector3 _moveDirection;
    private Vector3 _leftStickDirection;

    private CapsuleCollider _capsuleCollider;
    private float _capsuleColliderHeight;
    private Rigidbody _rigidbody;

    private Animator _jimAnimator;
    private AnimatorStateInfo _stateInfo;
    private int _locomotionID;
    private int _idleID;
    private int _idleJumpID;
    private int _runJumpID;
    private int _locomotionPivotLeftID;
    private int _locomotionPivotRightID;
    private int _swingStartID;
    private int _swingIdleID;
    void Start()
    {
        _jimAnimator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _capsuleColliderHeight = _capsuleCollider.height;

        _rigidbody = GetComponent<Rigidbody>();

        _locomotionID = Animator.StringToHash("Base Layer.Locomotion");
        _idleID = Animator.StringToHash("Base Layer.Idle");
        _idleJumpID = Animator.StringToHash("Base Layer.IdleJump");
        _runJumpID = Animator.StringToHash("Base Layer.RunJump");
        _locomotionPivotLeftID = Animator.StringToHash("Base Layer.LocomotionPivotLeft");
        _locomotionPivotRightID = Animator.StringToHash("Base Layer.LocomotionPivotRight");
        _swingStartID = Animator.StringToHash("Base Layer.swingStart");
        _swingIdleID = Animator.StringToHash("Base Layer.swingIdle");
    }

    void Update()
    {
        _stateInfo = _jimAnimator.GetCurrentAnimatorStateInfo(0);
        
    }

    private void FixedUpdate()
    {
        if (IsInIdleJump())
        {
            _rigidbody.MovePosition(Vector3.up * jumpHeight * _jimAnimator.GetFloat("jumpCurve"));
            _capsuleCollider.height = _capsuleColliderHeight + (_jimAnimator.GetFloat("colliderCurve") * 0.5f);
        }

        if (IsInRunJump())
        {
            _rigidbody.MovePosition(Vector3.up * jumpHeight * _jimAnimator.GetFloat("jumpCurve"));
            _rigidbody.MovePosition(Vector3.forward * jumpDistance * Time.deltaTime);

            _capsuleCollider.height = _capsuleColliderHeight + (_jimAnimator.GetFloat("colliderCurve") * 0.5f);
        }

    }
    public override void LeftAnalogStick()
    {
        _leftStickInput.x = Input.GetAxis("Left Horizontal");
        _leftStickInput.y = Input.GetAxis("Left Vertical");

        _jimAnimator.SetFloat("leftInputX", _leftStickInput.x);
        _jimAnimator.SetFloat("leftInputY", _leftStickInput.y);

        _jimAnimator.SetFloat("leftInputMagnitude", _leftStickInput.sqrMagnitude, speedDampTime, Time.deltaTime);

        _leftStickDirection = new Vector3(_leftStickInput.x, 0.0f, _leftStickInput.y);

        // Get players forward and kill the y value
        Vector3 playerDirection = transform.forward;
        playerDirection.y = 0.0f;

        // Get camera's forward and kill the y value
        Vector3 cameraDirection = Camera.main.transform.forward;
        cameraDirection.y = 0.0f;

        // Create rotation from the players forward to the direction the joystick is being held
        Quaternion referenceShift = Quaternion.FromToRotation(Vector3.forward, cameraDirection);

        // Convert joystick input to world space
        _moveDirection = referenceShift * _leftStickDirection;

        // y value of this vector is used to figure out if the direction is left or right of the player
        Vector3 axisSign = Vector3.Cross(_moveDirection, playerDirection);

        float angle = Vector3.Angle(playerDirection, _moveDirection) * (axisSign.y > 0 ? -1.0f : 1.0f);
        float direction = (angle/180.0f) * directionSpeed;
        _jimAnimator.SetFloat("direction", direction, directionDampTime, Time.deltaTime);

       
        // TO-DO: We might need to remove this if-else block. If we get the necessary turning animations, 
        // this block will be unnecessary, as it is unwise to use root motion and physical rotation
        if (_leftStickInput.sqrMagnitude >= leftStickDeadzone)
        {
            // Directly rotate the player if the joystick is moving and they are in the idel or locomotion state
            if (IsInIdle() || IsInLocomotion()) 
            {
                Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
            }
            
        } 
        else if (_leftStickInput.sqrMagnitude < leftStickDeadzone)
        {
            // Set the direction and angle to 0 if the joystick is within deadzone
            _jimAnimator.SetFloat("direction", 0.0f);
            _jimAnimator.SetFloat("angle", 0.0f);
        }
    }

    

    public override void SouthFaceButton()
    {
        if(Input.GetButtonDown("South Face Button"))
        {
            if (IsInLocomotion() || IsInIdle())
            {
                _jimAnimator.SetTrigger("jump");
            }
        }
    }

    private Vector3 CalculateArcPosition()
    {
        Vector3 currentPosition = transform.position;
        Vector3 newPosition = Vector3.forward;

        newPosition.y = currentPosition.x - newPosition.x;
        return newPosition;
    }

    #region Utility functions to see if the animator is in the indicated state
    private bool IsInPivot()
    {
        return _stateInfo.fullPathHash == _locomotionPivotLeftID || _stateInfo.fullPathHash == _locomotionPivotRightID;
    }

    private bool IsInIdle()
    {
        return _stateInfo.fullPathHash == _idleID;
    }

    private bool IsInLocomotion()
    {
        return _stateInfo.fullPathHash == _locomotionID;
    }
    private bool IsInIdleJump()
    {
        return _stateInfo.fullPathHash == _idleJumpID;
    }
    private bool IsInRunJump()
    {
        return _stateInfo.fullPathHash == _runJumpID;
    }
    #endregion

    private void OnDrawGizmos()
    {
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), _moveDirection, Color.red);
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), _leftStickDirection, Color.green);
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), transform.forward, Color.blue);
    }
}
