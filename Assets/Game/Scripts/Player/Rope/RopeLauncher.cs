using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class RopeLauncher : ControllableBase
{
    public enum LauncherStates
    {
        Ready,
        Fired,
        Latched
    }

    public LauncherStates currentLauncherState = LauncherStates.Ready;
    public GameObject ropeHead;
    public GameObject ropeBase;
    public float ropeRange;
    public float ropeSpeed;
    public float ropeResolution = 0.5f;
    public ObiRopeSection section;
    public Material ropeMaterial;

    private GameObject _launchedRopeHead;
    private float _rightTrigger;

    private ObiRope _rope;
    private ObiRopeExtrudedRenderer _ropeRenderer;
    private ObiRopeBlueprint _blueprint;
    private ObiRopeCursor _cursor;
    private ObiCollider _ropeBaseCollider;

    void Awake()
    {
        // Create both the rope and the solver:	
        _rope = GetComponent<ObiRope>();
        _ropeRenderer = GetComponent<ObiRopeExtrudedRenderer>();
        _ropeRenderer.section = section;
        _ropeRenderer.uvScale = new Vector2(1, 5);
        _ropeRenderer.normalizeV = false;
        _ropeRenderer.uvAnchor = 1;
        _rope.GetComponent<MeshRenderer>().material = ropeMaterial;

        // Setup a blueprint for the rope:
        _blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
        _blueprint.resolution = ropeResolution;

        // Tweak rope parameters:
        _rope.maxBending = 0.02f;

        // Add a cursor to be able to change rope length:
        _cursor = GetComponent<ObiRopeCursor>();
        _cursor.cursorMu = 0;
        _cursor.direction = true;

        _ropeBaseCollider = ropeBase.GetComponent<ObiCollider>();
    }

    void Update()
    {
        if(_launchedRopeHead != null)
        {
            if(Vector3.Distance(transform.position, _launchedRopeHead.transform.position) >= ropeRange)
            {
                Destroy(_launchedRopeHead);
            }
        }
    }

    public override void RightTriggerButton()
    {
        _rightTrigger = Input.GetAxis("Right Trigger Button");

        if (currentLauncherState == LauncherStates.Ready && _rightTrigger >= 1.0f)
        {
            _launchedRopeHead = GameObject.Instantiate(ropeHead, ropeBase.transform.position, transform.rotation);
            _launchedRopeHead.GetComponent<RopeHead>().ropeLauncher = this;
            currentLauncherState = LauncherStates.Fired;
        }
        else if(_rightTrigger <= -1.0f && _launchedRopeHead == null)
        {
            currentLauncherState = LauncherStates.Ready;
        }
    }

    private void OnDestroy()
    {
        DestroyImmediate(_blueprint);
    }

}
