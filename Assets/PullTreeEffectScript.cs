//-------------------------------------------------------------------------------------------------
// file: PullTreeEffectScript.cs
//
// author: Jesse Berube
// date: 2020/08/12
//
// summary: The scripts detects when the tree has been pulled down, which then triggers a particle effect.
///-------------------------------------------------------------------------------------------------

using UnityEngine;

public class PullTreeEffectScript : MonoBehaviour
{
    public GameObject groundUprootParticleEffect;

    private RopeAnchorPoint _ropeAnchorPoint;

    // Start is called before the first frame update
    void Start()
    {
         _ropeAnchorPoint = GetComponent<RopeAnchorPoint>();

        _ropeAnchorPoint.pullStartEvent.AddListener(TreeStartedFall);
        _ropeAnchorPoint.pullDoneEvent.AddListener(TreeFalledEnded);
    }

    private void TreeStartedFall()
    {
        groundUprootParticleEffect.SetActive(true);
    }

    private void TreeFalledEnded()
    {
        groundUprootParticleEffect.SetActive(false);
    }
}
