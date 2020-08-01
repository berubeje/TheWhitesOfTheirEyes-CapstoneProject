﻿///-------------------------------------------------------------------------------------------------
// file: BossSweepStateBehavior.cs
//
// author: Jesse Berube
// date: 2020-07-13
//
// summary: The boss sweep attack state. Until the actual animations for the boss are in, the attack is a cylinder that sweeps across the stage. 
///-------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSweepStateBehavior : StateMachineBehaviour
{
    private Animator _animator;
    private BossController _bossController;
    private Transform _playerTransform;
    private JimController _player;

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

        // Get the relative position of the player compared to the boss.
        Vector3 relativePosition = _bossController.transform.InverseTransformPoint(_playerTransform.position);

        // This determines where the sweep attacks starts, and what animation to play.
        if (relativePosition.x > 0f)
        {
            _bossController.SweepAttack(false);
        }
        else
        {
            _bossController.SweepAttack(true);
        }

        if(_bossController.player.hook.currentRopeState == PlayerGrapplingHook.RopeState.Swing)
        {
            _animator.SetFloat("Attack Speed", _bossController.slowedAttackSpeed);
        }
        else
        {
            _animator.SetFloat("Attack Speed", _bossController.normalAttackSpeed);
        }
    }

    public override void OnStateUpdate(Animator fsm, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        // If boss health is 0 or less, go to die state.
        if (_bossController.currentBossHealth <= 0.0f)
        {
            fsm.SetTrigger("Fall");
            return;
        }

        if (_bossController.flinch)
        {
            fsm.SetTrigger("Flinch");
            return;
        }

        var state = _animator.GetCurrentAnimatorStateInfo(0);

        // Check to see if the attack animation is still playing. Leave state when it finishes.
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
}