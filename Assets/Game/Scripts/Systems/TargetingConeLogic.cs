using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingConeLogic : MonoBehaviour
{
    public float scaleSpeed;
    public Vector3 maxScale;
    public JimController player;

    private Vector3 _startingScale;
    private RopeAnchorPoint _targetedAnchor;
    private Transform _pivotTransform;
    
    private void Awake()
    {
        _pivotTransform = transform.parent;
        _startingScale = _pivotTransform.localScale;
    }

    private void OnEnable()
    {
        ResetScale();

        if(player == null)
        {
            Debug.LogError("Player is null in the TargetingConeLogic script attached to " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float speed = scaleSpeed * Time.deltaTime;

        Vector3 newScale = new Vector3();
        newScale.x = Mathf.Clamp(_pivotTransform.localScale.x + speed, _startingScale.x, maxScale.x);
        newScale.y = Mathf.Clamp(_pivotTransform.localScale.y + speed, _startingScale.y, maxScale.y);
        newScale.z = Mathf.Clamp(_pivotTransform.localScale.z + speed, _startingScale.z, maxScale.z);

        _pivotTransform.localScale = newScale;
    }

    public void ResetScale()
    {
        _pivotTransform.localScale = _startingScale;
    }

    public RopeAnchorPoint GetTarget()
    {
        //This is done so the same target cannot accidently be regotten without using the targeting cone fully
        RopeAnchorPoint returnAnchor = _targetedAnchor;
        _targetedAnchor = null;
        return returnAnchor;
    }

    private void OnTriggerStay(Collider other)
    {
        RopeAnchorPoint anchorPoint = other.gameObject.GetComponent<RopeAnchorPoint>();

        if(anchorPoint == null || anchorPoint.gameObject == _targetedAnchor)
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
        }

        RaycastHit hit;

        if (Physics.Raycast(player.transform.position, (anchorPoint.transform.position - player.transform.position), out hit, newAnchorDistance))
        {
            if (hit.transform == anchorPoint.transform)
            {
                _targetedAnchor = anchorPoint;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_targetedAnchor != null)
        {
            if (other.gameObject == _targetedAnchor.gameObject)
            {
                _targetedAnchor = null;
            }
        }
    }
}
