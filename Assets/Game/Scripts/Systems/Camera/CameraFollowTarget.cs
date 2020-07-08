﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTarget : ControllableBase
{
    public JimController jimController;
    public float followSpeed;
    public Vector3 followOffset;

    private void Awake()
    {
        jimController.cameraFollowTarget = this;
    }

    void LateUpdate()
    {
        transform.position = jimController.transform.position + followOffset;
        transform.rotation = jimController.transform.rotation;
    }
     
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
