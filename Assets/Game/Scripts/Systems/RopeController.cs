using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RopeController : ControllableBase
{
    public PlayerGrapplingHook ropeLogic;
    public float swingAcceleration;
    public bool lockXOnSwing = true;
    public bool pullRopeIn;
    public float startingLengthOffset;
    public float breakPullDistance = 2.0f;

    public float pullStrainThreshold = 1.15f;



    private Rigidbody _playerRigidBody;
    private Rigidbody _targetRigidBody;
    private Transform _playerTransform;
    private Transform _targetTransform;
    private Animator _animator;
    private bool _pullObject;
    private float _ropeStrain;
    private float _currentLengthOffset;
    private float _playerXRotation;

    // Start is called before the first frame update

    private void Awake()
    {
        _playerRigidBody = GetComponentInParent<Rigidbody>();
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

            if (ropeLogic.GetRopeLength() <= breakPullDistance)
            {
                return;
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
        _targetRigidBody.MovePosition(_targetTransform.position + direction * pullSpeed * Time.deltaTime);
    }

    private void AdjustStrain()
    {
        ropeLogic.AdjustRopeLength(Vector3.Distance(_playerTransform.position, _targetTransform.position) + _currentLengthOffset);

        if (_ropeStrain < pullStrainThreshold)
        {
            _currentLengthOffset -= 0.5f;
        }
    }

    public override void LeftAnalogStick()
    {
        if (ropeLogic.ropeState == PlayerGrapplingHook.RopeState.Swing)
        {
            _playerRigidBody.AddForce(new Vector3(0, 0, Input.GetAxis("Left Vertical") * swingAcceleration), ForceMode.Acceleration);

            // If enabled, this will keep the rope from sometimes starting to swing left to right, instead of forwards and backwards
            if (lockXOnSwing)
            {
                Vector3 currentVelocty = _playerRigidBody.velocity;

                currentVelocty.x = 0.0f;

                _playerRigidBody.velocity = currentVelocty;
            }
        }
    }

    public override void LeftTriggerButton()
    {
        if (ropeLogic.ropeState == PlayerGrapplingHook.RopeState.Pull)
        {
            if (Input.GetButton("Left Trigger") || Input.GetKey(KeyCode.LeftShift))
            {
                _pullObject = true;
            }
            else
            {
                _pullObject = false;
            }
        }
    }

    public override void RightTriggerButton()
    {
        if(ropeLogic.ropeState == PlayerGrapplingHook.RopeState.Idle)
        {
            if (Input.GetButtonDown("Right Trigger") || Input.GetKeyDown(KeyCode.RightShift))
            {
                ropeLogic.ActivateTargeting();
            }
            else if(Input.GetButtonUp("Right Trigger") || Input.GetKeyUp(KeyCode.RightShift))
            {
                ropeLogic.LaunchHook();
            }
        }
    }


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
                        _playerRigidBody.isKinematic = true;
                        _animator.applyRootMotion = false;
                        break;
                    }

                case PlayerGrapplingHook.RopeState.Pull:
                    {
                        _playerRigidBody.isKinematic = true;
                        _animator.applyRootMotion = true;

                        if (ropeLogic.targetAnchor.transform.parent != null)
                        {
                            _targetRigidBody = ropeLogic.targetAnchor.transform.parent.GetComponent<Rigidbody>();
                        }

                        if (_targetRigidBody == null)
                        {
                            _targetRigidBody = ropeLogic.targetAnchor.transform.GetComponent<Rigidbody>();

                            if(_targetRigidBody == null)
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
                        _targetRigidBody = null;
                        _targetTransform = null;
                        _currentLengthOffset = startingLengthOffset;
                        break;
                    }


            }

        }
    }
}
