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

    [SerializeField] private InputAction _pauseAction;

    private void Awake()
    {
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
        SceneManager.LoadScene(1);
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isGamePaused = true;
    }

    public void ResumeGame()
    {
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
