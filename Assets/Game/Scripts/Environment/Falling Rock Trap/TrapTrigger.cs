using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class TrapTrigger : MonoBehaviour
{
    public IObstacle fallingRockObstacle;
    public FallingRock fallingRock;
    public Light light;

    private Rigidbody _fallingRockRigidBody;
    private MeshRenderer _fallingMeshRenderer;

    
    private void Awake()
    {
        _fallingRockRigidBody = fallingRock.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            fallingRock.gameObject.layer = 11;

            _fallingRockRigidBody.useGravity = true;
            
            fallingRock.isPlayerKillable = true;
            
            fallingRockObstacle.isTriggered = true;

            gameObject.SetActive(false);
        }
    }
}
