///-------------------------------------------------------------------------------------------------
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
    public float timeToStartPull;

    
    public float pullTime;
    public bool canRepeatPull = false;
    public bool pullDone = false;

    public Vector3 angleOfPull;

    [Header("Optional")]
    public Transform pivot;

    private bool _pulling;
    private bool _resetting;
    private Transform _targetTransform;

    private Quaternion _targetAngle;
    private Quaternion _startRotation;
    private float _t;

    private void Awake()
    {
        if (pivot == null)
        {
            _targetTransform = transform.parent;
        }
        else
        {
            _targetTransform = pivot;
        }
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
        if (canRepeatPull == false)
        {
            cantAttach = true;
            GetComponent<MeshRenderer>().enabled = false;
        }

        _startRotation = _targetTransform.rotation;

        _pulling = true;
        

        _targetAngle = Quaternion.Euler(_targetTransform.rotation.eulerAngles + angleOfPull);
        _t = 0.0f;   
    }

    public void ResetPull()
    {
        _pulling = true;
        _resetting = true;
        pullDone = false;


        _startRotation = _targetTransform.rotation;
        _targetAngle = Quaternion.Euler(_targetTransform.rotation.eulerAngles - angleOfPull);
        _t = 0.0f;

    }

    public void RotateObject()
    {
        _t += Time.deltaTime / pullTime;
        _targetTransform.rotation = Quaternion.Lerp(_startRotation, _targetAngle, _t);

        if(_t >= 1.0f)
        {
            _pulling = false;
            if(_resetting)
            {
                cantAttach = false;
                GetComponent<MeshRenderer>().enabled = true;
                _resetting = false;
            }
            else
            {
                pullDone = true;
            }
        }
    }
}
