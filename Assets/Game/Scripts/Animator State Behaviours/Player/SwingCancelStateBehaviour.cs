using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingCancelStateBehaviour : StateMachineBehaviour
{
    public float groundCheckDistance;

    private PlayerGrapplingHook _playerGrapplingHook;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _playerGrapplingHook = animator.GetComponent<JimController>().hook;
        _playerGrapplingHook.DetachHook();
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Physics.Raycast(animator.transform.position, Vector3.down, out _, groundCheckDistance))
        {
            animator.SetTrigger("fallLand");
        }
    }

}
