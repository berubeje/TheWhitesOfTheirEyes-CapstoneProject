using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class SwingIdleStateBehaviour : StateMachineBehaviour
{
    public float swingArcLimit;
    public float swingSpeed;
    public float swingRadius;
    public float minReleaseDistanceX;
    public float maxReleaseDistanceX;
    public float minReleaseDistanceY;
    public float maxReleaseDistanceY;

    private Rigidbody _rigidbody;
    private PlayerGrapplingHook _grapplingHook;
    private Transform _anchor;
    private SplineRoute _splineRoute;

    private int _direction;
    private Vector3 _arcOrigin;
    private Vector3 _pendulumArm;
    private float _angle;
    private float _speedMultiplier;
    private Vector3 _releaseDirection;
    private Vector3 _forwardArcLimit;
    private Vector3 _backwardArcLimit;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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
        _forwardArcLimit = _anchor.position + (animator.transform.forward * xLimit);
        _forwardArcLimit.y -= yLimit;

        _backwardArcLimit = _anchor.position - (animator.transform.forward * xLimit);
        _backwardArcLimit.y -= yLimit;

        _pendulumArm = _anchor.position - animator.transform.position;
        _angle = Vector3.Angle(Vector3.up, _pendulumArm);

        // Snap to the backward limit if the approach angle was too high
        if(_angle >= swingArcLimit)
        {
            animator.transform.position = _backwardArcLimit;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!animator.GetAnimatorTransitionInfo(0).IsName("SwingIdle -> FallIdle"))
        {
            _rigidbody.MovePosition(CalculateArcPosition(animator));
        }

        Debug.DrawLine(_anchor.position, _forwardArcLimit, Color.yellow);

        Debug.DrawLine(_anchor.position, _backwardArcLimit, Color.red);

        Debug.DrawLine(animator.transform.position, _anchor.position, Color.white);

        Debug.DrawRay(
            new Vector3(
                animator.transform.position.x,
                animator.transform.position.y + 1.0f,
                animator.transform.position.z
                ),
            _releaseDirection,
            Color.cyan
            );
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float releaseDistanceX = Mathf.Lerp(minReleaseDistanceX, maxReleaseDistanceX, _speedMultiplier);
        float releaseDistanceY = Mathf.Lerp(minReleaseDistanceY, maxReleaseDistanceY, 1 -_speedMultiplier);

        _splineRoute.controlPoints[0].position = animator.transform.position;
        _splineRoute.controlPoints[1].position = animator.transform.position + _releaseDirection;

        _splineRoute.controlPoints[3].position = animator.transform.position + 
            (animator.transform.forward * releaseDistanceX) * _direction +
            (Vector3.up * releaseDistanceY);
        _splineRoute.controlPoints[2].position = _splineRoute.controlPoints[3].position + new Vector3(0, 2.0f * (1 - _speedMultiplier), 0);

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
                    _direction = 1;
                    break;
                case 1:
                    animator.transform.position = _forwardArcLimit;
                    _direction = -1;
                    break;
            }
            _angle = swingArcLimit;
        }

        float anglePercent = _angle / swingArcLimit;

        // Speed multiplier is based off position. The closer we are to the origin, the higher it is, and the faster we will move
        _speedMultiplier = (1.05f - Mathf.Round(anglePercent * 100f) / 100f);

        _releaseDirection = _direction * Vector3.Cross(_pendulumArm, -animator.transform.right);


        Vector3 moveAmount = animator.transform.forward * swingSpeed * _speedMultiplier *_direction;
        Vector3 newPosition = animator.transform.position + moveAmount;
        newPosition.y = _arcOrigin.y;

        newPosition.y += -Mathf.Pow((swingRadius * swingRadius) - (_arcOrigin - newPosition).sqrMagnitude, 0.5f) + swingRadius;
        animator.SetFloat("swingDirection", _speedMultiplier * _direction);

        return newPosition;
    }
}
