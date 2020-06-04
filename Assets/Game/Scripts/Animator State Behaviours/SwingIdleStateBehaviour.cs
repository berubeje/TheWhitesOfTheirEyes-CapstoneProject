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

        _arcOrigin = new Vector3(
                       _anchor.position.x,
                       _anchor.position.y - swingRadius,
                       _anchor.position.z
                       );

        float xLimit = Mathf.Sin(swingArcLimit * Mathf.Deg2Rad) * swingRadius;
        float yLimit = Mathf.Cos(swingArcLimit * Mathf.Deg2Rad) * swingRadius;

        _forwardArcLimit = _anchor.position + (_animator.transform.forward * xLimit);
        _forwardArcLimit.y -= yLimit;

        _backwardArcLimit = _anchor.position - (_animator.transform.forward * xLimit);
        _backwardArcLimit.y -= yLimit;

        _pendulumArm = _anchor.position - _animator.transform.position;
        _angle = Vector3.Angle(Vector3.up, _pendulumArm);

        if(_angle > swingArcLimit)
        {
            InterpolateToArcLimit(_backwardArcLimit);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _rigidbody.MovePosition(CalculateArcPosition());

        Debug.DrawLine(_anchor.position, _forwardArcLimit, Color.green);

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
        //_rigidbody.isKinematic = false;
       // Debug.Log(_releaseDirection);
        //_rigidbody.AddForce(_releaseDirection, ForceMode.Impulse);
        
        
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

    public Vector3 CalculateArcPosition()
    {
        _pendulumArm = _anchor.position - _animator.transform.position;

        _angle = Vector3.Angle(Vector3.up, _pendulumArm);
        if (_angle > swingArcLimit)
        {
            _angle = swingArcLimit;
            float interpolant = 0.1f;
            switch (_direction)
            {
                case -1:
                    InterpolateToArcLimit(_backwardArcLimit);
                    _direction = 1;
                    break;
                case 1:
                    InterpolateToArcLimit(_forwardArcLimit);
                    _direction = -1;
                    break;
            }
        }

        float anglePercent = _angle / swingArcLimit;


        _speedMultiplier = (1.05f - Mathf.Round(anglePercent * 100f) / 100f);

        _releaseDirection = _direction * Vector3.Cross(_pendulumArm, -_animator.transform.right);


        Vector3 moveAmount = (_direction * _animator.transform.forward) * swingSpeed * _speedMultiplier;
        Vector3 newPosition = _animator.transform.position + moveAmount;
        newPosition.y = _arcOrigin.y;

        //Debug.Log(-Mathf.Pow((swingRadius * swingRadius) - (_arcOrigin - newPosition).sqrMagnitude, 0.5f));
        //newPosition.y += swingArcWidth * (_arcOrigin - newPosition).sqrMagnitude;

        newPosition.y += -Mathf.Pow((swingRadius * swingRadius) - (_arcOrigin - newPosition).sqrMagnitude, 0.5f) + swingRadius;

        _animator.SetFloat("swingDirection", _speedMultiplier * _direction);

        return newPosition;
    }

    private void InterpolateToArcLimit(Vector3 arcLimit) 
    {
        float interpolant = 0.1f;
        while (interpolant <= 1.0f)
        {
            _rigidbody.MovePosition(Vector3.Lerp(_animator.transform.position, arcLimit, interpolant));
            interpolant += 0.1f;
            interpolant = Mathf.Round(interpolant * 10f) / 10f;
        }
    }
}
