using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingCancelStateBehaviour : StateMachineBehaviour
{
    public float groundCheckDistance;

    private Rigidbody _rigidbody;
    private JimController _jimController;
    private PlayerGrapplingHook _playerGrapplingHook;
    private Vector3 _swingForward;
    private Vector3 _rotationRight;
    private Quaternion _targetRotation;
    private int _direction;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _rigidbody = animator.GetComponent<Rigidbody>();
        _jimController = animator.GetComponent<JimController>();
        _playerGrapplingHook = _jimController.hook;
        _playerGrapplingHook.DetachHook();

        _swingForward = -animator.transform.forward;
        _swingForward.y = 0;

        _direction = (int)animator.GetFloat("swingDirectionRaw");

        _targetRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, animator.transform.right * _direction));

        // Can now roll since we're jumping off a wall
        animator.SetBool("canRoll", true);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.rotation = Quaternion.RotateTowards(animator.transform.rotation, _targetRotation, _jimController.rotationSpeed);

        if (Physics.Raycast(animator.transform.position, Vector3.down, out _, groundCheckDistance))
        {
            animator.SetTrigger("fallLand");
        }
    }
}
