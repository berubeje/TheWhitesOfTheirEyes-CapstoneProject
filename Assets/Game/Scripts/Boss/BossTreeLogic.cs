//-------------------------------------------------------------------------------------------------
// file: BossTreeLogic.cs
//
// author: Jesse Berube
// date: 2020/07/17
//
// summary: The scripts detects when it has been pulled down, then alerts the boss that it has been knocked down.
///-------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTreeLogic : MonoBehaviour
{
    public BossController bossController;
    public float alertBossTimer = 0.5f;

    private bool _addedToList = false;
    private RopeAnchorPoint _ropeAnchorPoint;
    private RopeAnchorPoint _swingAnchorPoint;
    private float _currentAlertTime = 0.0f;

    private void Start()
    {
        if(bossController == null)
        {
            this.enabled = false;
        }

        RopeAnchorPoint[] anchorPoints = transform.parent.GetComponentsInChildren<RopeAnchorPoint>();

        foreach(RopeAnchorPoint anchorPoint in anchorPoints)
        {
            if(anchorPoint.gameObject == this.gameObject)
            {
                _ropeAnchorPoint = anchorPoint;
            }
            else
            {
                _swingAnchorPoint = anchorPoint;
            }
        }

        _swingAnchorPoint.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        // Checks to see if the anchor point has been pulled down, if so, tell the boss it has been knocked down after a delay.
        if (_ropeAnchorPoint.pullDone && _addedToList == false)
        {
            if (bossController.treeRepairInProgress == false)
            {
                _currentAlertTime += Time.deltaTime;

                if (_currentAlertTime >= alertBossTimer)
                {
                    _addedToList = true;
                    bossController.fallenTreeList.Add(_ropeAnchorPoint);
                    _currentAlertTime = 0.0f;
                }
            }

            if(_swingAnchorPoint.gameObject.activeSelf == false)
            {
                _swingAnchorPoint.gameObject.SetActive(true);
            }
        }
        else if (_ropeAnchorPoint.pullDone == false && _ropeAnchorPoint.resetting == false && _addedToList == true)
        {
            bossController.fallenTreeList.Remove(_ropeAnchorPoint);
            _addedToList = false;
        }
        else if(_ropeAnchorPoint.pullDone == false && _swingAnchorPoint.gameObject.activeSelf == true)
        {
            _swingAnchorPoint.gameObject.SetActive(false);
        }

    }
}
