using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingRock : MonoBehaviour
{
    public TrapTrigger trapTrigger;
    public bool isPlayerKillable = false;
    public float colliderVerticalOffset;

    private CapsuleCollider _playerCapsuleCollider;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Player") && isPlayerKillable)
        {
            _playerCapsuleCollider = collision.collider as CapsuleCollider;
            collision.gameObject.GetComponent<Animator>().SetBool("dead", true);

            _playerCapsuleCollider.direction = 0;
            _playerCapsuleCollider.radius = 0.15f;
            _playerCapsuleCollider.height = 0;
            _playerCapsuleCollider.center = new Vector3(0.0f, colliderVerticalOffset, 0.0f);

            InputManager.Instance.currentGameState = InputManager.GameStates.GameOver;
        }

        isPlayerKillable = false;
    }

}
