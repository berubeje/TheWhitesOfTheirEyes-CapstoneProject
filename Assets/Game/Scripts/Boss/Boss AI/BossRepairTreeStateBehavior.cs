///-------------------------------------------------------------------------------------------------
// file: BossRepairTreeStateBehavior.cs
//
// author: Jesse Berube
// date: 2020-07-13
//
// summary: The repair tree state behavior. Once the boss is facing a fallen tree, they will repair them one at a time.
///-------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRepairTreeStateBehavior : StateMachineBehaviour
{
    private Animator _animator;
    private BossController _bossController;
    private bool _animationStarted;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
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

        _animator.SetTrigger("Repair");
    }


    public override void OnStateUpdate(Animator fsm, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Check the boss health. If 0 or less, switch to "Die" state.
        if (_bossController.bossHealth <= 0.0f)
        {
            fsm.SetTrigger("Die");
            return;
        }

        // Check to see if point to repait animation is playing. When the animation is finished, change states.
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Point To Repair"))
        {
            _animationStarted = true;
        }
        else if (_animationStarted == true)
        {
            _animationStarted = false;
            fsm.SetTrigger("Idle");
        }


        //if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Tree Heal End"))
        //{
        //    if (_animationStarted == false)
        //    {
        //        _bossController.fallenTreeList[0].ResetPull();
        //        _bossController.fallenTreeList.RemoveAt(0);
        //        _animationStarted = true;
        //    }
        //}
        //else if (_animationStarted == true)
        //{
        //    _animationStarted = false;
        //    fsm.SetTrigger("Idle");
        //}





    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _bossController.fallenTreeList[0].ResetPull();
        _bossController.fallenTreeList.RemoveAt(0);
    }

}
