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
    public bool interuptRepair = false;

    private Animator _animator;
    private Animator _fsm;
    private BossController _bossController;
    private bool _firstAnimationStarted;
    private bool _secondAnimationStarted;

    private float currentTime = 0.0f;

    private BossTreeLogic _tree;
    private PlayerGrapplingHook _hook;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator fsm, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_animator == null)
        {
            _animator = fsm.transform.parent.GetComponent<Animator>();
            _fsm = fsm;
        }

        if (_bossController == null)
        {
            _bossController = fsm.GetComponentInParent<BossController>();
        }

        _bossController.flinchEvent.AddListener(Flinch);
        _tree = _bossController.fallenTreeList[0];
        _animator.SetTrigger("Heal Start");
        _hook = _bossController.player.ropeLogic;

        AudioManager.Instance.PlaySound("TreeRepair");
    }


    public override void OnStateUpdate(Animator fsm, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Tree Heal Start"))
        {
            if (_firstAnimationStarted == false)
            {
                if (_hook.targetAnchor == null || _hook.targetAnchor.transform.root != _bossController.fallenTreeList[0].transform.root)
                {
                    _tree.StartHeal();
                    _bossController.ToggleHealParticles(true);
                }

                _firstAnimationStarted = true;

            }
        }
        else if (_firstAnimationStarted == true)
        {
            _firstAnimationStarted = false;

        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Tree Heal End"))
        {
            if (_secondAnimationStarted == false)
            {
                _secondAnimationStarted = true;
                _bossController.ToggleHealParticles(false);
            }
        }
        else if (_secondAnimationStarted == true)
        {
            _secondAnimationStarted = false;
            fsm.SetTrigger("Idle");
        }
    }

    public override void OnStateExit(Animator fsm, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _firstAnimationStarted = false;
        _secondAnimationStarted = false;
        _bossController.flinchEvent.RemoveListener(Flinch);
    }

    private void Flinch()
    {
        _fsm.SetTrigger("Flinch");
        _bossController.ToggleHealParticles(false);

        if (interuptRepair)
        {
            //_tree.PauseHeal();
        }
    }

}
