using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingRock : MonoBehaviour
{
    public static float damage = 20;

    public TrapTrigger trapTrigger;
    public bool isPlayerKillable = false;
    private CapsuleCollider _playerCapsuleCollider;

    private JimController _jimController;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Player") && isPlayerKillable)
        {
            // Make the player take damage
            _jimController = collision.gameObject.GetComponent<JimController>();
            _jimController.currentHealth -= damage;
        }

        isPlayerKillable = false;
    }

}
