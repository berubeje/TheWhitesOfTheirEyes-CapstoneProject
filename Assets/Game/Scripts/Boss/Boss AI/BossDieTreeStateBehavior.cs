using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDieTreeStateBehavior : StateMachineBehaviour
{
    private Animator _animator;
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator fsm, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_animator == null)
        {
            _animator = fsm.transform.parent.GetComponent<Animator>();
        }

        _animator.SetTrigger("Die");
    }



}
