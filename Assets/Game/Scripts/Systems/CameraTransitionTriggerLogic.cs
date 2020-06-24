///-------------------------------------------------------------------------------------------------
// file: CameraTransitionTriggerLogic.cs
//
// author: Jesse Berube
// date: 06/18/2020
//
// summary: When the player goes through this, the camera changes to a new dolly track.
///-------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransitionTriggerLogic : MonoBehaviour
{
    public GameObject dollyCameraTrack;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<JimController>() != null)
        {
           if(DollyCameraManager.Instance.currentDolly != dollyCameraTrack)
           {
                DollyCameraManager.Instance.ChangeDolly(dollyCameraTrack);
           }
        }
    }
}
