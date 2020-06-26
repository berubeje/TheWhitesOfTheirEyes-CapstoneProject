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
    public Material anchorPointMaterial;
    public Vector3 tieTargetSize;
    public PlayerGrapplingHook hookLogic;

    public int playerMaskNum = 8;
    public int targetIgnoreMaskNum = 10;

    // public List<RopeAnchorPoint> _anchorTargets = new List<RopeAnchorPoint>();
    // private int _currentTargetIndex = 0;
    private RopeAnchorPoint _targetedAnchor;
    private Vector3 _startRot;
    private Vector3 _startSize;

    private void Start()
    {
        _startSize = transform.parent.localScale;
        //_startRot = transform.eulerAngles;
    }

    private void LateUpdate()
    {
        //_startRot.y = transform.eulerAngles.y; // keep current rotation about Y
       // _startRot.z = transform.eulerAngles.z; // keep current rotation about Y
        //transform.rotation = Quaternion.Euler(_startRot); // restore original rotation with new Y
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
        return _targetedAnchor;
    }

    // When the rope is attatched to a something, the targeting zone will shrink so the range to tie the rope to an object is different from the range to launch to an anchor point.
    public void TieSizeToggle(bool set)
    {
        if(set)
        {
            transform.parent.localScale = tieTargetSize;
        }
        else
        {
            transform.parent.localScale = _startSize;
        }
    }

    // Allows the user to cycle targets inside the target zone. Not in use at the moment.

    //public void NextTarget()
    //{
    //    int startingIndex = _currentTargetIndex;
    //    _currentTargetIndex++;

    //    if(_currentTargetIndex + 1 > _anchorTargets.Count)
    //    {
    //        _currentTargetIndex = 0;
    //    }

    //    while (CheckLineOfSight(_anchorTargets[_currentTargetIndex]) == false)
    //    {
    //        _currentTargetIndex++;

    //        if (_currentTargetIndex == startingIndex)
    //        {
    //            break;
    //        }

    //        if (_currentTargetIndex + 1 > _anchorTargets.Count)
    //        {
    //            _currentTargetIndex = 0;
    //        }

    //    }

    //    if (_anchorTargets[_currentTargetIndex] != null)
    //    {
    //        ChangeTarget();
    //    }

    //}

    //public void PreviousTarget()
    //{
    //    int startingIndex = _currentTargetIndex;
    //    _currentTargetIndex--;

    //    if (_currentTargetIndex < 0)
    //    {
    //        _currentTargetIndex = _anchorTargets.Count - 1;
    //    }

    //    while (CheckLineOfSight(_anchorTargets[_currentTargetIndex]) == false)
    //    {
    //        _currentTargetIndex--;

    //        if (_currentTargetIndex == startingIndex)
    //        {
    //            break;
    //        }

    //        if (_currentTargetIndex < 0)
    //        {
    //            _currentTargetIndex = _anchorTargets.Count - 1;
    //        }

    //    }

    //    if (_anchorTargets[_currentTargetIndex] != null)
    //    {
    //        ChangeTarget();
    //    }
    //}

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
                mRender.material = anchorPointMaterial;
            }
        }

        _targetedAnchor = anchorPoint;

        mRender = _targetedAnchor.GetComponent<MeshRenderer>();
        mRender.material = targetedMaterial;
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
                    if (anchorPoint.cantAttach == false && hookLogic.targetAnchor != anchorPoint)
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
                if (anchorPoint.cantAttach == false && hookLogic.targetAnchor != anchorPoint)
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
                MeshRenderer mRender = _targetedAnchor.GetComponent<MeshRenderer>();

                if (mRender != null)
                {
                    mRender.material = anchorPointMaterial;
                }

                _targetedAnchor = null;
            }
        }
    }
}
