///-------------------------------------------------------------------------------------------------
// file: BossController.cs
//
// author: Jesse Berube
// date: 2020/07/13
//
// summary: The boss AI calls certain methosd in he BossController under certain conditions. Used to store general paramenters like health and a reference to the player as well.
///-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public float maxHealth = 100.0f;
    public float currentBossHealth
    {
        get { return _bossHealth; }
        set
        {
            if (_bossHealth == value)
            {
                return;
            }
            _bossHealth = value;

            if (OnHealthChange != null && _bossHealth != 0)
            {
                OnHealthChange(_bossHealth);
            }
        }
    }
    public delegate void OnHealthChangeDelegate(float health);
    public event OnHealthChangeDelegate OnHealthChange;

    public List<Transform> markers = new List<Transform>();
    public Transform currentMarkerTarget;
    //public SweepAttackPlaceholderLogic sweepAttackPivot;


    public JimController player;
    public bool bossStart;
    public bool treeRepairInProgress = false;


    [Header("Turn Parameters")]
    public float turnTime = 2f;
    public float flinchTurnTime = 0.5f;
    public float turnAngle = 45f;

    [Header("Attack Parameters")]
    public float attackDamage = 15.0f;
    public float normalAttackSpeed = 1.3f;
    public float slowedAttackSpeed = 1.0f;
    public AttackColliderLogic leftArmAttackCollider;
    public AttackColliderLogic rightArmAttackCollider;


    [HideInInspector]
    public bool turning = false;

    public bool flinch = false;

    [HideInInspector]
    public List<RopeAnchorPoint> fallenTreeList = new List<RopeAnchorPoint>();

    private float _bossHealth;
    private Quaternion _startRotation;
    private Quaternion _targetRotation;

    private float _t = 0.0f;
    private Animator _animator;

    private void Awake()
    {
        currentBossHealth = maxHealth;

        OnHealthChange += OnHealthChanged;

    }

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
            if (flinch == false)
            {
                _t += Time.deltaTime / turnTime;
            }
            else
            {
                _t += Time.deltaTime / flinchTurnTime;
            }

            transform.rotation = Quaternion.Slerp(_startRotation, _targetRotation, _t);

            if (_t >= 1.0f)
            {
                turning = false;

                _startRotation = transform.rotation;
                _t = 0;

                if(flinch)
                {
                    SnapToWaypoint();
                }
            }
        }
    }

    public void HitStun()
    {
        _animator.SetTrigger("Front Hit Stun");
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

    public void Turn(bool rightTurn, float rotationAmount)
    {
        // Set the target rotation by 45 degrees, as well as play the appropriate animation.
        if (rightTurn)
        {
            _targetRotation = Quaternion.Euler(_startRotation.eulerAngles + new Vector3(0f, rotationAmount, 0f));
            _animator.SetTrigger("Turn Right");
        }
        else
        {
            _targetRotation = Quaternion.Euler(_startRotation.eulerAngles + new Vector3(0f, -rotationAmount, 0f));
            _animator.SetTrigger("Turn Left");

        }

        turning = true;
    }

    // Trigger the animation based on the bool passed in.
    public void SweepAttack(bool rightArmAttack)
    {
        //sweepAttackPivot.gameObject.SetActive(true);

        if (rightArmAttack)
        {
            _animator.SetTrigger("Right Swipe");
            rightArmAttackCollider.gameObject.SetActive(true);
            //sweepAttackPivot.StartAttack(true);
        }
        else
        {
            _animator.SetTrigger("Left Swipe");
            leftArmAttackCollider.gameObject.SetActive(true);
            //sweepAttackPivot.StartAttack(false);
        }
    }

    private void OnHealthChanged(float health)
    {
        //Healthbar stuff can be added here
        if(health != maxHealth && health > 0.0f)
        {
            flinch = true;
            turning = false;
            _t = 0.0f;
        }
    }

    // Snap the rotation to look at the currently targeted waypoint.
    public void SnapToWaypoint()
    {
        Vector3 lookVector = new Vector3(currentMarkerTarget.position.x, transform.position.y, currentMarkerTarget.position.z);

        transform.LookAt(lookVector);

        _startRotation = transform.rotation;
    }

    public void TurnToClosestWaypoint()
    {
        Transform result = null;
        float currentResultValue = 0.0f;

        foreach(Transform marker in markers)
        {
            float resultAngle = Vector3.Angle(transform.forward, marker.position - transform.position);

            if(result == null)
            {
                currentResultValue = resultAngle;
                result = marker;

                if(currentResultValue <= 22.5f)
                {
                    break;
                }
            }
            else
            {
                if(resultAngle < currentResultValue)
                {
                    currentResultValue = resultAngle;
                    result = marker;

                    if (currentResultValue <= 22.5f)
                    {
                        break;
                    }
                }
            }
        }

        Vector3 relativePosition = transform.InverseTransformPoint(result.position);

        if (relativePosition.x > 0f)
        {
            Turn(true, currentResultValue);
        }
        else
        {
            Turn(false, currentResultValue);
        }

        currentMarkerTarget = result;
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

    public void DisableLeftArmEvent()
    {
        leftArmAttackCollider.gameObject.SetActive(false);
    }

    public void DisableRightArmEvent()
    {
        rightArmAttackCollider.gameObject.SetActive(false);
    }
}
