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
        if(_ropeAnchorPoint.pullDone && _addedToList == false)
        {
            _currentAlertTime += Time.deltaTime;

            if (_currentAlertTime >= alertTimer)
            {
                _addedToList = true;
                bossController.fallenTreeList.Add(_ropeAnchorPoint);
                _currentAlertTime = 0.0f;
            }
        }
        else if(_ropeAnchorPoint.pullDone == false && _addedToList == true)
        {
            _addedToList = false;
        }
    }
}
