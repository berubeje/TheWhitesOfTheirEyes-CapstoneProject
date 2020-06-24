﻿///-------------------------------------------------------------------------------------------------
// file: RopeAnchorPoint.cs
//
// author: Jesse Berube
// date: N/A
//
// summary: This is attatched to any gameobject that is meant to be an anchor point
///-------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RopeAnchorPoint : MonoBehaviour
{
    [HideInInspector]
    public bool cantAttach;

    public AnchorType anchorType;
    public enum AnchorType
    {
        Swing,
        Pull
    }

    [Header("If pull type")]
    public float pullTime;
    public bool additiveAngle = false;

    public Vector3 angleOfPull;


    public bool _pulling;
    private bool _pullDone;
    private Transform _parentTransform;

    private Quaternion _targetAngle;
    private Quaternion _startRotation;
    private float _t;

    private void Awake()
    {
        _parentTransform = transform.parent;
    }

    private void Update()
    {
        if (_pulling == true)
        {
            RotateObject();
        }
    }

    public void StartPull()
    {
        cantAttach = true;
        GetComponent<MeshRenderer>().enabled = false;

        _startRotation = _parentTransform.rotation;

        if (_pullDone == false)
        {
            _pulling = true;
        }

        if (additiveAngle)
        {
            _targetAngle = Quaternion.Euler(_parentTransform.rotation.eulerAngles + angleOfPull);
        }
        else
        {
            _targetAngle = Quaternion.Euler(angleOfPull);
        }

    }

    public void RotateObject()
    {
        _t += Time.deltaTime / pullTime;
        _parentTransform.rotation = Quaternion.Lerp(_startRotation, _targetAngle, _t);

        if(_t >= 1.0f)
        {
            _pulling = false;
            _pullDone = true;
        }
    }
}
