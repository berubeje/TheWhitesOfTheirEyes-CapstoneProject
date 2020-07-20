///-------------------------------------------------------------------------------------------------
// file: GiantDeathHitboxLogic.cs
//
// author: Jesse Berube
// date: 2020-06-23
//
// summary: A script to kill the boss in aplha build when a certain thing drops on the giants head.
///-------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

public class GiantDeathHitboxLogic : MonoBehaviour
{

    private BossController _bossController;

    private void Awake()
    {
        _bossController = GetComponentInParent<BossController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        KillPillarScript killPillarScript = other.gameObject.GetComponent<KillPillarScript>();

        if (killPillarScript != null )
        {
            _bossController.bossHealth -= killPillarScript.damageDelt;
            Destroy(killPillarScript.gameObject);

            if(_bossController.bossHealth <=  0.0f)
            {
                Destroy(this.gameObject);
            }
            return;
        }
    }
}
