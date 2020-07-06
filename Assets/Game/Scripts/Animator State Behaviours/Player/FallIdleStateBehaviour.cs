using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallIdleStateBehaviour : StateMachineBehaviour
{
    public float groundCheckDistance;

    public float softLandingTime;
    public float mediumLandingTime;
    public float hardLandingTime;

    private float _fallTime;
    private bool _canRoll;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _fallTime = 0.0f;
        _canRoll = animator.GetBool("canRoll");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _fallTime += Time.deltaTime;
        animator.SetFloat("fallTime", _fallTime);
        if (Physics.SphereCast(animator.transform.position, 0.3f, Vector3.down, out _, groundCheckDistance))
        {
            if (_fallTime < softLandingTime)
            {
                animator.SetFloat("fallSpeed", 0.0f);
            }
            else if(_fallTime < mediumLandingTime && _canRoll)
            {
                animator.SetFloat("fallSpeed", 0.5f);
            }
            else if(_fallTime < hardLandingTime)
            {
                animator.SetFloat("fallSpeed", 1.0f);
            }

            animator.SetTrigger("fallLand");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
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
