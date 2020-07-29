using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCutscene : MonoBehaviour
{
    public CinemachineVirtualCamera cutsceneCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InputManager.Instance.DisableAllControls();
            cutsceneCamera.Priority = 100;
        }
    }
}
