///-------------------------------------------------------------------------------------------------
// file: BossTurnStateBehavior.cs
//
// author: Jesse Berube
// date: 2020/07/13
//
// summary: The turn state behavior for the boss. The boss can stay in this state to do muiltiple turns.
///-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTurnStateBehavior : StateMachineBehaviour
{
    public float angleDeadzone = 5;
    public bool treeRepairOn = false;

    private Animator _animator;
    private Animator _fsm;
    private BossController _bossController;
    private Transform _playerTransform;
    private PlayerGrapplingHook _hook;

    private bool _animationStarted;
    private bool _checkProgress;
    private bool _finalAdjustment;


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
            _playerTransform = _bossController.player.transform;
            _hook = _bossController.player.hook;
        }

        _bossController.flinchEvent.AddListener(Flinch);
        SendTurn();
    }

    // Get the the position of the waypoint the boss is targeting to determine the best direction to turn.
    private void SendTurn()
    {
        Vector3 relativePosition = _bossController.transform.InverseTransformPoint(_bossController.currentMarkerTarget.transform.position);

        if (relativePosition.x > 0f)
        {
            _bossController.Turn(true);
        }
        else
        {
            _bossController.Turn(false);
        }   
    }

    public override void OnStateUpdate(Animator fsm, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (_bossController.flinch)
        //{
        //    fsm.SetTrigger("Flinch");
        //    _bossController.TurnToClosestWaypoint(_bossController.markers);
        //    return;
        //}

        if (_finalAdjustment == false)
        {
            var state = _animator.GetCurrentAnimatorStateInfo(0);

            // Check if turn animation is finished. If so, check to see if more turning is needed
            if (state.IsName("RightTurn") || state.IsName("LeftTurn"))
            {
                _animationStarted = true;
            }
            else if (_animationStarted == true)
            {
                _animationStarted = false;
                _checkProgress = true;
            }

            if (_checkProgress)
            {
                _checkProgress = false;
                float resultAngle = Vector3.Angle(_bossController.transform.forward, _bossController.currentMarkerTarget.position - _bossController.transform.position);


                // Check if there are any fallen trees.
                if (_bossController.fallenTreeList.Count > 0 && treeRepairOn)
                {
                    _bossController.treeRepairInProgress = true;
                }

                // If there areno fallen trees, check if the boss needs to turn again to face the player.
                if (_bossController.treeRepairInProgress == false)
                {
                    if (_bossController.NeedToTurn(_playerTransform))
                    {
                        SendTurn();
                        return;
                    }
                }
                else
                {
                    //If there are fallen trees, check if the boss need to turn to face the tree
                    if (_bossController.NeedToTurn(_bossController.fallenTreeList[0].transform))
                    {
                        SendTurn();
                        return;
                    }
                }

                //Check if the boss has reached its target angle yet. If not, turn again.
                if (resultAngle > angleDeadzone)
                {
                    SendTurn();
                }
                else if (resultAngle <= angleDeadzone)
                {
                    //If target angle reached, snap the boss rotation to face directly at the waypoint to keep its rotation consistant.
                    _bossController.SnapToWaypoint();

                    if (_bossController.treeRepairInProgress == false)
                    {
                        fsm.SetTrigger("Idle");
                    }
                    else
                    {
                        if (_hook.targetAnchor == null ||  _hook.targetAnchor.transform.root != _bossController.fallenTreeList[0].transform.root)
                        {
                            fsm.SetTrigger("Fix Tree");
                        }
                        else
                        {
                            fsm.SetTrigger("Idle");
                        }
                    }
                }
            }
        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator fsm, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _finalAdjustment = false;
        _checkProgress = false;
        _animationStarted = false;
        _bossController.flinchEvent.RemoveListener(Flinch);

    }

    private void Flinch()
    {
        _fsm.SetTrigger("Flinch");
    }
}
