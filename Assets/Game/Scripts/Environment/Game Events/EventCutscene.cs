using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCutscene : MonoBehaviour
{
    public CinemachineVirtualCamera cutsceneCamera;
    public JimController jimController;
    public float cameraTrackSpeed;
    public float cameraWaitTime;
    public float cameraDestroyWaitTime;
    public bool waitForPlayerInput;

    private CinemachineTrackedDolly _dollyTrack;
    private CinemachineBrain _cinemachineBrain;
    private bool _cutsceneStarted = false;
    private float _dollyPosition = 0.0f;
    private float _elapsedWaitTime = 0.0f;

    private float _positionOffset;

    private void Awake()
    {
        _dollyTrack = cutsceneCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        _cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();

        _positionOffset = Vector3.Distance(transform.position, jimController.boss.transform.position);
    }
    private void Update()
    {
        if (_cutsceneStarted)
        {
            PlayCutscene();
        }
    }

    public void ActivateCutscene()
    {
        // Disable all player input
        InputManager.Instance.DisableAllControls();
        UICanvas.Instance.DisableAllControls();

        //Change transitions time for 
        _cinemachineBrain.m_DefaultBlend.m_Time = 0;

        //Show cinematic bars
        CinematicBars.Instance.ShowBars(100, 1f);

        // Switch to dolly camera
        cutsceneCamera.Priority = 100;

        // Move the track to the appropriate position
        float bossYRotation = jimController.boss.transform.eulerAngles.y;
        Debug.LogError(jimController.boss.transform.eulerAngles);
        if (bossYRotation > 240 && bossYRotation <= 330)
        {
            transform.position = jimController.boss.transform.position + (Vector3.left * _positionOffset);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 270, transform.eulerAngles.z);
        }
        else if (bossYRotation > 60 && bossYRotation <= 150)
        {
            transform.position = jimController.boss.transform.position + (Vector3.right * _positionOffset); 
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90, transform.eulerAngles.z);
        }
        else if (bossYRotation > 150 && bossYRotation <= 240)
        {
            transform.position = jimController.boss.transform.position + (Vector3.back * _positionOffset);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
        }
        else
        {
            transform.position = jimController.boss.transform.position + (Vector3.forward * _positionOffset);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
        }


        // Start the cutscene
        _cutsceneStarted = true;
        
    }

    private void PlayCutscene()
    {
        _dollyPosition += Time.deltaTime * cameraTrackSpeed;
        _dollyTrack.m_PathPosition = _dollyPosition;

        if (_dollyPosition >= 1)
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
                    if (jimController.isReceivingLeftStick || jimController.isReceivingRightStick)
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
