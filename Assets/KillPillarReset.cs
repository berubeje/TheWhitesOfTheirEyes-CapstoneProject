using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPillarReset : IObstacle
{
    private KillPillarScript _killPillar;
    private RopeAnchorPoint _ropeAnchor;

    private Vector3 _localPosition;
    private Quaternion _localRotation;
    // Start is called before the first frame update

    private void Awake()
    {
        _killPillar = GetComponentInChildren<KillPillarScript>();
        _ropeAnchor = GetComponentInChildren<RopeAnchorPoint>();

        _localRotation = transform.localRotation;
        _localPosition = transform.localPosition;
    }

    

    public override void ResetObstacle()
    {
        _killPillar.gameObject.SetActive(true);

        _ropeAnchor.canAttach = true;
        transform.localPosition = _localPosition;
        transform.localRotation = _localRotation;

        isTriggered = false;
    }

    public override void UnresetObstacle()
    {
        throw new System.NotImplementedException();
    }

}
