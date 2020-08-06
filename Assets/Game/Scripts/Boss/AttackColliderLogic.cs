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
        Animator animator = collision.gameObject.GetComponent<Animator>();

        if(jimController != null && animator != null)
        {
            if (!jimController.IsInState(jimController._rollID))
            {
                // Calculate which direction the player was facing when they were hit
                Debug.Log(Vector3.Angle(jimController.transform.forward, bossTransform.forward));
                Debug.Log(Vector3.Cross(jimController.transform.forward, bossTransform.forward));
                
                //animator.SetTrigger("hit");
                //Debug.Log(hitCounter);
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
