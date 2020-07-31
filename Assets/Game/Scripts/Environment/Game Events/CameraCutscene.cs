using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCutscene : MonoBehaviour
{
    public CinemachineVirtualCamera cutsceneCamera;
    public float cameraTrackSpeed;
    public float cameraWaitTime;
    public float cameraDestroyWaitTime;

    private CinemachineTrackedDolly _dollyTrack;
    private BoxCollider _collider;
    private JimController _jimController;
    private bool _cutsceneStarted = false;
    private float _dollyPosition = 0.0f;

    private void Awake()
    {
        _dollyTrack = cutsceneCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        _collider = GetComponent<BoxCollider>();
    }
    private void Update()
    {
        if (_cutsceneStarted)
        {
            PlayCutscene();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _jimController = other.GetComponent<JimController>();

            _collider.enabled = false;

            // Disable all player input
            InputManager.Instance.DisableAllControls();

            // Switch to dolly camera
            cutsceneCamera.Priority = 100;

            // Start the cutscene
            _cutsceneStarted = true;
        }
    }

    private void PlayCutscene()
    {
        _dollyPosition += Time.deltaTime * cameraTrackSpeed;
        _dollyTrack.m_PathPosition = _dollyPosition;

        if(_dollyPosition >= 1 + (cameraWaitTime * Time.deltaTime))
        {
            // Enable all controls again
            InputManager.Instance.EnableAllControls();

            if(_jimController.isReceivingLeftStick || _jimController.isReceivingRightStick)
            {
                // Switch back to normal camera after the player moves
                cutsceneCamera.Priority = 1;

                _cutsceneStarted = false;

                StartCoroutine(DestroyGameObject());
            }
        }
    }

    private IEnumerator DestroyGameObject()
    {
        // Wait before destroying this gameobject
        yield return new WaitForSeconds(cameraDestroyWaitTime);
        Destroy(this.gameObject);
    }
}
