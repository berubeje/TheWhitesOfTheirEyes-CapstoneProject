﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOnEnable: MonoBehaviour
{
    public Button button;

    private void OnEnable()
    {
        button.Select();
    }
}