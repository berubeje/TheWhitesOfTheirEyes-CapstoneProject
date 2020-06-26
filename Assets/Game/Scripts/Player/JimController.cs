using Cinemachine;
using Obi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

public class JimController : MonoBehaviour
{
    public float playerHealth;
    
    [Header("Locomotion Settings")]
    public float rotationSpeed;
    public float speedDampTime;
    public float directionDampTime;
    public float directionSpeed;
    public float leftStickDeadzone;
    public bool isPulling = false;

    [Header("Camera Settings")]
    public CinemachineFreeLook virtualCamera;

    [Header("Jump Settings")]
    public float jumpHeight;
    public float jumpDistance;

    [Header("Swing Settings")]
    public Transform anchor;
    public SplineRoute splineRoute;

    [Header("Settings recieved from the animator. Modifying these has no effect")]
    public int direction;
    public float speedMultiplier;
    public Vector3 swingForward;
    public Vector3 releaseDirection;
    public float releaseDirectionOffset;
    public float minDestinationAngle;
    public float maxDestinationAngle;
    public float minReleaseDistanceX;
    public float maxReleaseDistanceX;
    public float minReleaseDistanceY;
    public float maxReleaseDistanceY;

    private Vector2 _leftStickInput;
    private Vector2 _rightStickInput;

    private Vector3 _moveDirection;
    private Vector3 _leftStickDirection;

    private CapsuleCollider _capsuleCollider;
    private float _capsuleColliderHeight;
    private Rigidbody _rigidbody;
    private Vector3 _reelDirection;

    private Animator _jimAnimator;
    private AnimatorStateInfo _stateInfo;
    private int _locomotionID;
    private int _idleID;
    private int _idleJumpID;
    private int _runJumpID;
    private int _locomotionPivotLeftID;
    private int _locomotionPivotRightID;
    private int _swingStartID;
    private int _fallIdleID;

    void Awake()
    {
        CheckpointManager.Instance.jimController = this;
        InputManager.Instance.jimController = this;

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
        _fallIdleID = Animator.StringToHash("Base Layer.FallIdle");
    }

    void Update()
    {
        _stateInfo = _jimAnimator.GetCurrentAnimatorStateInfo(0);
        
    }

    private void FixedUpdate()
    {
        MoveAndRotatePlayer();

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

        #region Old reel in code. Moved to animation state behaviour
        //TO-DO move this if block to statebehaviour
        //if (IsInSwingStart())
        //{
        //    if(Vector3.Distance(transform.position, anchor.position) <= swingRadius)
        //    {
        //        _jimAnimator.SetBool("swingIdle", true);
        //        _reelDirection = transform.position - anchor.transform.position;
        //        transform.position = anchor.position + (_reelDirection.normalized * swingRadius);
        //    }
        //    else
        //    {
        //        _reelDirection = anchor.transform.position - transform.position;
        //        transform.Translate(_reelDirection.normalized * reelInSpeed * Time.fixedDeltaTime, Space.World);

        //        _reelDirection.y = 0;
        //        Quaternion targetRotation = Quaternion.LookRotation(_reelDirection);
        //        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, faceAnchorSpeed);
        //    }
        //}
        #endregion
    }

    private void MoveAndRotatePlayer ()
    {
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
            if (IsInIdle() || IsInLocomotion() || IsInFallIdle()) 
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

    public void OnLeftStick(InputAction.CallbackContext context)
    {
        if (!isPulling)
        {
            _leftStickInput = context.ReadValue<Vector2>();
        }
        else
        {
            _leftStickInput = Vector2.zero;
        }
    }

    public void OnRightStick(InputAction.CallbackContext context)
    {
        _rightStickInput = context.ReadValue<Vector2>();
        //virtualCamera.m_XAxis.m_InputAxisValue = _rightStickInput.x;
    }

    public void OnEastButtonDown(InputAction.CallbackContext context)
    {
        _jimAnimator.SetTrigger("dodgeRoll");
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
    private bool IsInFallIdle()
    {
        return _stateInfo.fullPathHash == _fallIdleID;
    }
    #endregion



    private void OnDrawGizmos()
    {
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), _moveDirection, Color.red);

        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), _leftStickDirection, Color.green);

        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), transform.forward, Color.blue);

        float releaseDistanceX = 0;
        float releaseDistanceY = 0;
        float releaseDestinationAngle = 0;

        if (_jimAnimator != null)
        {
            releaseDistanceX = Mathf.Lerp(minReleaseDistanceX, maxReleaseDistanceX, _jimAnimator.GetFloat("percentOfSwing"));
            releaseDistanceY = Mathf.Lerp(minReleaseDistanceY, maxReleaseDistanceY, _jimAnimator.GetFloat("percentOfSwing"));
            releaseDestinationAngle = Mathf.Lerp(minDestinationAngle, maxDestinationAngle, _jimAnimator.GetFloat("percentOfSwing"));
        }

        Vector3 p0 = transform.position; 
        Vector3 p1 = transform.position + releaseDirection + (Vector3.up * releaseDirectionOffset);

        Vector3 p3 = transform.position +
            (swingForward * releaseDistanceX) * direction +
            (Vector3.up * releaseDistanceY);
        
        Vector3 p2 = (Quaternion.AngleAxis(releaseDestinationAngle * -direction, transform.right) * Vector3.up) + p3;

        Gizmos.color = Color.green;
        for (float t = 0.0f; t <= 1; t += 0.05f)
        {
            Vector3 gizmosPosition = Mathf.Pow(1 - t, 3) * p0 +
                3 * Mathf.Pow(1 - t, 2) * t * p1 +
                3 * (1 - t) * Mathf.Pow(t, 2) * p2 +
                Mathf.Pow(t, 3) * p3;

            Gizmos.DrawSphere(gizmosPosition, 0.05f);
        }

        Gizmos.color = Color.white;

        Gizmos.DrawLine(p0, p1);

        Gizmos.DrawLine(p2, p3);
    }
}
