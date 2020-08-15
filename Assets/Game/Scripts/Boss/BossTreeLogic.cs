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
    public GameObject treeHealParticleEffect;
    public GameObject groundUprootParticleEffect;

    private RopeAnchorPoint _ropeAnchorPoint;
    private RopeAnchorPoint _swingAnchorPoint;


    private void Start()
    {
        if (bossController == null)
        {
            this.enabled = false;
        }

        RopeAnchorPoint[] anchorPoints = transform.parent.GetComponentsInChildren<RopeAnchorPoint>();

        foreach (RopeAnchorPoint anchorPoint in anchorPoints)
        {
            if (anchorPoint.gameObject == this.gameObject)
            {
                _ropeAnchorPoint = anchorPoint;
            }
            else
            {
                _swingAnchorPoint = anchorPoint;
            }
        }

        _swingAnchorPoint.AllowSwing(false);

        _ropeAnchorPoint.pullStartEvent.AddListener(TreeStartedFall);
        _ropeAnchorPoint.pullDoneEvent.AddListener(TreeFallEnded);
        _ropeAnchorPoint.resetDoneEvent.AddListener(HealEnded);

        treeHealParticleEffect.SetActive(false);
        groundUprootParticleEffect.SetActive(false);
    }

    // Tell the anchor point to reverse the pull, as well as play a particle effect
    public void StartHeal()
    {
        _swingAnchorPoint.AllowSwing(false);
        _ropeAnchorPoint.ResetPull(_ropeAnchorPoint.pullTime);

        treeHealParticleEffect.SetActive(true);
    }

    private void HealEnded()
    {
        treeHealParticleEffect.SetActive(false);
    }

    private void TreeStartedFall()
    {
        groundUprootParticleEffect.SetActive(true);
    }

    private void TreeFallEnded()
    {
        bossController.fallenTreeList.Add(this);
        groundUprootParticleEffect.SetActive(false);
        _swingAnchorPoint.AllowSwing(true);
    }
}
