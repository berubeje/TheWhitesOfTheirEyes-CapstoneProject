using System.Collections;
using System.Collections.Generic;
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
    private float _speedMultiplier;
    private Vector3 _releaseDirection;

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
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _rigidbody.MovePosition(CalculateArcPosition());

        Debug.DrawRay(new Vector3(_animator.transform.position.x,
            _animator.transform.position.y + 1.0f, 
            _animator.transform.position.z), 
            _releaseDirection.normalized, 
            Color.cyan);
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
        Vector3 pendulumArm = _anchor.position - _animator.transform.position;

        float angle = Vector3.Angle(Vector3.up, pendulumArm);

        if (angle >= swingArcLimit)
        {
            angle = swingArcLimit;
            _direction = _direction == 1 ? -1 : 1;
        }
        float anglePercent = angle / swingArcLimit;


        _speedMultiplier = (1.05f - Mathf.Round(anglePercent * 100f) / 100f);
        _releaseDirection = _direction * Vector3.Cross(pendulumArm, -_animator.transform.right);

        Vector3 moveAmount = _animator.transform.forward * swingSpeed * _speedMultiplier * _direction;
        Vector3 newPosition = _animator.transform.position + moveAmount;
        newPosition.y = _arcOrigin.y;

        //Debug.Log(-Mathf.Pow((swingRadius * swingRadius) - (_arcOrigin - newPosition).sqrMagnitude, 0.5f));
        //newPosition.y += swingArcWidth * (_arcOrigin - newPosition).sqrMagnitude;

        newPosition.y += -Mathf.Pow((swingRadius * swingRadius) - (_arcOrigin - newPosition).sqrMagnitude, 0.5f) + swingRadius;

        _animator.SetFloat("swingDirection", _speedMultiplier * _direction);

        return newPosition;
    }
}
