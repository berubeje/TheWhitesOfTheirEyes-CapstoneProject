using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicRopeProjectileLogic : MonoBehaviour
{
    public float projectileSpeed;

    private PlayerGrapplingHook _grapplingHookLogic;
    private Vector3 _targetPosition;
    private GameObject _targetGameObject;
    private bool _targetReached = false;
    private Rigidbody _rigidBody;



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
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, step);

            if (Vector3.Distance(transform.position, _targetPosition) < 0.001f)
            {
                _grapplingHookLogic.TargetReached();
                _targetReached = true;

                //if (_targetGameObject.GetComponent<BoxScript>() != null)
                //{
                // rigidBody.isKinematic = false;
                transform.parent = _targetGameObject.transform;

                //}

            }
        }
    }

    public void SetupProjectile(Vector3 position, PlayerGrapplingHook grappling, GameObject target)
    {
        _targetPosition = position;
        _grapplingHookLogic = grappling;
        _targetGameObject = target;
    }
}
