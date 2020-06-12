using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    public FallingRock fallingRock;

    private Rigidbody _fallingRockRigidBody;
    private SphereCollider _sphereCollider;
    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _fallingRockRigidBody = fallingRock.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _fallingRockRigidBody.useGravity = true;
            fallingRock.isPlayerKillable = true;
            _sphereCollider.enabled = false;
        }
    }
}
