///-------------------------------------------------------------------------------------------------
// file: BossFallStateBehavior.cs
//
// author: Jesse Berube
// date: 2020-07-13
//
// summary: The fall state for the boss. The boss falls over, allowing the player to pull out the core and end the game
///-------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFallStateBehavior : StateMachineBehaviour
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

        _animator.SetBool("Fall", true);
        fsm.SetBool("Fall", false);
        _bossController.TurnToClosestWaypoint(_bossController.fallMarkers);
    }



}
