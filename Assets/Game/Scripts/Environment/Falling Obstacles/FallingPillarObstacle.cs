using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPillarObstacle : IObstacle
{
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private RopeAnchorPoint _anchorPoint;
    private MeshRenderer _anchorPointMeshRenderer;
    private void Awake()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;

        _anchorPoint = GetComponentInChildren<RopeAnchorPoint>();
        _anchorPointMeshRenderer = _anchorPoint.GetComponent<MeshRenderer>();
    }
    public override void ResetObstacle()
    {
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
        _anchorPoint.cantAttach = false;
        _anchorPointMeshRenderer.enabled = true;
        isTriggered = false;
    }

    public override void UnresetObstacle()
    {
    }
}
