using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepWorldRotation : MonoBehaviour
{
    private Quaternion _worldRotation;

    private void OnEnable()
    {
        _worldRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = _worldRotation;
    }
}
