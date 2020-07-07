using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterSwingIdleStateBehaviour : StateMachineBehaviour
{
    public float swingArcLimit;
    public float maxSwingSpeed;
    public float minSwingSpeed;
    public float swingRadius;
    public float forwardCheckDistance;
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
    public float swingRotationSpeed;
    public float swingCameraFollowSpeed;

    private Rigidbody _rigidbody;
    private PlayerGrapplingHook _grapplingHook;
    private Transform _anchor;
    private SplineRoute _splineRoute;
    private JimController _jimController;

    private Vector3 _initialSwingPosition;
    private Quaternion _initialSwingRotation;
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
    private int _layerMask = ~(1 << 8);
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset all triggers for sanity
        animator.ResetTrigger("swingStart");
        animator.ResetTrigger("swingIdle");
        animator.ResetTrigger("swingLand");
        animator.ResetTrigger("swingCancel");
        animator.ResetTrigger("fallLand");
        animator.ResetTrigger("dodgeRoll");

        _grapplingHook = animator.GetComponentInChildren<PlayerGrapplingHook>();
        _rigidbody = animator.GetComponent<Rigidbody>();
        _splineRoute = animator.GetComponent<JimController>().splineRoute;

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

        _jimController.cameraFollowTarget.followSpeed = swingCameraFollowSpeed;

        // Cache initial position and rotation
        _initialSwingPosition = animator.transform.position;
        _initialSwingRotation = animator.transform.rotation;

        // Get reference to current anchor point
        _anchor = _grapplingHook.targetAnchor.transform;

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
        _interpolant = 0;

        _currentSlerpStart = animator.transform.position - _anchor.position;
        _currentSlerpEnd = _forwardLimitVector;
        _swingStartVector = _backwardLimitVector;

        _firstSwingComplete = false;
        
        // Initialize _direction to forward
        _direction = 1;
        animator.SetFloat("swingDirectionRaw", _direction);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.GetAnimatorTransitionInfo(0).IsName("SwingIdle -> SwingLand") && !animator.GetAnimatorTransitionInfo(0).IsName("SwingIdle -> SwingCancel"))
        {
            _pendulumArm = _anchor.position - animator.transform.position;
            _angle = Vector3.Angle(Vector3.up, _pendulumArm);
            _angle = Mathf.Round(_angle * 10.0f) / 10.0f;
            float anglePercent = _angle / swingArcLimit;

            // Calculate speed of swing based on how close we are to the center
            _speedMultiplier = Mathf.Lerp(maxSwingSpeed, minSwingSpeed, anglePercent);

            // Slerp between the current two heights of the swing
            Vector3 targetVector = Vector3.Slerp(_currentSlerpStart, _currentSlerpEnd, _interpolant);

            // Calculate the right direction of the swing 
            Vector3 swingRight = Vector3.Cross(_swingCenterAxis, _backwardSwingRotation).normalized;


            _rigidbody.MovePosition(_anchor.position + targetVector);
            _rigidbody.MoveRotation(Quaternion.LookRotation(Vector3.Cross(_pendulumArm, swingRight)));

            _releaseDirection = Vector3.Cross(-_pendulumArm, swingRight * _direction).normalized * releaseDirectionMagnitude;
            _percentOfSwing = Vector3.Angle(_swingStartVector, -_pendulumArm) / (swingArcLimit * 2);
            animator.SetFloat("percentOfSwing", _percentOfSwing * _direction);

            _interpolant += _speedMultiplier * Time.deltaTime * _direction;

            RotateSwing(animator);

            if (_interpolant > 1.0f)
            {
                _direction = -1;
                animator.SetFloat("swingDirectionRaw", _direction);
                animator.SetBool("canRoll", false);

                if (!_firstSwingComplete)
                {
                    _currentSlerpStart = _backwardLimitVector;
                    _currentSlerpEnd = _forwardLimitVector;
                    _firstSwingComplete = true;
                }
                _swingStartVector = _forwardLimitVector;
            }
            else if(_interpolant < 0)
            {
                _direction = 1;
                animator.SetFloat("swingDirectionRaw", _direction);
                animator.SetBool("canRoll", true);
                _swingStartVector = _backwardLimitVector;
            }
            animator.SetFloat("angle", anglePercent);
            animator.SetFloat("swingDirection", _speedMultiplier * _direction);

            SetUpSpline(animator);

        }

        if (Physics.SphereCast(animator.transform.position, 0.3f, _swingForward * _direction, out _, forwardCheckDistance, _layerMask))
        {
            animator.SetTrigger("swingCancel");
        }

        Debug.DrawLine(_anchor.position, _forwardSwingLimit, Color.yellow);

        Debug.DrawLine(_anchor.position, _backwardSwingLimit, Color.red);

        Debug.DrawLine(_anchor.position, _swingRotationCenter, Color.green);
        Debug.DrawRay(_swingRotationCenter, _forwardSwingRotation, Color.yellow);
        Debug.DrawRay(_swingRotationCenter, _backwardSwingRotation, Color.red);

        Debug.DrawLine(animator.transform.position, _anchor.position, Color.white);

        Debug.DrawRay(animator.transform.position, _releaseDirection, Color.cyan);
    }

    // Sets that set up the spline path according to user defined parameters and current point in the swing
    private void SetUpSpline(Animator animator)
    {
        float releaseDistanceX = Mathf.Lerp(minReleaseDistanceX, maxReleaseDistanceX, _percentOfSwing);
        float releaseDistanceY = Mathf.Lerp(minReleaseDistanceY, maxReleaseDistanceY, _percentOfSwing);
        float releaseDestinationAngle = Mathf.Lerp(minDestinationAngle, maxDestinationAngle, _percentOfSwing);

        _splineRoute.controlPoints[0].position = animator.transform.position;
        _splineRoute.controlPoints[1].position = animator.transform.position + _releaseDirection + (Vector3.up * releaseDirectionOffset);

        _splineRoute.controlPoints[3].position = animator.transform.position +
            (_swingForward * releaseDistanceX) * _direction +
            (Vector3.up * releaseDistanceY);


        _splineRoute.controlPoints[2].position = (Quaternion.AngleAxis(releaseDestinationAngle * -_direction, animator.transform.right) * Vector3.up) +
            _splineRoute.controlPoints[3].position;
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

        if (_firstSwingComplete)
        {
            _currentSlerpStart = _backwardLimitVector;
        }
        _currentSlerpEnd = _forwardLimitVector;
    }
}
