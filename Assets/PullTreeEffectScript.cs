using System.Collections;
using System.Collections.Generic;
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
