using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeLauncher : ControllableBase
{
    public enum LauncherStates
    {
        Ready,
        Fired,
        Latched
    }

    public LauncherStates currentLauncherState = LauncherStates.Ready;
    public GameObject ropeHead;
    public GameObject ropeBase;

    public float ropeRange;
    public float ropeSpeed;

    private GameObject _launchedRopeHead;
    private float _rightTrigger;

    void Update()
    {
        if(_launchedRopeHead != null)
        {
            if(Vector3.Distance(transform.position, _launchedRopeHead.transform.position) >= ropeRange)
            {
                Destroy(_launchedRopeHead);
            }
        }
    }

    public override void RightTriggerButton()
    {
        _rightTrigger = Input.GetAxis("Right Trigger Button");

        if (currentLauncherState == LauncherStates.Ready && _rightTrigger >= 1.0f)
        {
            _launchedRopeHead = GameObject.Instantiate(ropeHead, ropeBase.transform.position, transform.rotation);
            _launchedRopeHead.GetComponent<RopeHead>().ropeLauncher = this;
            currentLauncherState = LauncherStates.Fired;
        }
        else if(_rightTrigger <= -1.0f && _launchedRopeHead == null)
        {
            currentLauncherState = LauncherStates.Ready;
        }
    }
}
