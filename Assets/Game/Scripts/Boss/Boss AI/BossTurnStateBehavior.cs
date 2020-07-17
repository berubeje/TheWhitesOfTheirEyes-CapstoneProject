using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTurnStateBehavior : StateMachineBehaviour
{
    public float angleDeadzone = 5;

    private Animator _animator;
    private BossController _bossController;
    private Transform _playerTransform;

    private bool _animationStarted;
    private bool _checkProgress;
    private bool _finalAdjustment;


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

        SendTurn();
    }

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
        if (_bossController.bossHealth <= 0.0f)
        {
            fsm.SetTrigger("Die");
            return;
        }

        if (_finalAdjustment == false)
        {
            var state = _animator.GetCurrentAnimatorStateInfo(0);

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


                if (_bossController.fallenTreeList.Count > 0)
                {
                    _bossController.treeRepairInProgress = true;
                }


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
                    if (_bossController.NeedToTurn(_bossController.fallenTreeList[0].transform))
                    {
                        SendTurn();
                        return;
                    }
                }



                if (resultAngle > angleDeadzone)
                {
                    SendTurn();
                }
                else if (resultAngle <= angleDeadzone)
                {
                    _bossController.SnapToWaypoint();

                    if (_bossController.treeRepairInProgress == false)
                    {
                        fsm.SetTrigger("Idle");
                    }
                    else
                    {
                        fsm.SetTrigger("Fix Tree");
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
    }


}
