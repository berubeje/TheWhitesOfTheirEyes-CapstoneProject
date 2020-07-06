///-------------------------------------------------------------------------------------------------
// file: BossPlayerDetectionLogic.cs
//
// author: Jesse Berube
// date: 2020-06-24
//
// summary: This script allows the boss to detect where the player is relative to it. This is used for boss behaviour.
///-------------------------------------------------------------------------------------------------

using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPlayerDetectionLogic : MonoBehaviour
{
    public JimController player;

    public enum DetectionSide
    {
        NotDetected,
        Front,
        Behind,
        Beside
    }

    private DetectionSide _sideDetected;

    private bool _playerDetectedInZone;
    private SharedString _treeSideDetected;
    private SharedFloat _treePlayerDistance;

    // Start is called before the first frame update
    void Start()
    {
        BehaviorTree tree = GetComponent<BehaviorTree>();
        _treeSideDetected = tree.GetVariable("Side Detected") as SharedString;
        _treePlayerDistance = tree.GetVariable("Player Distance") as SharedFloat;

        _treeSideDetected.Value = _sideDetected.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        _treePlayerDistance.Value = Vector3.Distance(player.transform.position, transform.position);
    }

    public void DetectedInZone(BossDetectionZoneLogic.ZoneSide side)
    {
        if(side == BossDetectionZoneLogic.ZoneSide.Front)
        {
            _sideDetected = DetectionSide.Front;
        }
        else
        {
            _sideDetected = DetectionSide.Behind;
        }

        _treeSideDetected.Value = _sideDetected.ToString();

    }

    public void LeftZone()
    {
        //Vector3 relativePosition = transform.InverseTransformPoint(player.transform.position);

        //if (relativePosition.x > 0f)
        //{
        //    _sideDetected = DetectionSide.Right;

        //}
        //else
        //{
        //    _sideDetected = DetectionSide.Left;
        //}

        _sideDetected = DetectionSide.Beside;
        _treeSideDetected.Value = _sideDetected.ToString();

    }
}
