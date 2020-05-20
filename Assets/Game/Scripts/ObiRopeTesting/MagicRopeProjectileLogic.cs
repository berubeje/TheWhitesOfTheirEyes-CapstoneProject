using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicRopeProjectileLogic : MonoBehaviour
{
    public float projectileSpeed;
    public GrapplingHookType1 grapplingHookLogic1;
    public GrapplingHookType2 grapplingHookLogic2;

    private Vector3 targetPosition;
    private GameObject targetGameObject;
    private bool targetReached = false;
    private Rigidbody rigidBody;



    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        if (targetReached == false)
        {
            float step = projectileSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                if (grapplingHookLogic1 != null)
                {
                    grapplingHookLogic1.TargetReached();
                    targetReached = true;
                    
                    if(targetGameObject.GetComponent<BoxScript>() != null)
                    {
                       // rigidBody.isKinematic = false;
                        transform.parent = targetGameObject.transform;
                    }

                }
                else
                {
                    grapplingHookLogic2.TargetReached();
                    targetReached = true;

                    if (targetGameObject.GetComponent<BoxScript>() != null)
                    {
                       // rigidBody.isKinematic = false;
                        transform.parent = targetGameObject.transform;

                    }
                }
            }
        }
    }

    public void SetupProjectile(Vector3 position, GrapplingHookType1 grappling, GameObject target)
    {
        targetPosition = position;
        grapplingHookLogic1 = grappling;
        targetGameObject = target;
    }

    public void SetupProjectile(Vector3 position, GrapplingHookType2 grappling, GameObject target)
    {
        targetPosition = position;
        grapplingHookLogic2 = grappling;
        targetGameObject = target;

    }
}
