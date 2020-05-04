using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    // A list of all objects in the game that are controllable or interactable 
    public List<PlayerControllerBase> controllableObjects;

    //The main player gameobject. Must have a PlayerControllableBase type component
    public PlayerControllerBase player;

    private Vector2 leftStickInput;
    private Vector2 rightStickInput;
    void Update()
    {
        player.LeftAnalogStick();
        player.RightAnalogStick();
    }


    private void OnNorthFaceButtonDown() { }
    private void OnNorthFaceButton() { }
    private void OnNorthFaceButtonUp() { }
    private void OnEastFaceButtonDown() { }
    private void OnEastFaceButton() { }
    private void OnEastFaceButtonUp() { }
    private void OnSouthFaceButtonDown() { }
    private void OnSouthFaceButton() { }
    private void OnSouthFaceButtonUp() { }
    private void OnWestFaceButtonDown() { }
    private void OnWestFaceButton() { }
    private void OnWestFaceButtonUp() { }
    private void OnLeftShoulderButtonDown() { }
    private void OnLeftShoulderButton() { }
    private void OnLeftShoulderButtonUp() { }
    private void OnLeftTriggerButtonDown() { }
    private void OnLeftTriggerButton() { }
    private void OnLeftTriggerButtonUp() { }
    private void OnRightShoulderButtonDown() { }
    private void OnRightShoulderButton() { }
    private void OnRightShoulderButtonUp() { }
    private void OnRightTriggerButtonDown() { }
    private void OnRightTriggerButton() { }
    private void OnRightTriggerButtonUp() { }
}
