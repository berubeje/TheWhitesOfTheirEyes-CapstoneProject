///-------------------------------------------------------------------------------------------------
// file: CameraTransitionTriggerLogic.cs
//
// author: Jesse Berube
// date: 06/18/2020
//
// summary: Tells the auto camera to go to new position/rotation when player passes through
///-------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransitionTriggerLogic : MonoBehaviour
{
    public AutoCameraLogic autoCamera;

    public Transform cameraTransitionTransform;
    public CameraTransitionTriggerLogic previousCameraTransitionTrigger;


    public bool followPlayer;
    public bool centerX;
    public bool centerY;
    public bool centerZ;

    public bool lookAtPlayer;
    public float timeToMoveToPosition;
    public float timeToChangeRotation;

    public bool _backtracking;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<JimController>() != null)
        {
            if (_backtracking == false)
            {
                
                autoCamera.SetCameraTransition(cameraTransitionTransform, followPlayer, lookAtPlayer, timeToMoveToPosition, timeToChangeRotation, centerX, centerY, centerZ);

                _backtracking = true;
            }
            else
            {
                autoCamera.SetCameraTransition(previousCameraTransitionTrigger.cameraTransitionTransform, previousCameraTransitionTrigger.followPlayer, previousCameraTransitionTrigger.lookAtPlayer, previousCameraTransitionTrigger.timeToMoveToPosition, previousCameraTransitionTrigger.timeToChangeRotation, centerX, centerY, centerZ);

                _backtracking = false;
            }
        }
    }
}
