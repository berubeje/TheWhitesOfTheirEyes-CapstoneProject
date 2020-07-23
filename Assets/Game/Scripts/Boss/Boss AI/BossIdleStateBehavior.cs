///-------------------------------------------------------------------------------------------------
// file: BossIdleStateBehavior.cs
//
// author: Jesse Berube
// date: 2020-07-13
//
// summary: The bosses idle state does checks to decide which state to transition to.
///-------------------------------------------------------------------------------------------------

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
        // Check the boss health. If 0 or less, switch to "Die" state.
        if (_bossController.bossStart)
        {
            if(_bossController.bossHealth <= 0.0f)
            {
                fsm.SetTrigger("Die");
                return;
            }


            // Check to see if any trees have fallen over. If so, check to see if the boss needs to turn to it (turn state), otherwise, change to the "Fix Tree" state
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


            // Check if the boss needs to turn to face the player. If so, change to "Turn" state.
            if (_bossController.NeedToTurn(_playerTransform))
            {
                fsm.SetTrigger("Turn");
                return;

            }

            // If this bool is true, the boss will attack the player instantly if they try to pull down a pillar while the boss is facing them.
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


            // If the player is not pulling a pillar down, roll to attack after x seconds. Chance to attack goes up every time the roll fails.
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

    // Roll to attack. Chance to attack goes up everytime an attack roll fails. 
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
