///-------------------------------------------------------------------------------------------------
// file: BossController.cs
//
// author: Jesse Berube
// date: 2020/07/13
//
// summary: The logic for controlling the rope, such as launching it and pulling with the rope, are here.
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

        //if(finalAdjustment)
        //{
        //    _t += Time.deltaTime / finalTurnAdjustmentTime;

        //    transform.rotation = Quaternion.Slerp(_startRotation, Quaternion.Euler(transform.position - currentMarkerTarget.position), _t);

        //    if (_t >= 1.0f)
        //    {
        //        finalAdjustment = false;
        //        _t = 0;
        //        _startRotation = transform.rotation;
        //    }
        //}
    }

    public void Turn(bool rightTurn)
    {
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

    public void SnapToWaypoint()
    {
        Vector3 lookVector = new Vector3(currentMarkerTarget.position.x, transform.position.y, currentMarkerTarget.position.z);

        transform.LookAt(lookVector);

        _startRotation = transform.rotation;
    }

    public bool NeedToTurn()
    {
        Transform finalTarget = currentMarkerTarget;

        foreach (Transform marker in markers)
        {
            if (Vector3.Distance(player.transform.position, marker.position) < Vector3.Distance(player.transform.position, finalTarget.position))
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
