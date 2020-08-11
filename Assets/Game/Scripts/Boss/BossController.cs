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
using UnityEngine.Events;

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

            if (OnHealthChange != null)
            {
                OnHealthChange(_bossHealth);
            }
        }
    }
    public delegate void OnHealthChangeDelegate(float health);
    public event OnHealthChangeDelegate OnHealthChange;

    public UnityEvent flinchEvent = new UnityEvent();

    public BossCoreLogic bossCore;
    public List<Transform> markers = new List<Transform>();
    public List<Transform> fallMarkers = new List<Transform>();

    public Transform currentMarkerTarget;
    //public SweepAttackPlaceholderLogic sweepAttackPivot;

    [Header("Eye light settings")]
    public Light leftEyeLight;
    public Light rightEyeLight;
    public float lightIntensityRange;
    public float lightNoiseRange; 
    public float lightLerpSpeed;

    [Space]
    public JimController player;
    public bool bossStart;
    public bool treeRepairInProgress = false;
    public EventCutscene deathCutscene;

    [HideInInspector]
    public bool bossCutsceneFinished;

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

    [HideInInspector]
    public bool flinch = false;

    [HideInInspector]
    public List<RopeAnchorPoint> fallenTreeList = new List<RopeAnchorPoint>();

    public float _bossHealth;
    private Quaternion _startRotation;
    private Quaternion _targetRotation;

    private float _t = 0.0f;
    private Animator _animator;
    private Animator _fsm;
    private RopeAnchorPoint _bossCoreAnchorPoint;

    private float _minLightInstensity;
    private float _maxLightInstensity;
    private void Awake()
    {
        _minLightInstensity = leftEyeLight.intensity;
        _maxLightInstensity = leftEyeLight.intensity + lightIntensityRange;

        currentBossHealth = maxHealth;

        OnHealthChange += OnHealthChanged;

        if (bossCore != null)
        {
            _bossCoreAnchorPoint = bossCore.GetComponent<RopeAnchorPoint>();
            _bossCoreAnchorPoint.canAttach = false;
        }

        Animator[] animators = gameObject.GetComponentsInChildren<Animator>();

        foreach (Animator animator in animators)
        {
            if(animator.gameObject == this.gameObject)
            {
                _animator = animator;
            }
            else
            {
                _fsm = animator;
            }
        }
    }

    private void Start()
    {
        _startRotation = transform.rotation;

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
        LerpLightIntensity();

        if (turning)
        {
            UpdateTurn();
        }
    }


    // If turning is true, turn the boss on a Quaternion Slerp.
    private void UpdateTurn()
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

            if (flinch)
            {
                SnapToWaypoint();
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
        if (rightTurn)
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
        if (rightArmAttack)
        {
            _animator.SetTrigger("Right Swipe");
            rightArmAttackCollider.gameObject.SetActive(true);
        }
        else
        {
            _animator.SetTrigger("Left Swipe");
            leftArmAttackCollider.gameObject.SetActive(true);
        }
    }

    private void OnHealthChanged(float health)
    {
        UICanvas.Instance.ChangeBossHealthBar(health / maxHealth);

        if (health != maxHealth && health > 0.0f)
        {
            flinch = true;
            turning = false;
            _t = 0.0f;
            AudioManager.Instance.PlaySound("BossHurt");
            flinchEvent.Invoke();

        }
        else if(health <= 0.0f)
        {
            _fsm.SetBool("Fall", true);
            deathCutscene.ActivateCutscene();
        }
    }

    // Snap the rotation to look at the currently targeted waypoint.
    public void SnapToWaypoint()
    {
        Vector3 lookVector = new Vector3(currentMarkerTarget.position.x, transform.position.y, currentMarkerTarget.position.z);

        transform.LookAt(lookVector);

        _startRotation = transform.rotation;
    }

    public void TurnToClosestWaypoint(List<Transform> waypoints)
    {
        Transform result = null;
        float currentResultValue = 0.0f;

        foreach (Transform marker in waypoints)
        {
            float resultAngle = Vector3.Angle(transform.forward, marker.position - transform.position);

            if (result == null)
            {
                currentResultValue = resultAngle;
                result = marker;
            }
            else
            {
                if (resultAngle < currentResultValue)
                {
                    currentResultValue = resultAngle;
                    result = marker;
                }
            }
        }

        _startRotation = transform.rotation;
        currentMarkerTarget = result;

        Vector3 direction = currentMarkerTarget.position - transform.position;

        _targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        _targetRotation.x = 0;
        _targetRotation.z = 0;
        turning = true;
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

    public void EnableCoreAnchorPointEvent()
    {
        _bossCoreAnchorPoint.canAttach = true;
    }

    private void LerpLightIntensity()
    {
        float newIntensity = Mathf.Lerp(_minLightInstensity, _maxLightInstensity, Mathf.PingPong(Time.time * lightLerpSpeed, 1));

        leftEyeLight.intensity = newIntensity + UnityEngine.Random.Range(-lightNoiseRange, lightNoiseRange);
        rightEyeLight.intensity = newIntensity + UnityEngine.Random.Range(-lightNoiseRange, lightNoiseRange);
    }
}
