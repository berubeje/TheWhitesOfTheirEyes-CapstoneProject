using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

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
    [SerializeField] private InputAction _pullAction;

    
    private void Awake()
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
        //TO DO: ADD CALLBACKS FOR ROPE FIRE AND RELEASE

        _pullAction = _playerControls.Player.Pull;
    }

    private void OnEnable()
    {
        _moveAction.Enable();
        _lookAction.Enable();
        _fireAction.Enable();
        _pullAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _lookAction.Disable();
        _fireAction.Disable();
        _pull.Disable();
    }
}
