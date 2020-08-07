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
            if (OnHealthChange != null)
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
    public bool isReceivingLeftStick = false;
    public bool isReceivingRightStick = false;

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
    public MagicRopeProjectileLogic snakeHead;
    public RopeAnchorPoint anchor;
    public SplineRoute splineRoute;
   

    [Header("Blinking settings")]
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public float blinkRate;
    public float blinkSpeed;
    public float blinkAngleThreshold; 

    private Camera _mainCamera;

    private Vector2 _leftStickInput;
    private Vector2 _rightStickInput;

    private Vector3 _moveDirection;
    private Vector3 _leftStickDirection;

    private Animator _jimAnimator;
    private AnimatorStateInfo _stateInfo;
    
    [HideInInspector]
    public int _locomotionID;
    [HideInInspector]
    public int _idleID;
    [HideInInspector]
    public int _rollID;

    private float _blinkTimer;
    private float _initialBlinkValue;
    private float _blinkLerpValue;

    void Awake()
    {
        InputManager.Instance.jimController = this;
        CheckpointManager.Instance.jimController = this;

        currentHealth = maxHealth;
        //UICanvas.Instance.ChangeHealthBar(currentHealth);

        _jimAnimator = GetComponent<Animator>();

        // Cache the main camera
        _mainCamera = Camera.main;

        _locomotionID = Animator.StringToHash("Base Layer.Locomotion");
        _idleID = Animator.StringToHash("Base Layer.Idle");
        _rollID = Animator.StringToHash("Base Layer.DodgeRoll");

        // Initialize blink timer to 0 and get the initial value for the blend shape
        _blinkTimer = 0.0f;
        _initialBlinkValue = skinnedMeshRenderer.GetBlendShapeWeight(0);
        _blinkLerpValue = 0;

        // bind callback function
        OnHealthChange += OnHealthChanged;
    }

    private void Start()
    {
        InputManager.Instance.BindControls();
        InputManager.Instance.EnableAllControls();
    }

    void Update()
    {
        _stateInfo = _jimAnimator.GetCurrentAnimatorStateInfo(0);
        BlinkTimer();
    }

    private void FixedUpdate()
    {
        MoveAndRotatePlayer();
    }

    private void MoveAndRotatePlayer ()
    {
        _jimAnimator.SetFloat("leftInputX", _leftStickInput.x);
        _jimAnimator.SetFloat("leftInputY", _leftStickInput.y);

        if(_leftStickInput.x == 0 && _leftStickInput.y == 0)
        {
            isReceivingLeftStick = false;
        }
        else
        {
            isReceivingLeftStick = true;
        }

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
            // If the player rotates a certain amount then make the avatar blink immediately
            if(Vector3.Angle(_moveDirection, playerDirection) >= blinkAngleThreshold)
            {
                _blinkTimer = blinkRate;
            }

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

        if(_rightStickInput == Vector2.zero)
        {
            isReceivingRightStick = false;
        }
        else
        {
            isReceivingRightStick = true;
        }
        
        freeLookCamera.m_XAxis.m_InputAxisValue = _rightStickInput.x;
        freeLookCamera.m_YAxis.m_InputAxisValue = _rightStickInput.y;
        
        
    }

    public void OnEastButtonDown(InputAction.CallbackContext context)
    {
        _jimAnimator.SetTrigger("dodgeRoll");
    }

    private void OnHealthChanged(float health)
    {
        UICanvas.Instance.ChangeHealthBar(health/maxHealth);
        if(health <= 0) 
        {
            _jimAnimator.SetBool("dead", true);
        }
    }

    // Utility function to see if the animator is in the indicated state
    public bool IsInState(int stateHash)
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
    private void GameOverEvent()
    {
        InputManager.Instance.currentGameState = InputManager.GameStates.GameOver;
    }

    private void BlinkEvent()
    {
        _blinkLerpValue += blinkSpeed * Time.deltaTime;
        float blinkValue = Mathf.Lerp(_initialBlinkValue, 100.0f, _blinkLerpValue);

        skinnedMeshRenderer.SetBlendShapeWeight(0, blinkValue);

        if (_blinkLerpValue >= 1)
        {
            blinkSpeed *= -1;
            _blinkLerpValue = 1;
        }
        else if (_blinkLerpValue < 0)
        {
            blinkSpeed *= -1;
            _blinkLerpValue = 0;
            _blinkTimer = 0;
        }
    }
    private void BlinkTimer()
    {
        if(_blinkTimer >= blinkRate)
        {
            BlinkEvent();
        }
        _blinkTimer += Time.deltaTime;
    }
    private void OnDrawGizmos()
    {
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), _moveDirection, Color.red);

        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), _leftStickDirection, Color.green);

        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), transform.forward, Color.blue);
    }
}
