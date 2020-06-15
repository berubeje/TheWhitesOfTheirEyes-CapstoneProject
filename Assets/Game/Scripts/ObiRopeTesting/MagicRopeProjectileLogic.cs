using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicRopeProjectileLogic : MonoBehaviour
{
    public float projectileSpeed;
    public Transform ropeBaseReturnTransform;

    private PlayerGrapplingHook _grapplingHookLogic;
    private Transform _targetTransform;
    private GameObject _targetGameObject;
    private bool _targetReached = true;
    private Rigidbody _rigidBody;
    private bool _returning;



    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        if (_targetReached == false)
        {
            float step = projectileSpeed * Time.deltaTime;


            transform.position = Vector3.MoveTowards(transform.position, _targetTransform.position, step);

            if (Vector3.Distance(transform.position, _targetTransform.position) < 0.001f)
            {
                if (!_returning)
                {
                    _grapplingHookLogic.TargetReached();
                    _targetReached = true;
                    transform.parent = _targetGameObject.transform;
                }
                else
                {
                    _grapplingHookLogic.RopeReturned();

                    if(_targetTransform == ropeBaseReturnTransform)
                    {
                        _returning = false;
                        _targetReached = true;
                    }
                }
            }

        }
    }

    public void RopeReturn(Transform ropeBase)
    {
        _targetTransform = ropeBase;
        _targetReached = false;
        _returning = true;
    }

    public void RopeReturn()
    {
        _targetTransform = ropeBaseReturnTransform;
        _targetReached = false;
        _returning = true;
    }

    public void SetupProjectile(Transform targetTransform, PlayerGrapplingHook grappling, GameObject target)
    {
        _targetTransform = targetTransform;
        _grapplingHookLogic = grappling;
        _targetGameObject = target;
        _targetReached = false;
    }

    public void SetupGrappleHook(PlayerGrapplingHook grappling)
    {
        _grapplingHookLogic = grappling;
    }
}
