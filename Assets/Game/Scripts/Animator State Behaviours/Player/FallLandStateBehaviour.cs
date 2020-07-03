using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallLandStateBehaviour : StateMachineBehaviour
{
    [Range(0.0f, 1.0f)]
    public float colliderSizeMultiplier;

    [Range(0.0f, 1.0f)]
    public float colliderYOffsetMultiplier;

    private CapsuleCollider _capsuleCollider;
    private float _capsuleColliderHeight;
    private Vector3 _capsuleColliderCenter;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _capsuleCollider = animator.GetComponent<CapsuleCollider>();
        if (_capsuleCollider == null)
        {
            Debug.LogError("Capsule Collider not found");
        }
        _capsuleColliderHeight = _capsuleCollider.height;
        _capsuleColliderCenter = _capsuleCollider.center;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _capsuleCollider.height = _capsuleColliderHeight + (animator.GetFloat("colliderCurve") * colliderSizeMultiplier);
        Vector3 newCenter = _capsuleColliderCenter;
        newCenter.y += (animator.GetFloat("colliderCurve") * colliderYOffsetMultiplier);
        _capsuleCollider.center = newCenter;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("swingStart");
        animator.ResetTrigger("swingIdle");
        animator.ResetTrigger("swingLand");
        animator.ResetTrigger("swingCancel"); 
        animator.ResetTrigger("fallIdle");
        animator.ResetTrigger("fallLand");
        animator.ResetTrigger("dodgeRoll");
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
}
