using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallIdleStateBehaviour : StateMachineBehaviour
{
    [Range(0.0f, 2.0f)]
    public float splineSpeed;

    private SplineRoute _splineRoute;
    private Rigidbody _rigidbody;
    private float _t;

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

        _splineRoute = animator.GetComponent<JimController>().splineRoute;

        if(_splineRoute == null)
        {
            Debug.LogError("Unable to find Spline Route object");
        }


        _t = 0.0f;
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.GetAnimatorTransitionInfo(0).IsName("SwingIdle -> FallIdle"))
        {
            _p0 = _splineRoute.controlPoints[0].position;
            _p1 = _splineRoute.controlPoints[1].position;
            _p2 = _splineRoute.controlPoints[2].position;
            _p3 = _splineRoute.controlPoints[3].position;

            _t += splineSpeed * Time.deltaTime;

            if (_t >= 1)
            {
                animator.SetTrigger("fallLand");
            }

            Vector3 target = Mathf.Pow(1 - _t, 3) * _p0 +
                 3 * Mathf.Pow(1 - _t, 2) * _t * _p1 +
                 3 * (1 - _t) * Mathf.Pow(_t, 2) * _p2 +
                 Mathf.Pow(_t, 3) * _p3;

            animator.transform.Translate(target - animator.transform.position, Space.World);
        }
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

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
