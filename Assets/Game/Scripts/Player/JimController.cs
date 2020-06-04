using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class JimController : ControllableBase
{
    [Header("Locomotion Settings")]
    public float rotationSpeed;
    public float speedDampTime;
    public float directionDampTime;
    public float directionSpeed;
    public float leftStickDeadzone;

    [Header("Jump Settings")]
    public float jumpHeight;
    public float jumpDistance;

    [Header("Swing Settings")]
    public Transform anchor; 
    public float faceAnchorSpeed;
    public float swingRadius;
    public float reelInSpeed;
    public float swingArcWidth;
    public float swingArcLimit;
    public float swingSpeed;
    public float releaseForce;

    private float _speedMultiplier;
    private int _direction = 1;
    private Vector3 _arcOrigin;

    private Vector2 _leftStickInput;

    private Vector3 _moveDirection;
    private Vector3 _leftStickDirection;
    private Vector3 _releaseDirection;

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
    private int _swingLandID;

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
        _swingStartID = Animator.StringToHash("Base Layer.SwingStart");
        _swingIdleID = Animator.StringToHash("Base Layer.SwingIdle");
        _swingLandID = Animator.StringToHash("Base Layer.SwingLand");
    }

    void Update()
    {
        _stateInfo = _jimAnimator.GetCurrentAnimatorStateInfo(0);
        
    }

    private void FixedUpdate()
    {
        if (IsInIdleJump())
        {
            transform.Translate(Vector3.up * jumpHeight * _jimAnimator.GetFloat("jumpCurve"));
            _capsuleCollider.height = _capsuleColliderHeight + (_jimAnimator.GetFloat("colliderCurve") * 0.5f);
        }

        if (IsInRunJump())
        {
            transform.Translate(Vector3.up * jumpHeight * _jimAnimator.GetFloat("jumpCurve"));
            transform.Translate(Vector3.forward * jumpDistance * Time.fixedDeltaTime);

            _capsuleCollider.height = _capsuleColliderHeight + (_jimAnimator.GetFloat("colliderCurve") * 0.5f);
        }

        if (IsInSwingStart())
        {
            if(Vector3.Distance(transform.position, anchor.position) <= swingRadius)
            {
                _jimAnimator.SetBool("swingIdle", true);
            }
            else
            {
                _arcOrigin = new Vector3(
                       anchor.position.x,
                       anchor.position.y - swingRadius,
                       anchor.position.z
                       );

                Vector3 reelDirection = anchor.transform.position - transform.position;
                transform.Translate(reelDirection.normalized * reelInSpeed * Time.fixedDeltaTime, Space.World);
                reelDirection.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(reelDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, faceAnchorSpeed);
            }
        }

        if (IsInSwingIdle())
        {
            //_rigidbody.MovePosition(CalculateArcPosition());
        }

        if (IsInSwingLand())
        {
            //_releaseDirection.x *= releaseForce;
            //_releaseDirection.z *= releaseForce;
           // _rigidbody.AddForce(_releaseDirection, ForceMode.Impulse);
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

        // Create rotation from the players forward to the direction the camera is facing
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
            // Directly rotate the player if the joystick is moving and they are in the idle or locomotion state
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

    public Vector3 CalculateArcPosition()
    {
        Vector3 pendulumArm = anchor.position - transform.position;
        
        float angle = Vector3.Angle(Vector3.up, pendulumArm);

        if (angle >= swingArcLimit)
        {
             angle = swingArcLimit;
            _direction = _direction == 1 ? -1 : 1;
        }
        float anglePercent = angle / swingArcLimit;


        _speedMultiplier = (1.05f - Mathf.Round(anglePercent * 100f) / 100f);
        _releaseDirection = _direction * (_speedMultiplier) * Vector3.Cross(pendulumArm, -transform.right);


        Vector3 moveAmount = transform.forward * swingSpeed * _speedMultiplier * _direction;
        Vector3 newPosition = transform.position + moveAmount;
        newPosition.y = _arcOrigin.y;

        //Debug.Log(-Mathf.Pow((swingRadius * swingRadius) - (_arcOrigin - newPosition).sqrMagnitude, 0.5f));
        //newPosition.y += swingArcWidth * (_arcOrigin - newPosition).sqrMagnitude;

        newPosition.y += -Mathf.Pow((swingRadius * swingRadius) - (_arcOrigin - newPosition).sqrMagnitude, 0.5f) + swingRadius;

        _jimAnimator.SetFloat("swingDirection", _speedMultiplier * _direction);
        
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
    private bool IsInSwingStart()
    {
        return _stateInfo.fullPathHash == _swingStartID;
    }
    private bool IsInSwingIdle()
    {
        return _stateInfo.fullPathHash == _swingIdleID;
    }
    private bool IsInSwingLand()
    {
        return _stateInfo.fullPathHash == _swingLandID;
    }
    #endregion

    private void OnDrawGizmos()
    {
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), _moveDirection, Color.red);

        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), _leftStickDirection, Color.green);

        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), transform.forward, Color.blue);
    }
}
