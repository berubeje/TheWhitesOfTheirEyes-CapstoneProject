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

    private RopeAnchorPoint _anchorPoint;
    

    private void Awake()
    {
        _anchorPoint = transform.parent.GetComponentInChildren<RopeAnchorPoint>();

        _anchorPoint.pullStartEvent.AddListener(PullStarted);
    }


    private void PullStarted()
    {
        Collider playerCollider = InputManager.Instance.jimController.GetComponent<Collider>();

        Physics.IgnoreCollision(playerCollider, GetComponent<Collider>(), true);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<GiantDeathHitboxLogic>() != null)
        {
            Transform copiedTranform = this.transform.parent;

            Instantiate(crumblingPillarPrefab, copiedTranform.position, copiedTranform.rotation);
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        _anchorPoint.pullStartEvent.RemoveListener(PullStarted);
    }
}
