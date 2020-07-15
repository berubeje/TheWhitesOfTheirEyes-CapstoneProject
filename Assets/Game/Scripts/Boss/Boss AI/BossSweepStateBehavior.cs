using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSweepStateBehavior : StateMachineBehaviour
{
    public bool startAttackCloseToPlayer = false;

    private Animator _animator;
    private BossController _bossController;
    private Transform _playerTransform;

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
            _playerTransform = _bossController.player.transform;
        }


        Vector3 relativePosition = _bossController.transform.InverseTransformPoint(_playerTransform.position);

        if (relativePosition.x > 0f)
        {
            if (startAttackCloseToPlayer)
            {
                _bossController.SweepAttack(true);
            }
            else
            {
                _bossController.SweepAttack(false);
            }
        }
        else
        {
            if (startAttackCloseToPlayer)
            {
                _bossController.SweepAttack(false);
            }
            else
            {
                _bossController.SweepAttack(true);
            }
        }
    }

    public override void OnStateUpdate(Animator fsm, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        var state = _animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("Right Swipe") || state.IsName("Left Swipe"))
        {
            _animationStarted = true;
        }
        else if (_animationStarted == true)
        {
            _animationStarted = false;

            fsm.SetTrigger("Idle");
        }
    }


    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator fsm, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
