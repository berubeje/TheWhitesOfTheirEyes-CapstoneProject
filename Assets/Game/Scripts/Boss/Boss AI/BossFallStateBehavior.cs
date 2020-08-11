///-------------------------------------------------------------------------------------------------
// file: BossFallStateBehavior.cs
//
// author: Jesse Berube
// date: 2020-07-13
//
// summary: The fall state for the boss. The boss falls over, allowing the player to pull out the core and end the game
///-------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFallStateBehavior : StateMachineBehaviour
{
    public float fallPositionOffset = 5.0f;
    public float positionLerpTime = 3.0f;

    private Animator _animator;
    private BossController _bossController;
    private bool _animationStarted;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;

    private float _currentTime = 0.0f;

    override public void OnStateEnter(Animator fsm, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_animator == null)
        {
            _animator = fsm.transform.parent.GetComponent<Animator>();
        }

        if (_bossController == null)
        {
            _bossController = fsm.GetComponentInParent<BossController>();
        }

        _animator.SetBool("Fall", true);
        _bossController.TurnToClosestWaypoint(_bossController.fallMarkers);

        _startPosition = _bossController.transform.position;

        _targetPosition = _startPosition + _bossController.transform.forward * fallPositionOffset;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (positionLerpTime > 0.0f && _currentTime < 1.0f)
        {
            _currentTime += Time.deltaTime / positionLerpTime;


            _bossController.transform.position = Vector3.Lerp(_startPosition, _targetPosition, _currentTime);

        }
    }



}
