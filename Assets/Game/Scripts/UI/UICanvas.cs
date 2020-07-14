using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UICanvas : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject controlsMenu;
    public GameObject settingsMenu;
    public GameObject gameOverMenu;
    public GameObject gameFinishedMenu;

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
        _canvas.planeDistance = 2;

        _pauseAction.started += OnPauseButtonDown;
    }

    private void Start()
    {
        InputManager.Instance.OnGameStateChange += OnGameStateChanged;
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
                Time.timeScale = 1;
                break;

            case InputManager.GameStates.Paused:
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
                break;

            case InputManager.GameStates.Reloading:
                PlayerData data = SaveLoadSystem.LoadPlayerData();

                if (data != null)
                {
                    CheckpointManager.Instance.LoadCheckpoint(data);
                }

                InputManager.Instance.currentGameState = InputManager.GameStates.Playing;
                break;

            case InputManager.GameStates.GameOver:
                gameOverMenu.SetActive(true);
                break;

            case InputManager.GameStates.GameFinished:
                gameFinishedMenu.SetActive(true);
                Time.timeScale = 0;
                break;
        }
    }

    public void LoadLastCheckpoint()
    {
        InputManager.Instance.currentGameState = InputManager.GameStates.Reloading;
    }

    public void ResetLevel()
    {
        InputManager.Instance.currentGameState = InputManager.GameStates.Playing;
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        InputManager.Instance.currentGameState = InputManager.GameStates.Paused;
    }

    public void ResumeGame()
    {
        InputManager.Instance.currentGameState = InputManager.GameStates.Playing;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
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

    private void OnEnable()
    {
        _pauseAction.Enable();
    }

    private void OnDisable()
    {
        _pauseAction.Disable();
    }
}
