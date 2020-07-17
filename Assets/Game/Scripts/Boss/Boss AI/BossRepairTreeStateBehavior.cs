﻿using System.Collections;
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
        _bossController.fallenTreeList[0].ResetPull();
    }


    public override void OnStateUpdate(Animator fsm, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_bossController.bossHealth <= 0.0f)
        {
            fsm.SetTrigger("Die");
            return;
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Point To Repair"))
        {
            _animationStarted = true;
        }
        else if (_animationStarted == true)
        {
            _animationStarted = false;
            _bossController.fallenTreeList.RemoveAt(0);
            fsm.SetTrigger("Idle");
        }
    }


    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator fsm, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}