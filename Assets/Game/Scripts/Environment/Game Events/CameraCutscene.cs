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
    public string trackToPlay;
    public bool waitForPlayerInput;

    private CinemachineTrackedDolly _dollyTrack;
    private BoxCollider _collider;
    private JimController _jimController;
    private CinemachineBrain _cinemachineBrain;
    private bool _cutsceneStarted = false;
    private float _dollyPosition = 0.0f;
    private float _elapsedWaitTime = 0.0f;

    private void Awake()
    {
        _dollyTrack = cutsceneCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        _collider = GetComponent<BoxCollider>();
        _cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
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
            UICanvas.Instance.DisableAllControls();

            //Change transitions time for 
            _cinemachineBrain.m_DefaultBlend.m_Time = 5;

            //Show cinematic bars
            CinematicBars.Instance.ShowBars(100, 1f);

            // Switch to dolly camera
            cutsceneCamera.Priority = 100;

            // Start the cutscene
            _cutsceneStarted = true;
            
            // Play the music
            if(trackToPlay != "")
            {
                AudioManager.Instance.PlaySound(trackToPlay);
            }
        }
    }

    private void PlayCutscene()
    {
        _dollyPosition += Time.deltaTime * cameraTrackSpeed;
        _dollyTrack.m_PathPosition = _dollyPosition;

        if(_dollyPosition >= 1)
        {
            _elapsedWaitTime += Time.deltaTime;

            if (_elapsedWaitTime >= cameraWaitTime)
            {
                // Enable all controls again
                InputManager.Instance.EnableAllControls();
                UICanvas.Instance.EnableAllControls();

                _cinemachineBrain.m_DefaultBlend.m_Time = 2;

                // Hide cinematic bars
                CinematicBars.Instance.HideBars(1f);
                if (waitForPlayerInput)
                {
                    if (_jimController.isReceivingLeftStick || _jimController.isReceivingRightStick)
                    {
                        // Switch back to normal camera after the player moves
                        cutsceneCamera.Priority = 1;

                        _cutsceneStarted = false;

                        StartCoroutine(DestroyGameObject());
                    }
                }
                else
                {
                    // Switch back to normal camera after the player moves
                    cutsceneCamera.Priority = 1;

                    _cutsceneStarted = false;

                    StartCoroutine(DestroyGameObject());
                }
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
