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
    public float alertTimer = 1.5f;

    private bool _addedToList = false;
    private RopeAnchorPoint _ropeAnchorPoint;
    private float _currentAlertTime = 0.0f;

    private void Start()
    {
        _ropeAnchorPoint = GetComponent<RopeAnchorPoint>();
    }
    // Update is called once per frame
    void Update()
    {
        // Checks to see if the anchor point has been pulled down, if so, tell the boss it has been knocked down after a delay.
        if(_ropeAnchorPoint.pullDone && _addedToList == false)
        {
            if (bossController.treeRepairInProgress == false)
            {
                _currentAlertTime += Time.deltaTime;

                if (_currentAlertTime >= alertTimer)
                {
                    _addedToList = true;
                    bossController.fallenTreeList.Add(_ropeAnchorPoint);
                    _currentAlertTime = 0.0f;
                }
            }
        }
        else if(_ropeAnchorPoint.pullDone == false && _addedToList == true)
        {
            _addedToList = false;
        }
    }
}
