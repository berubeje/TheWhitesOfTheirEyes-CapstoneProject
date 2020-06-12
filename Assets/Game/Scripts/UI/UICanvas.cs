using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UICanvas : MonoBehaviour
{
    public static bool isGamePaused = false;

    public GameObject pauseMenu;
    public GameObject controlsMenu;
    public GameObject settingsMenu;

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

    public void ResetLevel()
    {
        isGamePaused = false;
        SceneManager.LoadScene(2);
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        _inputManager.SetActive(false);

        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isGamePaused = true;
    }

    public void ResumeGame()
    {
        _inputManager.SetActive(true);

        pauseMenu.SetActive(false);
        controlsMenu.SetActive(false);
        settingsMenu.SetActive(false);

        Time.timeScale = 1;

        isGamePaused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnPauseButtonDown(InputAction.CallbackContext context)
    {
        if (!isGamePaused)
        {
            PauseGame();
        }
        else
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
