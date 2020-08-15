///-------------------------------------------------------------------------------------------------
// file: TargetingConeLogic.cs
//
// author: Jesse Berube
// date: N/A
//
// summary: The targeting cone (more like a zone now) allows the player to see the anchor points and launch the rope at them. The targeting zone will target the anchor point closest to the player inside the zone.
///-------------------------------------------------------------------------------------------------
using UnityEngine;

public class TargetingConeLogic : MonoBehaviour
{
    public JimController player;
    public Material targetedMaterial;
    public PlayerGrapplingHook hookLogic;

    public int playerMaskNum = 8;
    public int targetIgnoreMaskNum = 10;

    // public List<RopeAnchorPoint> _anchorTargets = new List<RopeAnchorPoint>();
    // private int _currentTargetIndex = 0;
    private RopeAnchorPoint _targetedAnchor;
    private Material _anchorPointMaterial;
    private Vector3 _startRot;
    private Vector3 _startSize;

    private void Start()
    {
        _startSize = transform.parent.localScale;
    }

    private void OnEnable()
    {
        if (player == null)
        {
            Debug.LogError("Player is null in the TargetingConeLogic script attached to " + gameObject.name);
        }
    }

    // Get the current target that the targeting zone is highlighting.
    public RopeAnchorPoint GetTarget()
    {
        RopeAnchorPoint target = _targetedAnchor;

        if (target != null)
        {
            Untarget();
        }

        return target;
    }

    // Change the current target from one anchor point to another. 
    private void ChangeTarget(RopeAnchorPoint anchorPoint)
    {
        MeshRenderer mRender = null;

        // If there is an old target, set that target's material back to it's original material
        if (_targetedAnchor != null)
        {
            mRender = _targetedAnchor.GetComponent<MeshRenderer>();

            if (mRender != null)
            {
                mRender.material = _anchorPointMaterial;
            }
        }
        _targetedAnchor = anchorPoint;


        mRender = _targetedAnchor.GetComponent<MeshRenderer>();

        if (mRender != null)
        {
            _anchorPointMaterial = mRender.material;
            mRender.material = targetedMaterial;
        }
    }

    // Check the line of sight of the current target. If there is no line of sight, do not target that anchor point.
    private bool CheckLineOfSight(RopeAnchorPoint anchorPoint)
    {
        // A Layer mask to ignore the player layer and a target ignore layer.
        LayerMask mask = ~(1 << playerMaskNum | 1 << targetIgnoreMaskNum);

        RaycastHit hit;

        if (Physics.Raycast(player.transform.position, (anchorPoint.transform.position - player.transform.position), out hit, Vector3.Distance(player.transform.position, anchorPoint.transform.position), mask))
        {
            if (hit.transform == anchorPoint.transform)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void Untarget()
    {
        MeshRenderer mRender = _targetedAnchor.GetComponent<MeshRenderer>();

        if (mRender != null)
        {
            mRender.material = _anchorPointMaterial;
        }

        _targetedAnchor = null;
    }

    private void OnTriggerStay(Collider other)
    {
        RopeAnchorPoint anchorPoint = other.gameObject.GetComponent<RopeAnchorPoint>();

        if (anchorPoint == null || anchorPoint.gameObject == _targetedAnchor)
        {
            return;
        }

        float newAnchorDistance = Vector3.Distance(player.transform.position, anchorPoint.transform.position);

        // If there is already a target, check the distance between the old target and new target. The target closest to the player will be the new target.
        if (_targetedAnchor != null)
        {
            float currentAnchorDistance = Vector3.Distance(player.transform.position, _targetedAnchor.transform.position);

            if (newAnchorDistance >= currentAnchorDistance)
            {
                return;
            }
            else
            {
                if (CheckLineOfSight(anchorPoint) == true)
                {
                    if (anchorPoint.canAttach == true && hookLogic.targetAnchor != anchorPoint)
                    {
                        ChangeTarget(anchorPoint);
                    }
                }
            }
        }
        else
        {
            if (CheckLineOfSight(anchorPoint) == true)
            {
                if (anchorPoint.canAttach == true && hookLogic.targetAnchor != anchorPoint)
                {
                    ChangeTarget(anchorPoint);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If the target leaves the target zone, remove it from the current target an replace the material.
        RopeAnchorPoint anchorPoint = other.gameObject.GetComponent<RopeAnchorPoint>();

        if (_targetedAnchor != null)
        {
            if (anchorPoint == _targetedAnchor)
            {
                Untarget();
            }
        }
    }
}
