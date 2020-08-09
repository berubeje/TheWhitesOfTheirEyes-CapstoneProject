using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterSwingIdleStateBehaviour : StateMachineBehaviour
{
    public float swingArcLimit;
    public float maxSwingSpeed;
    public float minSwingSpeed;
    public float swingRadius;
    public float swingRotationSpeed;
    public float forwardCheckDistance;

    [Space]
    [Header("Spline Settings")]
    [Range(1.0f, 10.0f)]
    public float releaseDirectionMagnitude;
    public float releaseDirectionOffset;
    public float minDestinationAngle;
    public float maxDestinationAngle;
    public float minReleaseDistanceX;
    public float maxReleaseDistanceX;
    public float minReleaseDistanceY;
    public float maxReleaseDistanceY;

    [Space]
    [Header("Coyote Time Settings")]
    [Range(0.0f, 1.0f)]
    public float coyoteTimeThreshold;

    private Rigidbody _rigidbody;
    private PlayerGrapplingHook _grapplingHook;
    private Transform _anchor;
    private SplineRoute _splineRoute;
    private JimController _jimController;
    private CinemachineTrackedDolly _dollyCamera;

    private Vector3 _initialSwingPosition;
    private Vector3 _swingForward;
    private Vector3 _releaseDirection;
    private Vector3 _backwardSwingLimit;
    private Vector3 _forwardSwingLimit;
    private Vector3 _backwardLimitVector;
    private Vector3 _forwardLimitVector;
    private Vector3 _currentSlerpStart;
    private Vector3 _currentSlerpEnd;
    private Vector3 _pendulumArm;
    private Vector3 _swingStartVector;

    // Parameters to handle swing rotation
    private Vector3 _swingRotationCenter;
    private Vector3 _swingCenterAxis;
    private Vector3 _forwardSwingRotation;
    private Vector3 _backwardSwingRotation;
    private bool _firstSwingComplete;

    private float _percentOfSwing;
    private float _speedMultiplier;
    private float _angle;
    private float _interpolant;
    private int _direction;
    private LayerMask _layerMask = ~(1 << 8);
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset all triggers for sanity
        //animator.ResetTrigger("swingStart");
        //animator.ResetTrigger("swingIdle");
        //animator.ResetTrigger("swingLand");
        //animator.ResetTrigger("swingCancel");
        //animator.ResetTrigger("fallLand");
        //animator.ResetTrigger("dodgeRoll");

        _grapplingHook = animator.GetComponentInChildren<PlayerGrapplingHook>();
        _rigidbody = animator.GetComponent<Rigidbody>();
        _jimController = animator.GetComponent<JimController>();
        _splineRoute = _jimController.splineRoute;

        // Checks to make sure componenets are not null
        if (_grapplingHook == null)
        {
            Debug.LogError("Unable to find PlayerGrapplingHook component in any children");
        }
        if (_rigidbody == null)
        {
            Debug.LogError("Unable to find Rigidbody component");
        }
        if (_splineRoute == null)
        {
            Debug.LogError("Unable to find Spline Route object");
        }
        _dollyCamera = _jimController.swingCamera.GetCinemachineComponent<CinemachineTrackedDolly>();

        // Cache initial position
        _initialSwingPosition = animator.transform.position;

        // Get reference to current anchor point
        _anchor = _grapplingHook.targetAnchor.transform;

        // Switch to dolly camera
        _jimController.swingCamera.Priority = 15;

        // Calculate the max x and y distance away from the origin, based on swing radius
        float xLimit = Mathf.Sin(swingArcLimit * Mathf.Deg2Rad) * swingRadius;
        float yLimit = Mathf.Cos(swingArcLimit * Mathf.Deg2Rad) * swingRadius;

        // Create forward vector because player is rotated
        _swingForward = _anchor.position - _initialSwingPosition;
        _swingForward.y = 0;
        _swingForward = _swingForward.normalized;

        // Calculate the end positions of the arc, based on the swing arc limit
        _forwardSwingLimit = _anchor.position + (_swingForward * xLimit);
        _forwardSwingLimit.y -= yLimit;

        _backwardSwingLimit = _anchor.position - (_swingForward * xLimit);
        _backwardSwingLimit.y -= yLimit;

        // Calculate the center of the rotation ring
        _swingRotationCenter = _anchor.position;
        _swingRotationCenter.y -= yLimit;

        // The vector from the center of the rotation ring to the anchor point
        _swingCenterAxis = _anchor.position - _swingRotationCenter;

        //Vectors from the center of the rotation ring to the swing limits
        _forwardSwingRotation = _forwardSwingLimit - _swingRotationCenter;
        _backwardSwingRotation = _backwardSwingLimit - _swingRotationCenter;

        // Vectors from the anchor point to the swing limits
        _backwardLimitVector = _backwardSwingLimit - _anchor.position; 
        _forwardLimitVector = _forwardSwingLimit - _anchor.position;

        // Calculate the interpolant we're starting at
        float angle = Vector3.Angle(_backwardLimitVector, animator.transform.position - _anchor.position);
        _interpolant = angle/(swingArcLimit * 2);

        _currentSlerpStart = _backwardLimitVector;
        _currentSlerpEnd = _forwardLimitVector;
        _swingStartVector = _backwardLimitVector;

        _firstSwingComplete = false;
        
        // Initialize _direction to forward
        _direction = 1; 
        animator.SetBool("canRoll", true);
        animator.SetFloat("swingDirectionRaw", _direction);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.GetAnimatorTransitionInfo(0).IsName("SwingIdle -> SwingLand") && !animator.GetAnimatorTransitionInfo(0).IsName("SwingIdle -> SwingCancel"))
        {
            RotateSwing(animator);

            _pendulumArm = _anchor.position - animator.transform.position;
            _angle = Vector3.Angle(Vector3.up, _pendulumArm);
            _angle = Mathf.Round(_angle * 10.0f) / 10.0f;

            float anglePercent = _angle / swingArcLimit;
            animator.SetFloat("angle", anglePercent);

            // Calculate speed of swing based on how close we are to the center
            _speedMultiplier = Mathf.Lerp(maxSwingSpeed, minSwingSpeed, anglePercent);
            animator.SetFloat("swingDirection", _speedMultiplier * _direction);

            // Play the whoosh sound when player is moving the fastest
            if (anglePercent <= 0.1f)
            {
                AudioManager.Instance.PlaySound("SwingWhoosh");
            }

            // Slerp between the current two heights of the swing
            Vector3 targetVector = Vector3.Slerp(_currentSlerpStart, _currentSlerpEnd, _interpolant);

            // Calculate the right direction of the swing 
            Vector3 swingRight = Vector3.Cross(_swingCenterAxis, _backwardSwingRotation).normalized;

            // Move and rotate the player
            _rigidbody.MovePosition(_anchor.position + targetVector);
            _rigidbody.MoveRotation(Quaternion.LookRotation(Vector3.Cross(_pendulumArm, swingRight)));

            // Calculate the release direction based on where we are in the swing arc 
            _releaseDirection = Vector3.Cross(_pendulumArm, swingRight * _direction).normalized * releaseDirectionMagnitude;

            // How much of the current swing we have completed
            _percentOfSwing = Vector3.Angle(_swingStartVector, -_pendulumArm) / (swingArcLimit * 2);
            animator.SetFloat("percentOfSwing", _percentOfSwing * _direction);

            // Update camera position and dolly track position/rotation
            _jimController.swingCameraTrack.transform.position = _backwardSwingLimit;
            _jimController.swingCameraTrack.transform.rotation = Quaternion.LookRotation(_swingForward);
            
            
            _interpolant += _speedMultiplier * Time.deltaTime * _direction;
            _dollyCamera.m_PathPosition = _interpolant * 2;

            SetUpSpline(animator);

            if (_interpolant >= 1.0f)
            {
                _interpolant = 1;
                _direction = -1;
                animator.SetFloat("swingDirectionRaw", _direction);
                animator.SetBool("canRoll", false);
                _swingStartVector = _forwardLimitVector;
            }
            else if(_interpolant <= 0)
            {
                _interpolant = 0;
                _direction = 1;
                animator.SetFloat("swingDirectionRaw", _direction);
                
                // Can only roll land while swinging forward
                animator.SetBool("canRoll", true);
                _swingStartVector = _backwardLimitVector;
            }
        }

        if (Physics.SphereCast(animator.transform.position + new Vector3(0, 1f, 0), 0.4f, _swingForward * _direction, out _, forwardCheckDistance, _layerMask))
        {
            _direction *= -1;

            if(_direction == 1)
            {
                _swingStartVector = _backwardLimitVector;
            }
            else if (_direction == -1)
            {
                _swingStartVector = _forwardLimitVector;

                if (!_firstSwingComplete)
                {
                    _currentSlerpStart = _backwardLimitVector;
                    _currentSlerpEnd = _forwardLimitVector;
                    _firstSwingComplete = true;
                }
            }

            animator.SetFloat("swingDirectionRaw", _direction);

        }

        Debug.DrawLine(_anchor.position, _forwardSwingLimit, Color.yellow);

        Debug.DrawLine(_anchor.position, _backwardSwingLimit, Color.red);

        Debug.DrawLine(_anchor.position, _swingRotationCenter, Color.green);
        Debug.DrawRay(_swingRotationCenter, _forwardSwingRotation, Color.yellow);
        Debug.DrawRay(_swingRotationCenter, _backwardSwingRotation, Color.red);

        Debug.DrawLine(animator.transform.position, _anchor.position, Color.white);

        Debug.DrawRay(animator.transform.position, _releaseDirection, Color.cyan);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset the freelook camera behind the player and switch back to it 
        _jimController.freeLookCamera.m_XAxis.Value = animator.transform.eulerAngles.y;
        _jimController.swingCamera.Priority = 5;
        animator.ResetTrigger("fallIdle");
    }

    // Sets that set up the spline path according to user defined parameters and current point in the swing
    private void SetUpSpline(Animator animator)
    {
        float releaseDistanceX = Mathf.Lerp(minReleaseDistanceX, maxReleaseDistanceX, _percentOfSwing);
        float releaseDistanceY = Mathf.Lerp(minReleaseDistanceY, maxReleaseDistanceY, _percentOfSwing);
        float releaseDestinationAngle = Mathf.Lerp(minDestinationAngle, maxDestinationAngle, _percentOfSwing);

        _splineRoute.controlPoints[0].position = animator.transform.position;
        _splineRoute.controlPoints[1].position = animator.transform.position + _releaseDirection + (Vector3.up * releaseDirectionOffset);

        if (_percentOfSwing > coyoteTimeThreshold)
        {
                _splineRoute.controlPoints[3].position = animator.transform.position +
                (_swingForward * releaseDistanceX) * _direction +
                (Vector3.up * releaseDistanceY);


            _splineRoute.controlPoints[2].position = (Quaternion.AngleAxis(releaseDestinationAngle * -_direction, animator.transform.right) * Vector3.up) +
                _splineRoute.controlPoints[3].position;
        }

        
    }

    private void RotateSwing(Animator animator)
    {
        float xInput = animator.GetFloat("leftInputX");

        Quaternion targetRotation = Quaternion.AngleAxis(swingRotationSpeed * xInput * Time.deltaTime, _swingCenterAxis);
        _forwardSwingRotation = targetRotation * _forwardSwingRotation;
        _backwardSwingRotation = -_forwardSwingRotation;

        _forwardSwingLimit = _swingRotationCenter + _forwardSwingRotation;
        _backwardSwingLimit = _swingRotationCenter + _backwardSwingRotation;

        _backwardLimitVector = _backwardSwingLimit - _anchor.position;
        _forwardLimitVector = _forwardSwingLimit - _anchor.position;

        // Create new forward vector 
        _swingForward = _forwardSwingRotation.normalized;

        _currentSlerpStart = _backwardLimitVector;
        _currentSlerpEnd = _forwardLimitVector;
    }
}
