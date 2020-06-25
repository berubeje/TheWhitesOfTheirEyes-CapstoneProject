using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (_instance == null)
        {
            _instance = (T)FindObjectOfType(typeof(T));
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
