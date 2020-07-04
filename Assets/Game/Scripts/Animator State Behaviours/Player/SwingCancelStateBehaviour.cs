using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingCancelStateBehaviour : StateMachineBehaviour
{
    public float groundCheckDistance;

    private JimController _jimController;
    private PlayerGrapplingHook _playerGrapplingHook;
    private Vector3 _swingForward;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _jimController = animator.GetComponent<JimController>();
        _playerGrapplingHook = _jimController.hook;
        _playerGrapplingHook.DetachHook();

        _swingForward = -animator.transform.forward;
        _swingForward.y = 0;

        // Default the direction to forward since we'll be jumping off the wall
        animator.SetBool("canRoll", true);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Physics.Raycast(animator.transform.position, Vector3.down, out _, groundCheckDistance))
        {
            animator.SetTrigger("fallLand");
        }
    }
}
