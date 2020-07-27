using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleLocomotionStateBehavior : StateMachineBehaviour
{
    public float groundCheckDistance;
    public Vector3 sphereCastStartOffset;

    private LayerMask _layerMask = ~(1 << 8);

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!Physics.SphereCast(animator.transform.position + sphereCastStartOffset, 0.4f, Vector3.down, out _, groundCheckDistance, _layerMask))
        {
            animator.SetBool("canRoll", true);
            animator.SetTrigger("fallIdle");
        }
    }
}
