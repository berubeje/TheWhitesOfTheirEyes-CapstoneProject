///-------------------------------------------------------------------------------------------------
// file: BossDieAIStateBehavior.cs
//
// author: Jesse Berube
// date: 2020-07-13
//
// summary: The die state for the boss. Sets the death in boss animator as well as disable boss logic loop
///-------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFlinchStateBehavior : StateMachineBehaviour
{
    private Animator _animator;
    private BossController _bossController;
    private bool _animationStarted;
    override public void OnStateEnter(Animator fsm, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_animator == null)
        {
            _animator = fsm.transform.parent.GetComponent<Animator>();
        }

        if (_bossController == null)
        {
            _bossController = fsm.GetComponentInParent<BossController>();
        }

        _animator.SetTrigger("Front Hit Stun");
    }

    override public void OnStateUpdate(Animator fsm, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Hit Stun"))
        {
            if (_animationStarted == false)
            {
                _animationStarted = true;
            }
        }
        else if (_animationStarted == true)
        {
            _animationStarted = false;
            _bossController.flinch = false;
            fsm.SetTrigger("Idle");
        }
    }


    public override void OnStateExit(Animator fsm, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        _animationStarted = false;
    }
}
