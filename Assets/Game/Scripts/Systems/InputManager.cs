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
        SceneManager.LoadScene(0, LoadSceneMode.Additive);

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

        //TO DO: ADD CALLBACKS FOR ROPE FIRE AND RELEASE

        //_pullTiePressAction = _playerControls.Player.PullTiePress;
        //_pullTiePressAction.performed += ropeController.OnLeftTriggerDown;
        //_pullTiePressAction.performed += ropeController.OnLeftTriggerUp;

        _pullTieAction = _playerControls.Player.PullTie;
        _pullTieAction.performed += ropeController.OnLeftTriggerPull;
        _pullTieAction.canceled += ropeController.OnLeftTriggerTie;
    }


    private void OnEnable()
    {
        _moveAction.Enable();
        _lookAction.Enable();
        _fireAction.Enable();
        _pullTieAction.Enable();
        _rollAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _lookAction.Disable();
        _fireAction.Disable();
        _pullTieAction.Disable();
        _rollAction.Disable();
    }
}
