using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDieStateBehaviour : StateMachineBehaviour
{
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        InputManager.Instance.currentGameState = InputManager.GameStates.GameFinished;
    }
}
