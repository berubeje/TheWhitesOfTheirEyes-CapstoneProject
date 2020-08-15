using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetUp : MonoBehaviour
{
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    void Start()
    {

        _mainCamera.orthographic = false;

        // Enable UI controls
        UICanvas.Instance.EnableAllControls();
    }
}
