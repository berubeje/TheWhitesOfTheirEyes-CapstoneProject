using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionStateBehaviour : StateMachineBehaviour
{
    public float rotationSpeed;
    private Transform playerTransform;

    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerTransform = animator.gameObject.GetComponent<Transform>();    
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 leftStickDirection = new Vector3(animator.GetFloat("leftInputX"), 0.0f, animator.GetFloat("leftInputY"));
        if(leftStickDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(leftStickDirection);
            playerTransform.rotation = Quaternion.RotateTowards(playerTransform.rotation, targetRotation, rotationSpeed);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
