using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdleStateBehavior : StateMachineBehaviour
{
    public float secondsTillAttackCheck = 2.0f;
    public float startingAttackChance = 30.0f;
    public float attackChanceIncreaseRate = 20.0f;


    private float _currentAttackChance;
    private float _currentTickTime;

    private Animator _animator;
    private BossController _bossController;
    private Transform _playerTransform;



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
        }
    }

    override public void OnStateUpdate(Animator fsm, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (_bossController.bossStart)
        {
            //Check if tree needs lifting. Tree repair is number one priority.

            if (_bossController.NeedToTurn())
            {
                fsm.SetTrigger("Turn");
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
