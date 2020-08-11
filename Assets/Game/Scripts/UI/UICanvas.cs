using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class UICanvas : Singleton<UICanvas>
{
    public GameObject pauseMenu;
    public GameObject controlsMenu;
    public GameObject settingsMenu;
    public GameObject gameOverMenu;
    public GameObject restartConfirmationMenu;
    public GameObject gameFinishedMenu;

    [Space]
    public GameObject healthBar;
    public RectTransform healthBarSlider;
    
    public GameObject bossHealthBar;
    public RectTransform bossHealthBarSlider;

    [Space]
    public CinematicBars cinematicBars;

    private Canvas _canvas;

    [SerializeField] private InputAction _pauseAction;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        
        if(_canvas == null)
        {
            Debug.LogError("Canvas not found.");
        }

        _canvas.worldCamera = Camera.main;
        _canvas.planeDistance = 0.05f;

        BindControls();
        InputManager.Instance.OnGameStateChange += OnGameStateChanged;
    }

    private void Start()
    {
        // Start with controls disabled
        //DisableAllControls();
    }

    private void OnGameStateChanged(InputManager.GameStates state)
    {
        switch (state)
        {
            case InputManager.GameStates.Playing:
                pauseMenu.SetActive(false);
                controlsMenu.SetActive(false);
                settingsMenu.SetActive(false);
                gameOverMenu.SetActive(false);
                gameFinishedMenu.SetActive(false);
                restartConfirmationMenu.SetActive(false); 
                healthBar.SetActive(true);

                if (InputManager.instance.jimController.boss.bossCutsceneFinished)
                {
                    bossHealthBar.SetActive(true);
                }

                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
                break;

            case InputManager.GameStates.Paused:
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                break;

            case InputManager.GameStates.Reloading:
                PlayerData data = SaveLoadSystem.LoadPlayerData();

                if (data != null)
                {
                    CheckpointManager.Instance.LoadCheckpoint(data);
                }

                InputManager.Instance.currentGameState = InputManager.GameStates.Playing;
                break;

            case InputManager.GameStates.Resetting:
                Time.timeScale = 1;
                InputManager.Instance.currentGameState = InputManager.GameStates.Playing;
                break;

            case InputManager.GameStates.GameOver:
                gameOverMenu.SetActive(true);
                healthBar.SetActive(false); 
                bossHealthBar.SetActive(false);
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.Confined;
                break;

            case InputManager.GameStates.GameFinished:
                gameFinishedMenu.SetActive(true);
                healthBar.SetActive(false);
                bossHealthBar.SetActive(false);
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.Confined;
                break;
        }
    }

    public void LoadLastCheckpoint()
    {
        InputManager.Instance.currentGameState = InputManager.GameStates.Reloading;
    }

    public void PauseGame()
    {
        InputManager.Instance.currentGameState = InputManager.GameStates.Paused;
    }

    public void ResumeGame()
    {
        InputManager.Instance.currentGameState = InputManager.GameStates.Playing;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeHealthBar(float normalizedHealth)
    {
        healthBarSlider.sizeDelta = new Vector2((-1 + normalizedHealth) * 228, 0);
    }

    public void ChangeBossHealthBar(float normalizedHealth)
    {
        bossHealthBarSlider.offsetMax = new Vector2((-1 + normalizedHealth) * 398, 0);
    }

    private void OnPauseButtonDown(InputAction.CallbackContext context)
    {
        if (InputManager.Instance.currentGameState == InputManager.GameStates.Playing)
        {
            PauseGame();
        }
        else if(InputManager.Instance.currentGameState == InputManager.GameStates.Paused)
        {
            ResumeGame();
        }
    }

    public void BindControls()
    {
        _pauseAction.started += OnPauseButtonDown;
    }

    public void UnbindControls()
    {
        _pauseAction.started -= OnPauseButtonDown;
    }
    public void EnableAllControls()
    {
        _pauseAction.Enable();
    }
    public void DisableAllControls()
    {
        _pauseAction.Disable();
    }

    private void OnEnable()
    {
        EnableAllControls();
    }

    private void OnDisable()
    {
        DisableAllControls();
    }
}
