using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
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
    private Rigidbody _targetRigidBody;
    private Transform _playerTransform;
    private Transform _targetTransform;
    private Animator _animator;
    private bool _pullObject;
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
    }



    // Update is called once per frame
    void Update()
    {
        CheckRopeState();

        if (ropeLogic.ropeState == PlayerGrapplingHook.RopeState.Pull)
        {
            _ropeStrain = ropeLogic.CalculateStrain();
        }
    }

    void FixedUpdate()
    {
        if (ropeLogic.ropeState == PlayerGrapplingHook.RopeState.Pull && _targetTransform != null)
        {
            AdjustStrain();

            if (ropeLogic.GetRopeLength() <= stopPullingDistance)
            {
                return;
            }

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

    private void PullObject()
    {
        float pullSpeed = ropeLogic.targetAnchor.pullSpeed;

        Vector3 lookVector = new Vector3(_targetTransform.position.x, _playerTransform.position.y, _targetTransform.position.z);
        _playerTransform.LookAt(lookVector);

        Vector3 direction = (_playerTransform.position - _targetTransform.position).normalized;
        direction.y = 0;
        //_targetRigidBody.MovePosition(_targetTransform.position + direction * pullSpeed * Time.deltaTime);
        _targetRigidBody.velocity = direction * pullSpeed;
    }

    private void AdjustStrain()
    {
        ropeLogic.AdjustRopeLength(Vector3.Distance(_playerTransform.position, _targetTransform.position) + _currentLengthOffset);

        if (_ropeStrain < pullStrain)
        {
            _currentLengthOffset -= 0.5f;
        }
    }

    //public void LeftTriggerButton()
    //{
    //    if (ropeLogic.ropeState == PlayerGrapplingHook.RopeState.Pull)
    //    {
    //        if (Input.GetAxisRaw("Left Trigger") > 0)
    //        {
    //            if (_isLeftTriggerInUse == false)
    //            {
    //                _pullObject = true;
    //                _playerLogic.isPulling = true;
    //                _isLeftTriggerInUse = true;
    //            }
    //        }
    //        if (Input.GetAxisRaw("Left Trigger") <= 0)
    //        {
    //            _pullObject = false;
    //            _playerLogic.isPulling = false;
    //            _isLeftTriggerInUse = false;
    //        }
    //    }
    //}

    //public void RightTriggerButton()
    //{
    //    if (ropeLogic.ropeState == PlayerGrapplingHook.RopeState.Idle)
    //    {
    //        //if (Input.GetAxisRaw("Right Trigger") > 0)
    //        //{
    //        //    if (_isRightTriggerInUse == false)
    //        //    {
    //        //        _targeting = true;
    //        //        ropeLogic.ActivateTargeting();
    //        //        _isRightTriggerInUse = true;
    //        //    }
    //        //}
    //        //if (Input.GetAxisRaw("Right Trigger") <= 0)
    //        //{
    //        //    if (_targeting)
    //        //    {
    //        //        _targeting = false;
    //        //        ropeLogic.LaunchHook();
    //        //    }
    //        //    _isRightTriggerInUse = false;
    //        //}

    //        if (Input.GetAxisRaw("Right Trigger") > 0)
    //        {
    //            if (_isRightTriggerInUse == false)
    //            {
    //                ropeLogic.LaunchHook();
    //                _isRightTriggerInUse = true;
    //            }
    //        }
    //        if (Input.GetAxisRaw("Right Trigger") <= 0)
    //        {
    //            _isRightTriggerInUse = false;
    //        }
    //    }
    //    else if (ropeLogic.ropeState == PlayerGrapplingHook.RopeState.Pull || ropeLogic.ropeState == PlayerGrapplingHook.RopeState.Swing)
    //    {
    //        if (Input.GetAxisRaw("Right Trigger") > 0)
    //        {
    //            if (_isRightTriggerInUse == false)
    //            {
    //                ropeLogic.DetachHook();
    //                _isRightTriggerInUse = true;
    //                _playerLogic.isPulling = false;
    //            }
    //        }
    //        if (Input.GetAxisRaw("Right Trigger") <= 0)
    //        {
    //            _isRightTriggerInUse = false;
    //        }
    //    }
    //}

    //public override void LeftShoulderButton()
    //{
    //    if(Input.GetButtonDown("Left Bumper"))
    //    {
    //        targetingCone.PreviousTarget();
    //    }
    //}

    //public override void RightShoulderButton()
    //{
    //    if (Input.GetButtonDown("Right Bumper"))
    //    {
    //        targetingCone.NextTarget();
    //    }
    //}


    private void CheckRopeState()
    {
        if (ropeLogic != null)
        {
            switch (ropeLogic.ropeState)
            {
                case PlayerGrapplingHook.RopeState.Launched:
                    {
                        _playerRigidBody.isKinematic = true;
                        break;
                    }

                case PlayerGrapplingHook.RopeState.Landed:
                    {
                        _playerRigidBody.isKinematic = false;
                        break;
                    }

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
                        _playerRigidBody.isKinematic = false;
                        _animator.applyRootMotion = true;

                        
                        if (ropeLogic.targetAnchor.transform.parent != null)
                        {
                            _targetRigidBody = ropeLogic.targetAnchor.transform.parent.GetComponent<Rigidbody>();
                        }

                        if (_targetRigidBody == null)
                        {
                            _targetRigidBody = ropeLogic.targetAnchor.transform.GetComponent<Rigidbody>();

                            if (_targetRigidBody == null)
                            {
                                Debug.LogError("The pull target does not have a rigid body");
                                return;
                            }
                        }

                        _targetTransform = _targetRigidBody.transform;
                        break;
                    }

                case PlayerGrapplingHook.RopeState.Idle:
                    {
                        _playerRigidBody.isKinematic = false;
                        _animator.applyRootMotion = true;
                        _playerRigidBody.useGravity = true;
                        _targetRigidBody = null;
                        _targetTransform = null;
                        _currentLengthOffset = startingLengthOffset;
                        break;
                    }


            }

        }
    }

    public void OnRightTriggerDown(InputAction.CallbackContext context)
    {
        _rightTriggerInput = context.ReadValue<float>();
        if (ropeLogic.ropeState == PlayerGrapplingHook.RopeState.Idle)
        {
            if (_isRightTriggerInUse == false)
            {
                ropeLogic.LaunchHook();
                _isRightTriggerInUse = true;
            }

        }
        else if (ropeLogic.ropeState == PlayerGrapplingHook.RopeState.Pull || ropeLogic.ropeState == PlayerGrapplingHook.RopeState.Swing || ropeLogic.ropeState ==  PlayerGrapplingHook.RopeState.Tied)
        {
            if (_isRightTriggerInUse == false)
            {
                ropeLogic.DetachHook();
                _isRightTriggerInUse = true;
                _playerLogic.isPulling = false;
            }

        }
    }

    public void OnRightTriggerUp(InputAction.CallbackContext context)
    {
        _isRightTriggerInUse = false;
    }

    //public void OnLeftTriggerDown(InputAction.CallbackContext context)
    //{
    //    Debug.Log("Left Down");

    //    _leftTriggerInput = context.ReadValue<float>();


    //    if (_isLeftTriggerInUse == false)
    //    {
    //        _isLeftTriggerInUse = true;
    //    }
    //}

    //public void OnLeftTriggerUp(InputAction.CallbackContext context)
    //{
    //    Debug.Log("Left Up");

    //    _isLeftTriggerInUse = false;
    //}

    public void OnLeftTriggerPull(InputAction.CallbackContext context)
    {
        if (ropeLogic.ropeState == PlayerGrapplingHook.RopeState.Pull)
        {
            Debug.Log("Pull");

            _pullObject = true;
            _playerLogic.isPulling = true;
        }
    }

    public void OnLeftTriggerTie(InputAction.CallbackContext context)
    {   
        // For some reason, this will trigger regardless if you hold the trigger long enough or not
        if (ropeLogic.ropeState == PlayerGrapplingHook.RopeState.Pull)
        {
            if (_pullObject == true)
            {
                _pullObject = false;
                _playerLogic.isPulling = false;
            }
            else
            {
                ropeLogic.TieRope();
                //Debug.Log("Tie");
            }
        }
    }
}
