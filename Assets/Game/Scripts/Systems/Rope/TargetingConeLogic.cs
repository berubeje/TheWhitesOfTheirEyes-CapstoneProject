using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingConeLogic : MonoBehaviour
{
    public JimController player;
    public Material targetedMaterial;
    public Material anchorPointMaterial;
    public Vector3 tieTargetSize;

    public int playerMaskNum = 8;
    public int targetIgnoreMaskNum = 10;

    //public List<RopeAnchorPoint> _anchorTargets = new List<RopeAnchorPoint>();
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


    public RopeAnchorPoint GetTarget()
    {
        RopeAnchorPoint returnAnchor = _targetedAnchor;
        return returnAnchor;
    }

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

    private void ChangeTarget(RopeAnchorPoint anchorPoint)
    {
        MeshRenderer mRender = null;
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

    private bool CheckLineOfSight(RopeAnchorPoint anchorPoint)
    {

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
                    ChangeTarget(anchorPoint);
                }
            }
        }
        else
        {
            if (CheckLineOfSight(anchorPoint) == true)
            {
                ChangeTarget(anchorPoint);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
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
