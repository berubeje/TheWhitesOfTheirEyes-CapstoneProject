using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPillarObstacle : IObstacle
{
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private RopeAnchorPoint _anchorPoint;

    private void Awake()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;

        _anchorPoint = GetComponentInChildren<RopeAnchorPoint>();
    }
    public override void ResetObstacle()
    {
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
        _anchorPoint.canAttach = true;
        isTriggered = false;
    }

    public override void UnresetObstacle()
    {
        _anchorPoint.canAttach = false;
        isTriggered = true;
    }
}
