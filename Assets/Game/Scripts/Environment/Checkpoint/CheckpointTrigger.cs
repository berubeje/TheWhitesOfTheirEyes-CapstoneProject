using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///-------------------------------------------------------------------------------------------------
// file: CheckpointTrigger.cs
//
// author: Rishi Barnwal
// date: 06/18/2020
//
// summary: keeps a list of all obstacles that come before this checkpoint and resets them as necessary
///-------------------------------------------------------------------------------------------------
///

public class CheckpointTrigger : IObstacle
{
    public List<IObstacle> obstacles;

    public Renderer eyeRenderer;
    public Material activatedMaterial;


    private SphereCollider _sphereCollider;

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _sphereCollider.enabled = false;

            isTriggered = true;

            foreach(IObstacle o in obstacles)
            {
                CheckpointManager.Instance.savedObstacles.Add(o);
            }

            obstacles.Clear();

            CheckpointManager.Instance.SaveCheckpoint(transform.position, 100.0f);

            eyeRenderer.material = activatedMaterial;
        }
    }

    public override void UnresetObstacle()
    {
        _sphereCollider.enabled = false;
        isTriggered = true;
        eyeRenderer.material = activatedMaterial;
    }
}
