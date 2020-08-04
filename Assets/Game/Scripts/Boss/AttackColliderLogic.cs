using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackColliderLogic : MonoBehaviour
{
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

        if(jimController != null)
        {
            Debug.Log(hitCounter);
            hitCounter++;
            jimController.currentHealth -= _boss.attackDamage;
            gameObject.SetActive(false);
        }
    }
}
