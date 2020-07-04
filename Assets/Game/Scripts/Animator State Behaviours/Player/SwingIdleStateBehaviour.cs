using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class SwingIdleStateBehaviour : StateMachineBehaviour
{
    public float lerpToStartPointSpeed;
    public float swingArcLimit;
    public float swingSpeed;
    public float swingRadius;
    public float rotationSpeed;
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

    private int _direction;
    private Vector3 _arcOrigin;
    private Vector3 _pendulumArm;
    private float _angle;
    private float _speedMultiplier;
    private Vector3 _releaseDirection;
    private Vector3 _forwardArcLimit;
    private Vector3 _backwardArcLimit;
    private float _percentOfSwing;
    private Vector3 _swingStartPoint;
    private Vector3 _swingForward;

    // Parameters to lerp to height of the swing if player begins from beyond the swing arc limit
    private bool _isBeyondArcLimit;
    private Vector3 _initialSwingPosition;
    private Quaternion _initialSwingRotation;
    private Vector3 _lookDirection;
    private Quaternion _lookRotation;
    private float _lerpRate;
    private float _interpolant;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset the fall trigger so the spline curve can finish 
        animator.ResetTrigger("fallLand");

        _grapplingHook = animator.GetComponentInChildren<PlayerGrapplingHook>();

        if(_grapplingHook == null)
        {
            Debug.LogError("Unable to find PlayerGrapplingHook component in any children");
        }

        _anchor = _grapplingHook.targetAnchor.transform;

        _rigidbody = animator.GetComponent<Rigidbody>();

        if (_rigidbody == null)
        {
            Debug.LogError("Unable to find Rigidbody component");
        }

        _splineRoute = animator.GetComponent<JimController>().splineRoute;

        if (_splineRoute == null)
        {
            Debug.LogError("Unable to find Spline Route object");
        }

        _initialSwingPosition = animator.transform.position;
        _initialSwingRotation = animator.transform.rotation;

        // Create forward vector because player is rotated
        _swingForward = _anchor.position - _initialSwingPosition;
        _swingForward.y = 0;
        _swingForward = _swingForward.normalized;


        // Initialize direction to forward
        _direction = 1;

        

        // Set the origin of the arc, just below the anchor point
        _arcOrigin = new Vector3(
                       _anchor.position.x,
                       _anchor.position.y - swingRadius,
                       _anchor.position.z
                       );

        // Calculate the max x and y distance away from the origin, based on swing radius
        float xLimit = Mathf.Sin(swingArcLimit * Mathf.Deg2Rad) * swingRadius;
        float yLimit = Mathf.Cos(swingArcLimit * Mathf.Deg2Rad) * swingRadius;

        // Calculate the end positions of the arc, based on the swing arc limit
        _forwardArcLimit = _anchor.position + (_swingForward * xLimit);
        _forwardArcLimit.y -= yLimit;

        _backwardArcLimit = _anchor.position - (_swingForward * xLimit);
        _backwardArcLimit.y -= yLimit;

        _pendulumArm = _anchor.position - _initialSwingPosition;

        _angle = Vector3.Angle(Vector3.up, _pendulumArm);
        _angle = Mathf.Round(_angle * 10.0f) / 10.0f;

        _swingStartPoint = _backwardArcLimit;
        _percentOfSwing = Vector3.Angle(_swingStartPoint - _anchor.position, -_pendulumArm) / (swingArcLimit*2);
        animator.SetFloat("percentOfSwing", _percentOfSwing);

        // Lerp to the backward limit if the approach angle was too high
        if(_angle > swingArcLimit)
        {
            _lerpRate = (lerpToStartPointSpeed * Time.deltaTime) / swingRadius;

            _lookDirection = Quaternion.AngleAxis(90, animator.transform.right) * (_anchor.position - _backwardArcLimit);
            _lookRotation = Quaternion.LookRotation(_lookDirection);
            _interpolant = 0.0f;
            _isBeyondArcLimit = true;
        }

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!animator.GetAnimatorTransitionInfo(0).IsName("SwingIdle -> FallIdle"))
        {
            if (_isBeyondArcLimit)
            {
                Debug.DrawRay(_initialSwingPosition, _lookDirection, Color.blue);
                Vector3 targetPosition = Vector3.Lerp(_initialSwingPosition, _backwardArcLimit, _interpolant);
                Quaternion targetRotation = Quaternion.Lerp(_initialSwingRotation, _lookRotation, _interpolant);
                _rigidbody.MovePosition(targetPosition);
                _rigidbody.MoveRotation(targetRotation);
                if (_interpolant >= 1)
                {
                    _isBeyondArcLimit = false;
                    _interpolant = 0.0f;
                }
                _interpolant += _lerpRate;
            }
            else
            {
                _rigidbody.MovePosition(CalculateArcPosition(animator));
                _rigidbody.MoveRotation(Quaternion.LookRotation(_releaseDirection * _direction));
            }
            
        }
        else
        {
            Quaternion targetRotation = Quaternion.LookRotation(_swingForward);
            animator.transform.rotation = Quaternion.RotateTowards(animator.transform.rotation, targetRotation, rotationSpeed);
        }

        Debug.DrawLine(_anchor.position, _forwardArcLimit, Color.yellow);

        Debug.DrawLine(_anchor.position, _backwardArcLimit, Color.red);

        Debug.DrawLine(animator.transform.position, _anchor.position, Color.white);

        Debug.DrawRay(animator.transform.position, _releaseDirection, Color.cyan);

    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
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

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    private Vector3 CalculateArcPosition(Animator animator)
    {
        // Get the vector between the player and the anchor and use that to get the angle
        _pendulumArm = _anchor.position - animator.transform.position;
        _angle = Vector3.Angle(Vector3.up, _pendulumArm);
        _angle = Mathf.Round(_angle * 10.0f) / 10.0f;

        // Snap to the appropriate limit and change the direction parameter if we swing beyong the arc limit
        if (_angle > swingArcLimit)
        {
            switch (_direction)
            {
                case -1:
                    animator.transform.position = _backwardArcLimit;
                    _swingStartPoint = _backwardArcLimit;
                    _direction = 1;
                    break;
                case 1:
                    animator.transform.position = _forwardArcLimit;
                    _swingStartPoint = _forwardArcLimit;
                    _direction = -1;
                    break;
            }
            _angle = swingArcLimit;
        }

        float anglePercent = _angle / swingArcLimit;
        _percentOfSwing = Vector3.Angle(_swingStartPoint - _anchor.position, -_pendulumArm) / (swingArcLimit * 2);
        animator.SetFloat("percentOfSwing", _percentOfSwing);

        // Speed multiplier is based off position. The closer we are to the origin, the higher it is, and the faster we will move
        _speedMultiplier = (1.05f - Mathf.Round(anglePercent * 100f) / 100f);

        // Calculate the direction the player should be launched when releasing the rope
        Vector3 normalizedDirection = Vector3.Cross(_pendulumArm, -animator.transform.right).normalized;
        _releaseDirection = (_direction * normalizedDirection * releaseDirectionMagnitude) + (Vector3.up * releaseDirectionOffset);


        Vector3 moveAmount = _swingForward * swingSpeed * _speedMultiplier *_direction * Time.deltaTime;
        Vector3 newPosition = animator.transform.position + moveAmount;
        newPosition.y = _arcOrigin.y;

        newPosition.y += -Mathf.Pow((swingRadius * swingRadius) - (_arcOrigin - newPosition).sqrMagnitude, 0.5f) + swingRadius;
        animator.SetFloat("swingDirection", _speedMultiplier * _direction);

        return newPosition;
    }
}
