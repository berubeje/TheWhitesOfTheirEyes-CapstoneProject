using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingLandStateBehaviour : StateMachineBehaviour
{
    [Range(0.0f, 10.0f)]
    public float splineSpeed;
    public float splineAcceleration;
    public float forwardCheckDistance;
    public float groundCheckDistance;

    private JimController _jimController;
    private SplineRoute _splineRoute;
    private Rigidbody _rigidbody;

    private Vector3 _swingForward;
    private Quaternion _targetRotation;
    private bool _splineComplete;
    private float _initialSplineSpeed;
    private float _t;
    private int _layerMask = ~(1 << 8);

    private Vector3 _p0;
    private Vector3 _p1;
    private Vector3 _p2;
    private Vector3 _p3;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _rigidbody = animator.GetComponent<Rigidbody>();

        if (_rigidbody == null)
        {
            Debug.LogError("Unable to find Rigidbody component");
        }

        _jimController = animator.GetComponent<JimController>();
        _splineRoute = _jimController.splineRoute;

        if(_splineRoute == null)
        {
            Debug.LogError("Unable to find Spline Route object");
        }

        // Cache the spline speed so we can reset it later
        _initialSplineSpeed = splineSpeed;
        _targetRotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, -animator.transform.right));
        _splineComplete = false;
        _t = 0.0f;


        _swingForward = Vector3.Cross(Vector3.up, -animator.transform.right).normalized;
        _targetRotation = Quaternion.LookRotation(_swingForward);

        // Cache spline point positions for readability
        _p0 = _splineRoute.controlPoints[0].position;
        _p1 = _splineRoute.controlPoints[1].position;
        _p2 = _splineRoute.controlPoints[2].position;
        _p3 = _splineRoute.controlPoints[3].position;
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        if (!animator.GetAnimatorTransitionInfo(0).IsName("SwingLand -> SwingCancel") && !animator.GetAnimatorTransitionInfo(0).IsName("SwingLand -> FallIdle"))
        {

            if (!_splineComplete)
            {

                splineSpeed += splineAcceleration;
                _t += splineSpeed * Time.deltaTime;

                if (_t >= 1)
                {
                    animator.SetTrigger("fallIdle");
                    _splineComplete = true;
                }

                Vector3 targetPosition = Mathf.Pow(1 - _t, 3) * _p0 +
                        3 * Mathf.Pow(1 - _t, 2) * _t * _p1 +
                        3 * (1 - _t) * Mathf.Pow(_t, 2) * _p2 +
                        Mathf.Pow(_t, 3) * _p3;

                _rigidbody.MovePosition(targetPosition);
                animator.transform.rotation = Quaternion.RotateTowards(animator.transform.rotation, _targetRotation, _jimController.rotationSpeed);
            }
        }
        if (Physics.Raycast(animator.transform.position, Vector3.down, out _, groundCheckDistance))
        {
            splineSpeed = _initialSplineSpeed;
            animator.SetTrigger("fallLand");
        }

        if (Physics.SphereCast(animator.transform.position, 0.3f, _swingForward * animator.GetFloat("swingDirectionRaw"), out _, forwardCheckDistance, _layerMask))
        {
            splineSpeed = _initialSplineSpeed;
            animator.SetTrigger("swingCancel");
        }
        Debug.DrawRay(animator.transform.position, _swingForward, Color.yellow);
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        splineSpeed = _initialSplineSpeed;

        animator.ResetTrigger("swingStart");
        animator.ResetTrigger("swingIdle");
        animator.ResetTrigger("swingLand");
        animator.ResetTrigger("swingCancel");
        animator.ResetTrigger("fallLand");
        animator.ResetTrigger("dodgeRoll");
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
