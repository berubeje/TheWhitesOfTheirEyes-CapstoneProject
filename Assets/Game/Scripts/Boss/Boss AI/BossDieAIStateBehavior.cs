using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDieAIStateBehavior : StateMachineBehaviour
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

        _animator.SetTrigger("Die");
        _bossController.enabled = false;
    }



}
