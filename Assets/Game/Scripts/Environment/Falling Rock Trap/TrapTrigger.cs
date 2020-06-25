using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    public IObstacle fallingRockObstacle;
    public FallingRock fallingRock;
    private Rigidbody _fallingRockRigidBody;

    
    private void Awake()
    {
        _fallingRockRigidBody = fallingRock.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _fallingRockRigidBody.useGravity = true;
            
            fallingRock.isPlayerKillable = true;
            
            fallingRockObstacle.isTriggered = true;

            gameObject.SetActive(false);
        }
    }
}
