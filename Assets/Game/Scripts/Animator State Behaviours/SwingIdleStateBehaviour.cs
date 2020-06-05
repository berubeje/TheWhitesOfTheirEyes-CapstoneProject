using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class SwingIdleStateBehaviour : StateMachineBehaviour
{
    public float swingArcLimit;
    public float swingSpeed;
    public float swingRadius;

    private Animator _animator;
    private Rigidbody _rigidbody;
    private PlayerGrapplingHook _grapplingHook;
    private Transform _anchor;

    private int _direction = 1;
    private Vector3 _arcOrigin;
    private Vector3 _pendulumArm;
    private float _angle;
    private float _speedMultiplier;
    private Vector3 _releaseDirection;
    private Vector3 _forwardArcLimit;
    private Vector3 _backwardArcLimit;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animator = animator;
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
        _forwardArcLimit = _anchor.position + (_animator.transform.forward * xLimit);
        _forwardArcLimit.y -= yLimit;

        _backwardArcLimit = _anchor.position - (_animator.transform.forward * xLimit);
        _backwardArcLimit.y -= yLimit;

        _pendulumArm = _anchor.position - _animator.transform.position;
        _angle = Vector3.Angle(Vector3.up, _pendulumArm);

        // Snap to the backward limit if the approach angle was too high
        if(_angle > swingArcLimit)
        {
            _animator.transform.position = _backwardArcLimit;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _rigidbody.MovePosition(CalculateArcPosition());

        Debug.DrawLine(_anchor.position, _forwardArcLimit, Color.yellow);

        Debug.DrawLine(_anchor.position, _backwardArcLimit, Color.red);

        Debug.DrawLine(_animator.transform.position, _anchor.position, Color.white);

        Debug.DrawRay(
            new Vector3(
                _animator.transform.position.x,
                _animator.transform.position.y + 1.0f,
                _animator.transform.position.z
                ),
            _releaseDirection,
            Color.cyan
            );
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _direction = 1;
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

    private Vector3 CalculateArcPosition()
    {
        // Get the vector between the player and the anchor and use that to get the angle
        _pendulumArm = _anchor.position - _animator.transform.position;
        _angle = Vector3.Angle(Vector3.up, _pendulumArm);
        _angle = Mathf.Round(_angle * 10.0f) / 10.0f;

        // Snap to the appropriate limit and change the direction parameter if we swing beyong the arc limit
        if (_angle > swingArcLimit)
        {
            switch (_direction)
            {
                case -1:
                    _animator.transform.position = _backwardArcLimit;
                    _direction = 1;
                    break;
                case 1:
                    _animator.transform.position = _forwardArcLimit;
                    _direction = -1;
                    break;
            }
            _angle = swingArcLimit;
        }

        float anglePercent = _angle / swingArcLimit;

        // Speed multiplier is based off position. The closer we are to the origin, the higher it is, and the faster we will move
        _speedMultiplier = _direction * (1.05f - Mathf.Round(anglePercent * 100f) / 100f);

        _releaseDirection = _direction * Vector3.Cross(_pendulumArm, -_animator.transform.right);


        Vector3 moveAmount = _animator.transform.forward * swingSpeed * _speedMultiplier;
        Vector3 newPosition = _animator.transform.position + moveAmount;
        newPosition.y = _arcOrigin.y;

        newPosition.y += -Mathf.Pow((swingRadius * swingRadius) - (_arcOrigin - newPosition).sqrMagnitude, 0.5f) + swingRadius;
        _animator.SetFloat("swingDirection", _speedMultiplier);

        return newPosition;
    }
}
