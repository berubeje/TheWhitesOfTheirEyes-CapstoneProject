using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

public class InputManager : Singleton<InputManager>
{
    public JimController jimController;
    public RopeController ropeController;

    public enum GameStates
    {
        Playing,
        Paused,
        Reloading,
        GameOver
    }

    private GameStates _gameState = GameStates.Playing;

    // Set up event handler for whenever the game state is changed
    public GameStates currentGameState
    {
        get { return _gameState; }
        set
        {
            if (_gameState == value)
            {
                return;
            }
            _gameState = value;

            if (OnGameStateChange != null) 
            {
                OnGameStateChange(_gameState); 
            }
        }
    }
    public delegate void OnGameStateChangeDelegate(GameStates state);
    public event OnGameStateChangeDelegate OnGameStateChange;

    [SerializeField] private PlayerControls _playerControls;
    [Space]
    [SerializeField] private InputAction _moveAction;
    [SerializeField] private InputAction _rollAction;
    [SerializeField] private InputAction _lookAction;
    [SerializeField] private InputAction _fireAction;
    [SerializeField] private InputAction _pullTieAction;


    
    private void Awake()
    {
        applicationClosing = false;

        // Load the UI scene before anything else
        if (!SceneManager.GetSceneByBuildIndex(2).isLoaded)
        {
            SceneManager.LoadScene(2, LoadSceneMode.Additive);
        }
        OnGameStateChange += OnGameStateChanged;
    }

    private void Start()
    {
        BindControls();
        EnableAllControls();
    }

    private void OnGameStateChanged(GameStates state)
    {
        switch (state)
        {
            case GameStates.Playing:
                EnableAllControls();
                break;

            case GameStates.Paused:
                DisableAllControls();
                break;

            case GameStates.Reloading:
                DisableAllControls();
                break;

            case GameStates.GameOver:
                DisableAllControls();
                break;
        }
    }

    private void BindControls()
    {
        _playerControls = new PlayerControls();

        _moveAction = _playerControls.Player.Move;
        _moveAction.performed += jimController.OnLeftStick;
        _moveAction.canceled += jimController.OnLeftStick;

        _rollAction = _playerControls.Player.Roll;
        _rollAction.performed += jimController.OnEastButtonDown;

        _lookAction = _playerControls.Player.Look;
        _lookAction.performed += jimController.OnRightStick;
        _lookAction.canceled += jimController.OnRightStick;

        _fireAction = _playerControls.Player.Fire;
        _fireAction.performed += ropeController.OnRightTriggerDown;
        _fireAction.canceled += ropeController.OnRightTriggerUp;

        _pullTieAction = _playerControls.Player.PullTie;
        _pullTieAction.performed += ropeController.OnLeftTriggerPull;
        _pullTieAction.canceled += ropeController.OnLeftTriggerTie;
    }
    private void EnableAllControls()
    {
        _moveAction.Enable();
        _lookAction.Enable();
        _fireAction.Enable();
        _pullTieAction.Enable();
        _rollAction.Enable();
    }
    private void DisableAllControls()
    {
        _moveAction.Disable();
        _lookAction.Disable();
        _fireAction.Disable();
        _pullTieAction.Disable();
        _rollAction.Disable();
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
