///-------------------------------------------------------------------------------------------------
// file: GiantDeathHitboxLogic.cs
//
// author: Jesse Berube
// date: 2020-06-23
//
// summary: Detects when the pillars collide with the trigger to deal damage.
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
