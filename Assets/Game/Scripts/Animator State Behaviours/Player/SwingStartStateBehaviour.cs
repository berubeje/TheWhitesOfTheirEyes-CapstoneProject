using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingStartStateBehaviour : StateMachineBehaviour
{
    public float swingRadius;
    public float faceAnchorSpeed;
    public float reelInSpeed;
    public float swingArcLimit;
    [Space]
    public float releaseMagnitude;
    public float releaseDirectionOffset;
    public float releaseDistanceX;
    public float releaseDestinationAngle;
    [Space]
    public float forwardCheckDistance;

    private JimController _jimController;
    private SplineRoute _splineRoute;
    private PlayerGrapplingHook _grapplingHook;
    private Transform _anchor;
    private Rigidbody _rigidbody;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private Vector3 _lookDirection;
    private Vector3 _reelDirection;
    private Vector3 _reelLocation;
    private Vector3 _swingForward;
    private Quaternion _lookRotation;
    private float _interpolant;
    private float _lerpRate;
    private int _layerMask = ~(1 << 8);

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        _jimController = animator.GetComponent<JimController>();
        _splineRoute = _jimController.splineRoute;
        
        _grapplingHook = animator.GetComponentInChildren<PlayerGrapplingHook>();

        if (_grapplingHook == null)
        {
            Debug.LogError("Unable to find PlayerGrapplingHook component in any children");
        }

        _anchor = _grapplingHook.targetAnchor.transform;

        _rigidbody = animator.GetComponent<Rigidbody>();

        if (_rigidbody == null)
        {
            Debug.LogError("Unable to find Rigidbody component");
        }

        _interpolant = 0.0f;

        _initialPosition = animator.transform.position;
        _initialRotation = animator.transform.rotation;

        // Grab the direction from the player to the anchor and change the y value to face downward at an angle
        _lookDirection = (_anchor.position - animator.transform.position);
        _lookDirection.y = Vector3.Cross(_lookDirection, -animator.transform.right).y;

        // Grab the direction from the anchor to the player and normalize it 
        _reelDirection = (animator.transform.position - _anchor.position).normalized;

        // Reverse it and kill the y to get the swing forward
        _swingForward = _swingForward - _reelDirection;
        _swingForward.y = 0;

        // Calculate the point to be reeled to
        _reelLocation = _anchor.position + (_reelDirection * swingRadius);

        // Direction player needs to rotate to
        _lookRotation = Quaternion.LookRotation(_lookDirection);

        _lerpRate = (reelInSpeed * Time.deltaTime) / Vector3.Distance(_reelLocation, _initialPosition);

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.DrawLine(_initialPosition, _reelLocation, Color.magenta);
        Debug.DrawRay(_initialPosition, _lookDirection, Color.red);
        if (!animator.GetAnimatorTransitionInfo(0).IsName("SwingStart -> SwingIdle"))
        {
            Vector3 targetPosition = Vector3.Lerp(_initialPosition, _reelLocation, _interpolant);
            Quaternion targetRotation = Quaternion.Lerp(_initialRotation, _lookRotation, _interpolant);

            _rigidbody.MovePosition(targetPosition);
            _rigidbody.MoveRotation(targetRotation);

            if (_interpolant >= 1.0f)
            {
                animator.SetTrigger("swingIdle");
            }

            _interpolant += _lerpRate;
        }


        if (Physics.SphereCast(animator.transform.position + new Vector3(0, 1, 0), 0.3f, _swingForward, out _, forwardCheckDistance, _layerMask))
        {
            //_rigidbody.MoveRotation(Quaternion.LookRotation(Vector3.Cross(Vector3.up, animator.transform.right)));
            animator.SetTrigger("swingCancel");
        }

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _splineRoute.controlPoints[0].position = animator.transform.position;
        _splineRoute.controlPoints[1].position = animator.transform.position + (-_reelDirection * releaseMagnitude) + (Vector3.up * releaseDirectionOffset);

        _lookDirection.y = 0.0f;
        _splineRoute.controlPoints[3].position = animator.transform.position + (_lookDirection * releaseDistanceX);

        _splineRoute.controlPoints[2].position = (Quaternion.AngleAxis(releaseDestinationAngle, animator.transform.right) * Vector3.up) +
            _splineRoute.controlPoints[3].position;
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
