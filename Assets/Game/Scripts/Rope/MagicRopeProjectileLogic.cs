///-------------------------------------------------------------------------------------------------
// file: MagicRopeProjectileLogic.cs
//
// author: Jesse Berube
// date: N/A
//
// summary: This script is what pushed the magic rope projectile towards the anchor point, as well as bringing the projectile back to the player when they reel in the rope.
///-------------------------------------------------------------------------------------------------

using UnityEngine;

public class MagicRopeProjectileLogic : MonoBehaviour
{
    public float projectileSpeed;
    public Transform ropeBaseReturnTransform;

    private PlayerGrapplingHook _grapplingHookLogic;
    private Transform _targetTransform;
    private bool _targetReached = true;
    private bool _returning;
    private bool _dontDestroy;
    private MeshRenderer _meshRenderer;


    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        MoveToTarget();
    }

    // Moves the projectile to the target once a target is given. It just moves towards the target using Vector3.MoveTowards. 
    private void MoveToTarget()
    {
        if (_targetReached == false)
        {
            float step = projectileSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, _targetTransform.position, step);


            // Check to see if the projectile is close enough to the target. If it is, let the grapplehooklogic know and take action depending on if the rope is returning or not.
            if (Vector3.Distance(transform.position, _targetTransform.position) < 0.001f)
            {
                if (!_returning)
                {
                    _grapplingHookLogic.TargetReached();
                    _targetReached = true;
                    transform.parent = _targetTransform;
                }
                else
                {
                    _grapplingHookLogic.RopeReturned();

                    if (_dontDestroy == false)
                    {
                        _meshRenderer.enabled = false;
                    }

                    if(_targetTransform == ropeBaseReturnTransform)
                    {
                        _returning = false;
                        _targetReached = true;
                    }
                }
            }

        }
    }

    // Tell the rope to start returning to the base of the rope.
    public void RopeReturn(Transform ropeBase)
    {
        transform.parent = null;
        _targetTransform = ropeBase;
        _targetReached = false;
        _returning = true;
    }

    // Launch the projectile at a target.
    public void Relaunch(Transform targetTransform)
    {
        _targetTransform = targetTransform;
        _returning = false;
        _targetReached = false;
        _meshRenderer.enabled = true;
    }

    // This is for the rope base to come back to the player's hand after a tie action.
    public void RopeReturn()
    {
        _targetTransform = ropeBaseReturnTransform;
        _targetReached = false;
        _returning = true;
        _dontDestroy = true;
    }

    public void InstantReturn()
    {
        transform.parent = null;
        transform.position = ropeBaseReturnTransform.position;
        _meshRenderer.enabled = false;
        _targetReached = true;
        _returning = false;
    }

    // Setup the magic projectile to have a reference to the grappling hook.
    public void SetupGrappleHook(PlayerGrapplingHook grappling)
    {
        _grapplingHookLogic = grappling;
    }
}
