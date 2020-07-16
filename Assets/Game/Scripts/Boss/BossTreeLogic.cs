using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTreeLogic : MonoBehaviour
{
    public BossController bossController;

    private bool _addedToList = false;
    private RopeAnchorPoint _ropeAnchorPoint;


    private void Start()
    {
        _ropeAnchorPoint = GetComponent<RopeAnchorPoint>();
    }
    // Update is called once per frame
    void Update()
    {
        if(_ropeAnchorPoint.pullDone && _addedToList == false)
        {
            _addedToList = true;
            bossController.fallenTreeList.Add(_ropeAnchorPoint);
        }
        else if(_ropeAnchorPoint.pullDone == false && _addedToList == true)
        {
            _addedToList = false;
        }
    }
}
