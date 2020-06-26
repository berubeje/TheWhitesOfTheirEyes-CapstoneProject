using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
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
    public Renderer eyeRenderer;
    public Material activatedMaterial;
    public Material unactivatedMaterial;

    private SphereCollider _sphereCollider;

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //_sphereCollider.enabled = false;

            isTriggered = true;

            CheckpointManager.Instance.SaveCheckpoint(transform.position, 100.0f);

            eyeRenderer.material = activatedMaterial;

            _sphereCollider.enabled = false;

        }
    }

    public override void ResetObstacle()
    {
        isTriggered = false;
        eyeRenderer.material = unactivatedMaterial;
        _sphereCollider.enabled = true;
    }

    public override void UnresetObstacle()
    {
        isTriggered = true;
        eyeRenderer.material = activatedMaterial;
        _sphereCollider.enabled = false;
    }
}
