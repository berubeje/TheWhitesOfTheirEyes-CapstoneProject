using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenSetup : MonoBehaviour
{
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        _mainCamera.transform.position = new Vector3(0.0f, -11.8f, -5f);
        _mainCamera.transform.rotation = Quaternion.identity;
        _mainCamera.orthographic = true;
        _mainCamera.orthographicSize = 2;
        _mainCamera.farClipPlane = 20;
    }
}
