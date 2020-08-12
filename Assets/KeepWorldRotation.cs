//-------------------------------------------------------------------------------------------------
// file: KeepWorldRotation.cs
//
// author: Jesse Berube
// date: 2020/07/10
//
// summary: Keeps the rotation that the gameobject started at when it is a child.
///-------------------------------------------------------------------------------------------------


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
