using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackColliderLogic : MonoBehaviour
{
    public Transform bossTransform;

    private BossController _boss;
    private int hitCounter = 0;

    private void Awake()
    {
        _boss = GetComponentInParent<BossController>();
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        JimController jimController = collision.gameObject.GetComponent<JimController>();
        Animator jimAnimator = collision.gameObject.GetComponent<Animator>();

        if(jimController != null && jimAnimator != null)
        {
            if (!jimController.IsInState(jimController._rollID))
            {
                //Vector3 contactPoint = collision.GetContact(0).point;

                // Get the direction of the collision
                Vector3 collisionVector = transform.position - jimController.transform.position;
                collisionVector.y = 0;

                // The sign of the y value of the cross product is the direction the player was hit
                float hitDirection = Vector3.Cross(jimController.transform.forward, collisionVector).y;

                // Get the angle at which the player was hit
                float hitAngle = Vector3.Angle(jimController.transform.forward, collisionVector);

                if (hitAngle <= 45)
                {
                    jimAnimator.SetFloat("hitDirectionX", 0);
                    jimAnimator.SetFloat("hitDirectionY", 1);
                }
                else if (hitAngle > 45 && hitAngle <= 135)
                {
                    if(hitDirection >= 0)
                    {
                        jimAnimator.SetFloat("hitDirectionX", 1);
                        jimAnimator.SetFloat("hitDirectionY", 0);
                    }
                    else
                    {
                        jimAnimator.SetFloat("hitDirectionX", -1);
                        jimAnimator.SetFloat("hitDirectionY", 0);
                    }
                    
                }
                else if(hitAngle > 135)
                {
                    jimAnimator.SetFloat("hitDirectionX", 0);
                    jimAnimator.SetFloat("hitDirectionY", -1);
                }

                jimAnimator.SetTrigger("hit");

                // Detach hook
                if (jimController.ropeLogic.currentRopeState == PlayerGrapplingHook.RopeState.Pull || jimController.ropeLogic.currentRopeState == PlayerGrapplingHook.RopeState.Swing)
                {
                    if (jimController.isPulling == false)
                    {
                        jimController.ropeLogic.DetachHook();
                    }
                }
                else if (jimController.ropeLogic.currentRopeState == PlayerGrapplingHook.RopeState.Launched)
                {
                    jimController.ropeLogic.CancelLaunch();
                }

                hitCounter++;
                jimController.currentHealth -= _boss.attackDamage;
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
