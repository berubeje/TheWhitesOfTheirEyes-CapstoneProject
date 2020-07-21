using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowRopeStateBehaviour : StateMachineBehaviour
{
    private JimController _jimController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _jimController = animator.GetComponent<JimController>();
        animator.ResetTrigger("returnToIdle");
        animator.ResetTrigger("ropeLaunch");
        animator.ResetTrigger("swingStart");
        animator.ResetTrigger("swingIdle");
        animator.ResetTrigger("swingLand");
        animator.ResetTrigger("swingCancel");
        animator.ResetTrigger("fallIdle");
        animator.ResetTrigger("fallLand");
        animator.ResetTrigger("dodgeRoll");
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
