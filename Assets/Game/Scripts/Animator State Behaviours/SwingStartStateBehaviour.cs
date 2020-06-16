using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingStartStateBehaviour : StateMachineBehaviour
{
    public float swingRadius;
    public float faceAnchorSpeed;
    public float reelInSpeed;
    public float swingArcLimit;

    private PlayerGrapplingHook _grapplingHook;
    private Transform _anchor;
    private Rigidbody _rigidbody;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private Vector3 _lookDirection;
    private Vector3 _reelDirection;
    private Vector3 _reelLocation;
    private Quaternion _lookRotation;
    private float _interpolant;
    private float _lerpRate;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _interpolant = 0.0f;

        _grapplingHook = animator.GetComponentInChildren<PlayerGrapplingHook>();

        if (_grapplingHook == null)
        {
            Debug.LogError("Unable to find PlayerGrapplingHook component in any children");
        }

        _anchor = _grapplingHook.targetAnchor.transform;

        _rigidbody = animator.GetComponent<Rigidbody>();

        if (_rigidbody == null)
        {
            Debug.LogError("Unable to find Rigidbody component");
        }

        _initialPosition = animator.transform.position;
        _initialRotation = animator.transform.rotation;

        // Grab the direction from the player to the anchor and kill the y value
        _lookDirection = (_anchor.position - animator.transform.position);
        _lookDirection.y = Vector3.Cross(_lookDirection, -animator.transform.right).y;

        // Grab the direction from the anchor to the player and normalize it 
        _reelDirection = (animator.transform.position - _anchor.position).normalized;

        // Calculate the point to be reeled to
        _reelLocation = _anchor.position + (_reelDirection * swingRadius);

        // Direction player needs to rotate to
        _lookRotation = Quaternion.LookRotation(_lookDirection);

        _lerpRate = (reelInSpeed * Time.deltaTime) / Vector3.Distance(_reelLocation, _initialPosition);

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.DrawLine(_initialPosition, _reelLocation, Color.magenta);
        Debug.DrawRay(_initialPosition, _lookDirection, Color.red);

        Vector3 targetPosition = Vector3.Lerp(_initialPosition, _reelLocation, _interpolant);
        Quaternion targetRotation = Quaternion.Lerp(_initialRotation, _lookRotation, _interpolant);

        _rigidbody.MovePosition(targetPosition);
        _rigidbody.MoveRotation(targetRotation);

        if (_interpolant >= 1.0f)
        {
            animator.transform.position = _reelLocation;
            float cycleOffset = (swingArcLimit - Vector3.Angle(_anchor.position - animator.transform.position, Vector3.up)) / (swingArcLimit * 2);
            animator.SetFloat("cycleOffset", cycleOffset);
            animator.SetTrigger("swingIdle");
        }

        _interpolant += _lerpRate;
    }

    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

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
}
