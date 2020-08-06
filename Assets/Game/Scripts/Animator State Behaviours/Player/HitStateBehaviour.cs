using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStateBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // disable controls and let the hit animation play through completely 
        InputManager.Instance.DisableAllControls();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        InputManager.Instance.EnableAllControls();
    }
}
