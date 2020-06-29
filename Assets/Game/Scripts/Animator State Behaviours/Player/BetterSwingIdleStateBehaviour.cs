using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterSwingIdleStateBehaviour : StateMachineBehaviour
{
    public float swingArcLimit;
    public float swingSpeed;
    public float swingRadius;
    [Range(1.0f, 10.0f)]
    public float releaseDirectionMagnitude;
    public float releaseDirectionOffset;
    public float minDestinationAngle;
    public float maxDestinationAngle;
    public float minReleaseDistanceX;
    public float maxReleaseDistanceX;
    public float minReleaseDistanceY;
    public float maxReleaseDistanceY;

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
    private float _interpolant;
    private int _direction;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset the fall trigger so the spline curve can finish 
        animator.ResetTrigger("fallLand");

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

        _backwardLimitVector = _backwardSwingLimit - _anchor.position; 
        _forwardLimitVector = _forwardSwingLimit - _anchor.position;
        _interpolant = 0;

        _currentSlerpStart = animator.transform.position - _anchor.position;
        _currentSlerpEnd = _forwardLimitVector;

        // Initialize _direction to forward
        _direction = 1;
        #region Pass tunable paramters to the player controller, to draw the spline curve
        _jimController = animator.GetComponent<JimController>();
        _jimController.swingForward = _swingForward;
        _jimController.releaseDirectionOffset = releaseDirectionOffset;
        _jimController.minDestinationAngle = minDestinationAngle;
        _jimController.maxDestinationAngle = maxDestinationAngle;
        _jimController.minReleaseDistanceX = minReleaseDistanceX;
        _jimController.maxReleaseDistanceX = maxReleaseDistanceX;
        _jimController.minReleaseDistanceY = minReleaseDistanceY;
        _jimController.maxReleaseDistanceY = maxReleaseDistanceY;
        #endregion
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.GetAnimatorTransitionInfo(0).IsName("SwingIdle -> FallIdle"))
        {
            Vector3 targetVector = Vector3.Slerp(_currentSlerpStart, _currentSlerpEnd, _interpolant);

            _pendulumArm = animator.transform.position - _anchor.position;
            _releaseDirection = Vector3.Cross(_pendulumArm, animator.transform.right * _direction);

            _rigidbody.MovePosition(_anchor.position + targetVector);
            _rigidbody.MoveRotation(Quaternion.LookRotation(Vector3.Cross(_pendulumArm, animator.transform.right)));
            
            if(_interpolant >= 1.0f)
            {
                _direction = -1;
                _currentSlerpStart = _backwardLimitVector;
                _currentSlerpEnd = _forwardLimitVector;
            }
            else if(_interpolant < 0)
            {
                _direction = 1;
            }

            animator.SetFloat("swingDirection", _interpolant * _direction);
            _interpolant += swingSpeed * Time.deltaTime * _direction;
        }

        Debug.DrawLine(_anchor.position, _forwardSwingLimit, Color.yellow);

        Debug.DrawLine(_anchor.position, _backwardSwingLimit, Color.red);

        Debug.DrawLine(animator.transform.position, _anchor.position, Color.white);

        Debug.DrawRay(animator.transform.position, _releaseDirection, Color.cyan);

        //_jimController.speedMultiplier = _speedMultiplier;
        _jimController.releaseDirection = _releaseDirection;
        _jimController.direction = _direction;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

}
