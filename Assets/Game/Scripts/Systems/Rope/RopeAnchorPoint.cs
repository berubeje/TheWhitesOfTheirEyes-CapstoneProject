///-------------------------------------------------------------------------------------------------
// file: RopeAnchorPoint.cs
//
// author: Jesse Berube
// date: N/A
//
// summary: This is attatched to any gameobject that is meant to be an anchor point
///-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class RopeAnchorPoint : MonoBehaviour
{
    public bool canAttach
    {
        get { return _allowAttach; }
        set
        {
            _allowAttach = value;

            if (isBossCore == false)
            {
                OnCanAttachChange(_allowAttach);
            }
        }
    }

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

    [HideInInspector]
    public bool resetting;

    public Vector3 angleOfPull;

    [Header("Optional")]
    public Transform pivot;

    [Header("If Boss Core")]
    public bool isBossCore;


    public UnityEvent pullStartEvent = new UnityEvent();

    private MeshRenderer _meshRenderer;

    private bool _allowAttach = true;
    private bool _pulling;
    private Transform _targetTransform;

    private Quaternion _targetAngle;
    private Quaternion _startRotation;
    private float _t;

    private float _repairTime;


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

        _meshRenderer = GetComponent<MeshRenderer>();

    }

    private void Update()
    {
        if (_pulling == true)
        {
            if (!isBossCore)
            {
                RotateObject();
            }
            else
            {
                PullCore();
            }
        }
    }

    private void PullCore()
    {
        BossCoreLogic core = GetComponent<BossCoreLogic>();
        core.enabled = true;

        _allowAttach = false;
    }

    // Mainly used when loading a checkpoint, resets the anchor points so they can be pulled down again.
    private void OnCanAttachChange(bool _canAttachValue)
    {
        if(_canAttachValue == true)
        {
            _meshRenderer.enabled = true;
            pullDone = false;
            _pulling = false;
        }
        else
        {
            _meshRenderer.enabled = false;
        }
    }

    public void AllowSwing(bool set)
    {
        _allowAttach = set;
    }

    // Tell the pull anchor point to allow the object to start falling
    public void StartPull()
    {
        if (canRepeatPull == false)
        {
            canAttach = false;
        }
        AudioManager.Instance.PlaySound("TreeFall");
        _startRotation = _targetTransform.rotation;

        _pulling = true;
        

        _targetAngle = Quaternion.Euler(_targetTransform.rotation.eulerAngles + angleOfPull);

        pullStartEvent.Invoke();
        //GetComponentInParent<FallingPillarObstacle>().isTriggered = true;
    }

    // Mainly used for the tree in the boss fight. Reverses the effect of the pull, as well as make the anchor point usable again.
    public void ResetPull(float time)
    {
        // Make sure the object was not already in the middle of resetting 
        if (!resetting)
        {
            _startRotation = _targetTransform.rotation;
            _targetAngle = Quaternion.Euler(_targetTransform.rotation.eulerAngles - angleOfPull);
        }

        _pulling = true;
        resetting = true;
        pullDone = false;

        _repairTime = time;
    }

    public void PauseRotation()
    {
        _pulling = false;
    }



    // Rotate the object so it has the effect of falling or turning.
    public void RotateObject()
    {
        if (!resetting)
        {
            _t += Time.deltaTime / pullTime;
        }
        else
        {
            _t += Time.deltaTime / _repairTime;
        }

        _targetTransform.rotation = Quaternion.Lerp(_startRotation, _targetAngle, _t);


        if (_t >= 1.0f)
        {
            _pulling = false;
            if(resetting)
            {
                canAttach = true;
                _meshRenderer.enabled = true;
                resetting = false;
            }
            else
            {
                pullDone = true;
            }
            _t = 0.0f;
        }
    }
}
