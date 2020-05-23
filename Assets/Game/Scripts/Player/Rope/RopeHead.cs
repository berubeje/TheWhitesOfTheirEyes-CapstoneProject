using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeHead : MonoBehaviour
{
    public RopeLauncher ropeLauncher;

    private Rigidbody _rigidBody;
    private float _ropeSpeed;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();

        if(_rigidBody == null)
        {
            Debug.LogError("Unable to find Rigidbody component.");
        }

        if (ropeLauncher != null)
        {
            _ropeSpeed = ropeLauncher.ropeSpeed;
        }
        else
        {
            Debug.LogError("Unable to find source RopeLauncher component.");
        }
    }

    void Update()
    {
        _rigidBody.MovePosition(transform.position + (transform.forward * _ropeSpeed));
    }
}
