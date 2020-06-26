///-------------------------------------------------------------------------------------------------
// file: BossDetectionZoneLogic.cs
//
// author: Jesse Berube
// date: 2020-06-24
//
// summary: This script allows the boss to detect where the player is relative to it. This is used for boss behaviour.
///-------------------------------------------------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDetectionZoneLogic : MonoBehaviour
{
    public enum ZoneSide
    {
        Front,
        Back,
        ArenaCheck
    }

    public ZoneSide zoneSide;

    private BossPlayerDetectionLogic _bossDetectionLogic;

    private void Awake()
    {
        _bossDetectionLogic = GetComponentInParent<BossPlayerDetectionLogic>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<JimController>() != null)
        {
            if(zoneSide == ZoneSide.ArenaCheck)
            {

            }
            else
            {
                _bossDetectionLogic.DetectedInZone(zoneSide);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<JimController>() != null)
        {
            if (zoneSide == ZoneSide.ArenaCheck)
            {

            }
            else
            {
                _bossDetectionLogic.LeftZone();
            }
        }
    }
}
