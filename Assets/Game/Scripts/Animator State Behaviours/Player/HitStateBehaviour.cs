using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStateBehaviour : StateMachineBehaviour
{
    private Quaternion _targetRotation;
    private JimController _jimController;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // disable controls and let the hit animation play through completely 
        InputManager.Instance.DisableAllControls();

        _jimController = animator.GetComponent<JimController>();

        // Calculate rotation direction to ensure player remains upright
        _targetRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, -animator.transform.right).normalized);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.rotation = Quaternion.RotateTowards(animator.transform.rotation, _targetRotation, _jimController.rotationSpeed);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        InputManager.Instance.EnableAllControls();
    }
}
