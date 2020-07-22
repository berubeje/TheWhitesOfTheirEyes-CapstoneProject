using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdleStateBehavior : StateMachineBehaviour
{
    public float secondsTillAttackCheck = 2.0f;
    public float startingAttackChance = 30.0f;
    public float attackChanceIncreaseRate = 20.0f;
    public bool attackWhenPillarDisturbed = true;


    private float _currentAttackChance;
    private float _currentTickTime;

    private Animator _animator;
    private BossController _bossController;
    private Transform _playerTransform;
    private PlayerGrapplingHook _grapplingHook;



    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator fsm, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_animator == null)
        {
            _animator = fsm.transform.parent.GetComponent<Animator>();
        }

        if(_bossController == null)
        {
            _bossController = fsm.GetComponentInParent<BossController>();
            _playerTransform = _bossController.player.transform;
            _currentAttackChance = startingAttackChance;
            _grapplingHook = _playerTransform.GetComponentInChildren<PlayerGrapplingHook>();
        }
    }

    override public void OnStateUpdate(Animator fsm, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (_bossController.bossStart)
        {
            if(_bossController.bossHealth <= 0.0f)
            {
                fsm.SetTrigger("Die");
                return;
            }


            if(_bossController.fallenTreeList.Count > 0)
            {
                _bossController.treeRepairInProgress = true;
                if (_bossController.NeedToTurn(_bossController.fallenTreeList[0].transform))
                {
                    fsm.SetTrigger("Turn");
                }
                else
                {
                    fsm.SetTrigger("Fix Tree");
                }
                return;
            }
            else
            {
                _bossController.treeRepairInProgress = false;
            }

            if (_bossController.NeedToTurn(_playerTransform))
            {
                fsm.SetTrigger("Turn");
                return;

            }

            if (attackWhenPillarDisturbed)
            {
                RopeAnchorPoint currentPlayerAnchorTarget = _grapplingHook.targetAnchor;

                if (currentPlayerAnchorTarget != null)
                {
                    if (currentPlayerAnchorTarget.transform.GetComponentInParent<KillPillarScript>())
                    {
                        _currentTickTime = 0.0f;
                        fsm.SetTrigger("Sweep Attack");
                        return;
                    }
                }
            }

            _currentTickTime += Time.deltaTime;

            if (_currentTickTime >= secondsTillAttackCheck)
            {
                _currentTickTime = 0.0f;

                if (RollToAttack())
                {
                    fsm.SetTrigger("Sweep Attack");
                }
            }
        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator fsm, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    private bool RollToAttack()
    {
        float result = Random.Range(0.0f, 100.0f);

        if(result <= _currentAttackChance)
        {
            _currentAttackChance = startingAttackChance;
            return true;
        }
        else
        {
            _currentAttackChance += attackChanceIncreaseRate;
            return false;
        }
    }
}
