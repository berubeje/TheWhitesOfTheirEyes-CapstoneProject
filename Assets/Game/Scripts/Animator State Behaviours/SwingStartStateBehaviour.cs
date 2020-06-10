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
    private Vector3 _lookDirection;
    private Vector3 _reelDirection;
    private Vector3 _reelLocation;
    private float _interpolant = 0.0f;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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

        // Grab the direction from the player to the anchor and kill the y value
        _lookDirection = _anchor.position - animator.transform.position;
        _lookDirection.y *= -1;

        // Grab the direction from the anchor to the player and normalize it 
        _reelDirection = (animator.transform.position - _anchor.position).normalized;

        // Calculate the point to be reeled to
        _reelLocation = _anchor.position + (_reelDirection * swingRadius);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.DrawLine(_initialPosition, _reelLocation, Color.magenta);

        animator.transform.Translate(-_reelDirection * reelInSpeed * Time.deltaTime, Space.World);

        Quaternion targetRotation = Quaternion.LookRotation(_lookDirection);
        animator.transform.rotation = Quaternion.RotateTowards(animator.transform.rotation, targetRotation, faceAnchorSpeed);

        if (Vector3.Distance(animator.transform.position, _anchor.position) <= swingRadius)
        {
            animator.transform.position = _reelLocation;
            float cycleOffset = (swingArcLimit - Vector3.Angle(_anchor.position - animator.transform.position, Vector3.up)) / (swingArcLimit * 2);
            animator.SetFloat("cycleOffset", cycleOffset);
            animator.SetTrigger("swingIdle");
        }
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
