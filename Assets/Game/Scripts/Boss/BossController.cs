﻿///-------------------------------------------------------------------------------------------------
// file: BossController.cs
//
// author: Jesse Berube
// date: 2020/07/13
//
// summary: The boss AI calls certain methosd in he BossController under certain conditions. Used to store general paramenters like health and a reference to the player as well.
///-------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public float bossHealth = 100.0f;
    public List<Transform> markers = new List<Transform>();
    public Transform currentMarkerTarget;
    public SweepAttackPlaceholderLogic sweepAttackPivot;


    public JimController player;
    public bool bossStart;
    public bool treeRepairInProgress = false;


    [Header("Turn Parameters")]
    public float turnTime = 2f;
    public float turnAngle = 45f;


    [HideInInspector]
    public bool turning = false;

    [HideInInspector]
    public List<RopeAnchorPoint> fallenTreeList = new List<RopeAnchorPoint>();

    private Quaternion _startRotation;
    private Quaternion _targetRotation;

    private float _t = 0.0f;
    private Animator _animator;



    private void Start()
    {
        _startRotation = transform.rotation;
        _animator = GetComponent<Animator>();

        if (markers.Count > 0)
        {
            currentMarkerTarget = markers[0];
        }
        else
        {
            Debug.LogError("Boss needs look markers in the Boss Controller script to function properly.");
        }

    }

    private void Update()
    {

        // If turning is true, turn the boss on a Quaternion Slerp.
        if (turning)
        {
            _t += Time.deltaTime / turnTime;

            transform.rotation = Quaternion.Slerp(_startRotation, _targetRotation, _t);

            if (_t >= 1.0f)
            {
                turning = false;

                _startRotation = transform.rotation;
                _t = 0;
            }
        }
    }

    public void Turn(bool rightTurn)
    {
        // Set the target rotation by 45 degrees, as well as play the appropriate animation.
        if(rightTurn)
        {
             _targetRotation = Quaternion.Euler(_startRotation.eulerAngles + new Vector3(0f, 45f, 0f));
            _animator.SetTrigger("Turn Right");
        }
        else
        {
             _targetRotation = Quaternion.Euler(_startRotation.eulerAngles + new Vector3(0f, -45f, 0f));
            _animator.SetTrigger("Turn Left");

        }

        turning = true;
    }


    // Trigger the animation based on the bool passed in.
    public void SweepAttack(bool rightArmAttack)
    {
        sweepAttackPivot.gameObject.SetActive(true);

        if (rightArmAttack)
        {
            _animator.SetTrigger("Right Swipe");
            sweepAttackPivot.StartAttack(true);
        }
        else
        {
            _animator.SetTrigger("Left Swipe");
            sweepAttackPivot.StartAttack(false);
        }
    }

    // Snap the rotation to look at the currently targeted waypoint.
    public void SnapToWaypoint()
    {
        Vector3 lookVector = new Vector3(currentMarkerTarget.position.x, transform.position.y, currentMarkerTarget.position.z);

        transform.LookAt(lookVector);

        _startRotation = transform.rotation;
    }

    // Get the closest waypoint to the passed in target.
    public bool NeedToTurn(Transform target)
    {
        Transform finalTarget = currentMarkerTarget;

        foreach (Transform marker in markers)
        {
            if (Vector3.Distance(target.position, marker.position) < Vector3.Distance(target.position, finalTarget.position))
            {
                finalTarget = marker.transform;
            }
        }

        if (finalTarget == currentMarkerTarget)
        {
            return false;
        }
        else
        {
            currentMarkerTarget = finalTarget;
            return true;
        }
    }
}