using Cinemachine;
using Obi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

public class JimController : MonoBehaviour
{
    public float maxHealth;
    private float _health;

    // Set up event for when the players health is modified
    public float currentHealth
    {
        get { return _health; }
        set
        {
            if (_health == value)
            {
                return;
            }
            _health = value;
            if(_health == 0)
            {
                // Die stuff here 
            }
            if (OnHealthChange != null && _health != 0)
            {
                OnHealthChange(_health);
            }
        }
    }
    public delegate void OnHealthChangeDelegate(float health);
    public event OnHealthChangeDelegate OnHealthChange;

    [Header("Locomotion Settings")]
    public float rotationSpeed;
    public float speedDampTime;
    public float directionDampTime;
    public float directionSpeed;
    public float leftStickDeadzone;
    public bool isPulling = false;

    [Header("Camera Settings")]
    [Range(0.0f, 1.0f)]
    public float rightStickXDeadZone;
    [Range(0.0f, 1.0f)]
    public float rightStickYDeadZone;
    public CinemachineFreeLook freeLookCamera;
    public CinemachineVirtualCamera swingCamera;
    public DollyTrackFollow swingCameraTrack;

    [Header("Rope Settings")]
    public PlayerGrapplingHook ropeLogic;
    public RopeAnchorPoint anchor;
    public SplineRoute splineRoute;
    public float groundCheckDistance;

    [Header("Hook logic for animator")]
    public PlayerGrapplingHook hook;

    private Camera _mainCamera;

    private Vector2 _leftStickInput;
    private Vector2 _rightStickInput;

    private Vector3 _moveDirection;
    private Vector3 _leftStickDirection;

    private Animator _jimAnimator;
    private AnimatorStateInfo _stateInfo;
    private int _locomotionID;
    private int _idleID;

    void Awake()
    {
        InputManager.Instance.jimController = this;
        CheckpointManager.Instance.jimController = this;

        InputManager.Instance.BindControls();
        InputManager.Instance.EnableAllControls();

        currentHealth = maxHealth;

        _jimAnimator = GetComponent<Animator>();


        // Cache the main camera
        _mainCamera = Camera.main;

        _locomotionID = Animator.StringToHash("Base Layer.Locomotion");
        _idleID = Animator.StringToHash("Base Layer.Idle");

        // bind callback function
        OnHealthChange += OnHealthChanged;
    }

    void Update()
    {
        _stateInfo = _jimAnimator.GetCurrentAnimatorStateInfo(0);
    }

    private void FixedUpdate()
    {
        MoveAndRotatePlayer();
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
        Vector3 cameraDirection = _mainCamera.transform.forward;
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
            if (IsInState(_idleID) || IsInState(_locomotionID)) 
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

        
        freeLookCamera.m_XAxis.m_InputAxisValue = _rightStickInput.x;
        freeLookCamera.m_YAxis.m_InputAxisValue = _rightStickInput.y;
        
        
    }

    public void OnEastButtonDown(InputAction.CallbackContext context)
    {
        _jimAnimator.SetTrigger("dodgeRoll");
    }

    private void OnHealthChanged(float health)
    {
        UICanvas.Instance.ChangeHealthBar(health);
    }

    // Utility function to see if the animator is in the indicated state
    private bool IsInState(int stateHash)
    {
        return _stateInfo.fullPathHash == stateHash;
    }

    private void LaunchEvent() 
    {
        if(!_jimAnimator.GetAnimatorTransitionInfo(0).IsName("RopeLaunch -> Idle"))
        {
            ropeLogic.LaunchHook();
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), _moveDirection, Color.red);

        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), _leftStickDirection, Color.green);

        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), transform.forward, Color.blue);
    }
}
