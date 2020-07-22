using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetUp : MonoBehaviour
{

    public MenuClassifier menuToShow;
    public MenuClassifier menuToHide;

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    void Start()
    {
        MenuManager.Instance.showMenu(menuToShow);
        MenuManager.Instance.hideMenu(menuToHide);

        _mainCamera.orthographic = false;

    }
}
