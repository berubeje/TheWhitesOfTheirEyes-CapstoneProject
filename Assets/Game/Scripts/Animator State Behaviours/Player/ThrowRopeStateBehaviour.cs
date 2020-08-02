using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowRopeStateBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Clear all triggers
        animator.ResetTrigger("dodgeRoll");
        animator.ResetTrigger("fallIdle");
        animator.ResetTrigger("fallLand");
        animator.ResetTrigger("returnToIdle");
        animator.ResetTrigger("ropeLaunch");
        animator.ResetTrigger("swingCancel");
        animator.ResetTrigger("swingIdle");
        animator.ResetTrigger("swingLand");
        animator.ResetTrigger("swingStart");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if(_jimController.anchor.anchorType == RopeAnchorPoint.AnchorType.Swing)
        //{
        //    animator.SetTrigger("swingStart");
        //}
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
