//-------------------------------------------------------------------------------------------------
// file: KillPillarScript.cs
//
// author: Jesse Berube
// date: 2020/07/17
//
// summary: This script is reqired for the pillars to deal damage to the boss. 
///-------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPillarScript : MonoBehaviour
{
    public GameObject crumblingPillarPrefab;
    public float damageDelt = 25.0f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<GiantDeathHitboxLogic>() != null)
        {
            Transform copiedTranform = this.transform;

            Instantiate(crumblingPillarPrefab, copiedTranform.position, copiedTranform.rotation);
            Destroy(this.gameObject);
        }
    }
}
