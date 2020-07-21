///-------------------------------------------------------------------------------------------------
// file: RopeController.cs
//
// author: Jesse Berube
// date: N/A
//
// summary: The logic for controlling the rope, such as launching it and pulling with the rope, are here.
///-------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.InputSystem;


public class RopeController : MonoBehaviour
{
    public PlayerGrapplingHook ropeLogic;
    public float startingLengthOffset;
    public float stopPullingDistance = 2.0f;
    public float pullStrain = 1.15f;
    public float breakRopeLength = 10.0f;

    private JimController _playerLogic;
    private Rigidbody _playerRigidBody;
    // private Rigidbody _targetRigidBody;
    private Transform _playerTransform;
    private Transform _targetTransform;
    private Animator _animator;
    private bool _pullObject;
    private float _currentPullTime;
    private float _ropeStrain;
    private float _currentLengthOffset;
    private bool _targeting;

    private bool _isLeftTriggerInUse = false;
    private float _leftTriggerInput;


    private bool _isRightTriggerInUse = false;
    private float _rightTriggerInput;



    // Start is called before the first frame update

    private void Awake()
    {
        _playerRigidBody = GetComponentInParent<Rigidbody>();
        _playerLogic = _playerRigidBody.GetComponent<JimController>();
        _playerTransform = _playerRigidBody.transform;
        _animator = GetComponentInParent<Animator>();
        _currentLengthOffset = startingLengthOffset;

        InputManager.Instance.ropeController = this;
        ropeLogic.OnRopeStateChange += CheckRopeState;
    }



    // Update is called once per frame
    void Update()
    {
        if (ropeLogic.currentRopeState == PlayerGrapplingHook.RopeState.Pull)
        {
            _ropeStrain = ropeLogic.CalculateStrain();
        }
    }

    void FixedUpdate()
    {
        // Check to see if the rope is attatch to a pull anchor point. If so, adjust the rope so it looks tighter.
        if (ropeLogic.currentRopeState == PlayerGrapplingHook.RopeState.Pull && _targetTransform != null)
        {
            AdjustStrain();

            if (ropeLogic.GetRopeLength() <= stopPullingDistance)
            {
                return;
            }


            // Break the rope if the player walks too far from the pull anchor point.
            if (Vector3.Distance(ropeLogic.character.transform.position, ropeLogic.targetAnchor.transform.position) > breakRopeLength)
            {
                ropeLogic.DetachHook();
            }

            if (_pullObject)
            {
                PullObject();
            }
            
        }
    }

    // When the rope pulls an objects, it changes that's objects velocity so it goes towards the player and is still affected by colliders.
    private void PullObject()
    {
        Vector3 lookVector = new Vector3(_targetTransform.position.x, _playerTransform.position.y, _targetTransform.position.z);
        _playerTransform.LookAt(lookVector);


        _animator.SetFloat("pullTime", _currentPullTime / ropeLogic.targetAnchor.timeToStartPull);
        _currentPullTime += Time.deltaTime;

        if (_currentPullTime >= ropeLogic.targetAnchor.timeToStartPull)
        {
            _currentPullTime = 0.0f;
            ropeLogic.targetAnchor.StartPull();

            ropeLogic.DetachHook();
            _playerLogic.isPulling = false;
            _pullObject = false;
            _animator.SetBool("pull", false);
            _animator.SetFloat("pullTime", 0);
        }

    }


    // Adjust the strain of the rope so it will look tighter when pulling objects.
    private void AdjustStrain()
    {
        ropeLogic.AdjustRopeLength(Vector3.Distance(_playerTransform.position, _targetTransform.position) + _currentLengthOffset);

        if (_ropeStrain < pullStrain)
        {
            _currentLengthOffset -= 0.5f;
        }
    }

    // Check the state of the rope to allow different actions to happen, or to adjust the player and rope.
    public void CheckRopeState(PlayerGrapplingHook.RopeState state)
    {
        switch (state)
        {

            case PlayerGrapplingHook.RopeState.Swing:
                {
                    _playerRigidBody.isKinematic = false;
                    _playerRigidBody.useGravity = false;
                    _animator.applyRootMotion = false;

                    _targetTransform = ropeLogic.targetAnchor.transform;

                    break;
                }

            case PlayerGrapplingHook.RopeState.Pull:
                {
                    _animator.SetTrigger("returnToIdle");
                    _targetTransform = ropeLogic.targetAnchor.transform;
                    break;
                }


            case PlayerGrapplingHook.RopeState.Idle:
                {
                    _playerRigidBody.isKinematic = false;
                    _animator.applyRootMotion = true;
                    _playerRigidBody.useGravity = true;
                    _targetTransform = null;
                    _currentLengthOffset = startingLengthOffset;
                    break;
                }
        }
    }


    // Launch out the rope if there is a target in sight. If the rope is tied, the button will need to be pressed again to bring it back.
    public void OnRightTriggerDown(InputAction.CallbackContext context)
    {
        _rightTriggerInput = context.ReadValue<float>();
        if (ropeLogic.currentRopeState == PlayerGrapplingHook.RopeState.Idle)
        {
            if (_isRightTriggerInUse == false)
            {
                _animator.SetTrigger("ropeLaunch");
                _isRightTriggerInUse = true;
            }

        }
        //else if (ropeLogic.currentRopeState == PlayerGrapplingHook.RopeState.Tied)
        //{
        //    if (_isRightTriggerInUse == false)
        //    {
        //        ropeLogic.DetachHook();
        //        _isRightTriggerInUse = true;
        //    }

        //}
    }

    // Let go of the trigger to bring the rope back to the player. 
    public void OnRightTriggerUp(InputAction.CallbackContext context)
    {
        _isRightTriggerInUse = false;

        _animator.SetTrigger("returnToIdle");

        if (ropeLogic.currentRopeState == PlayerGrapplingHook.RopeState.Pull || ropeLogic.currentRopeState == PlayerGrapplingHook.RopeState.Swing)
        {
            if (_playerLogic.isPulling == false)
            {
                ropeLogic.DetachHook();
            }
        }
        else if (ropeLogic.currentRopeState == PlayerGrapplingHook.RopeState.Launched)
        {
            ropeLogic.CancelLaunch();
        }

    }

    // Hold the left trigger for a certain amount of time to start pulling in the object.
    public void OnLeftTriggerPull(InputAction.CallbackContext context)
    {
        if (ropeLogic.currentRopeState == PlayerGrapplingHook.RopeState.Pull)
        {
            _pullObject = true;
            _animator.SetBool("pull", true);
            _playerLogic.isPulling = true;
        }
    }

    //Tap the left trigger to tie the base of the rope to another anchor point.
    public void OnLeftTriggerCancel(InputAction.CallbackContext context)
    {
        // For some reason, this will trigger regardless if you hold the trigger long enough or not
        if (ropeLogic.currentRopeState == PlayerGrapplingHook.RopeState.Pull)
        {
            if (_pullObject == true)
            {
                _pullObject = false;
                _playerLogic.isPulling = false; 
                _animator.SetBool("pull", false);
                _currentPullTime = 0.0f;
            }
        }
        //else if (ropeLogic.currentRopeState == PlayerGrapplingHook.RopeState.OneEndTied)
        //{
        //    ropeLogic.TieRope();
        //}

    }
}
