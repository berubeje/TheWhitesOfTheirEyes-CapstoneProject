using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicRopeProjectileLogic : MonoBehaviour
{
    public float projectileSpeed;

    private PlayerGrapplingHook _grapplingHookLogic;
    private Transform _targetTransform;
    private GameObject _targetGameObject;
    private bool _targetReached = false;
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
                    Destroy(this.gameObject);
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

    public void SetupProjectile(Transform targetTransform, PlayerGrapplingHook grappling, GameObject target)
    {
        _targetTransform = targetTransform;
        _grapplingHookLogic = grappling;
        _targetGameObject = target;
    }
}
