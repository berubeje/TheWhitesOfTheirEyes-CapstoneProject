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

    private Canvas _canvas;
    private GameObject _inputManager;
   
    [SerializeField] private InputAction _pauseAction;

    private void Awake()
    {
        _inputManager = InputManager.Instance.gameObject;

        _canvas = GetComponent<Canvas>();
        
        if(_canvas == null)
        {
            Debug.LogError("Canvas not found.");
        }

        _canvas.worldCamera = Camera.main;
        _canvas.planeDistance = 1;

        _pauseAction.started += OnPauseButtonDown;
    }

    private void Update()
    {
        if(InputManager.Instance.currentGameState == InputManager.GameStates.GameOver)
        {
            gameOverMenu.SetActive(true); 
            _inputManager.SetActive(false);
        }    
    }

    public void LoadLastCheckpoint()
    {
        InputManager.Instance.currentGameState = InputManager.GameStates.Playing;
        SceneManager.LoadScene(1); 
        Time.timeScale = 1;
    }

    public void ResetLevel()
    {
        InputManager.Instance.currentGameState = InputManager.GameStates.Playing;
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        _inputManager.SetActive(false);

        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        InputManager.Instance.currentGameState = InputManager.GameStates.Paused;
    }

    public void ResumeGame()
    {
        _inputManager.SetActive(true);

        pauseMenu.SetActive(false);
        controlsMenu.SetActive(false);
        settingsMenu.SetActive(false);

        Time.timeScale = 1;

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
