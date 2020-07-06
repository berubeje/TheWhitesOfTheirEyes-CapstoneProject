///-------------------------------------------------------------------------------------------------
// file: PlayerGrapplingHook.cs
//
// author: Jesse Berube
// date: N/A
//
// summary: Used to generate the rope as well as manipulate to rope itself
///-------------------------------------------------------------------------------------------------

using System.Collections;
using UnityEngine;
using Obi;

public class PlayerGrapplingHook : MonoBehaviour
{
    public ObiRopeBlueprint blueprint;
    public ObiSolver solver;
    public ObiCollider character;
    public MagicRopeProjectileLogic ropeHook;
    public Material material;
    public ObiRopeSection section;

    public GameObject ropeProjectile;
    public RopeAnchorPoint targetAnchor;
    public RopeAnchorPoint baseTargetAnchor;

    public TargetingConeLogic targetCone;

    [Header("Swing Strain")]
    public float currentRopeLengthOffset;

    private bool _adjustSwingLength = false;
    private float _ropeMass = 0.1f;
    private ObiRope _rope;
    private ObiRopeExtrudedRenderer _ropeRenderer;

    private ObiRopeCursor _cursor;
    private RaycastHit _hookAttachment;
    private JimController _jimController;
    private Animator _jimAnimator;

    private Vector3 _startingBasePosition;
    private Transform _startingBaseParent;

    private bool _ropeReturning;

    private RopeState _ropeState;
    public enum RopeState
    {
        Idle,
        Launched,
        Landed,
        Swing,
        Pull,
        OneEndTied,
        Tied
    }

    public RopeState currentRopeState
    {
        get { return _ropeState; }
        set
        {
            if (_ropeState == value)
            {
                return;
            }
            _ropeState = value;

            if (OnRopeStateChange != null)
            {
                OnRopeStateChange(_ropeState);
            }
        }
    }
    public delegate void OnRopeStateChangeDelegate(RopeState state);
    public event OnRopeStateChangeDelegate OnRopeStateChange;

    void Awake()
    {

        // Create both the rope and the solver.
        _rope = gameObject.AddComponent<ObiRope>();
        _ropeRenderer = gameObject.AddComponent<ObiRopeExtrudedRenderer>();
        _ropeRenderer.section = section;
        _ropeRenderer.uvScale = new Vector2(1, 5);
        _ropeRenderer.normalizeV = false;
        _ropeRenderer.uvAnchor = 1;
        _rope.GetComponent<MeshRenderer>().material = material;

        // Tweak rope parameters.
        _rope.maxBending = 0.02f;

        // Add a cursor to be able to change rope length.
        _cursor = _rope.gameObject.AddComponent<ObiRopeCursor>();
        _cursor.cursorMu = 0;
        _cursor.direction = true;

        // Grab a reference to the player controller and animator.
        _jimController = GetComponentInParent<JimController>();
        if (_jimController == null)
        {
            Debug.LogError("No Jim Controller found in any parent.");
        }

        _jimAnimator = GetComponentInParent<Animator>();
        if (_jimAnimator == null)
        {
            Debug.LogError("No animator found in any parent.");
        }

        _startingBasePosition = character.transform.localPosition;
        _startingBaseParent = character.transform.parent;

        MagicRopeProjectileLogic ropeBaseLogic = character.GetComponent<MagicRopeProjectileLogic>();

        if (ropeBaseLogic != null)
        {
            ropeBaseLogic.SetupGrappleHook(this);
        }

        if(ropeHook != null)
        {
            ropeHook.SetupGrappleHook(this);
        }
        else
        {
            Debug.LogError("Public parameter RopeHook is null in " + this.gameObject.name);
        }
    }

    // Attempt to launch the rope at the target the target zone has found.
    public void LaunchHook()
    {
        if (currentRopeState == RopeState.Launched)
        {
            return;
        }

        targetAnchor = targetCone.GetTarget();

        if (targetAnchor != null)
        {
            if (_ropeReturning == true)
            {
                RopeReturned();
            }

            // Check to see if there is already a projectile. If there is, use it, otherwise create a new one.

            if (ropeHook != null)
            {
                ropeHook.Relaunch(targetAnchor.transform);
                ropeHook.transform.position = character.transform.position;
            }
            else
            {
                Debug.LogError("Public parameter RopeHook is null in " + this.gameObject.name);
            }

            currentRopeState = RopeState.Launched;
            StartCoroutine(AttachHook());

        }
    }

    // Pull the rope back if the playe cancels the launch.
    public void CancelLaunch()
    {
        ropeHook.RopeReturn(character.transform);
        _ropeReturning = true;
        targetAnchor = null;
        currentRopeState = RopeState.Idle;
    }

    // Once the rope reaches its target, change the state of the rope depending on the anchor point
    public void TargetReached()
    {
        currentRopeState = RopeState.Landed;

        if (targetAnchor.anchorType == RopeAnchorPoint.AnchorType.Swing)
        {
            currentRopeState = RopeState.Swing;
            _jimController.anchor = targetAnchor.transform;
            _jimAnimator.SetTrigger("swingStart");

        }
        else if (targetAnchor.anchorType == RopeAnchorPoint.AnchorType.Pull)
        {
            currentRopeState = RopeState.Pull;
        }
        else
        {
            currentRopeState = RopeState.OneEndTied;
            targetCone.TieSizeToggle(true);
        }

    }

    // The logic to tie the rope to another gameobject, as well as set the rope state to 'Tied' 
    public void TieRope()
    {
        if (currentRopeState != RopeState.OneEndTied)
        {
            return;
        }

        RopeAnchorPoint tieTarget = targetCone.GetTarget();

        if (tieTarget != null && tieTarget != targetAnchor)
        {
            character.transform.position = tieTarget.transform.position;
            character.transform.parent = tieTarget.transform;

            baseTargetAnchor = tieTarget;
            if (targetAnchor.GetComponentInParent<RockSwinging>() != null)
            {
                targetAnchor.GetComponentInParent<RockSwinging>().currentBaseAnchor = baseTargetAnchor;
                targetAnchor.GetComponentInParent<RockSwinging>().currentHeadAnchor = targetAnchor;
            }
            currentRopeState = RopeState.Tied;
        }
    }

    // This is needed to add the control points and pin constraints to the rope blueprint when it is launched.
    private IEnumerator AttachHook()
    {
        yield return 0;
        Vector3 localHit = _rope.transform.InverseTransformPoint(ropeHook.transform.position);

        // Procedurally generate the rope path.
        blueprint.path.Clear();
        blueprint.path.AddControlPoint(Vector3.zero, -localHit.normalized, localHit.normalized, Vector3.up, _ropeMass, 0.1f, 1, 1, Color.white, "Hook start");
        blueprint.path.AddControlPoint(localHit, -localHit.normalized, localHit.normalized, Vector3.up, _ropeMass, 0.1f, 1, 1, Color.white, "Hook end");
        blueprint.path.FlushEvents();

        yield return blueprint.Generate();

        // Pin both ends of the rope.
        var pinConstraints = blueprint.GetConstraintsByType(Oni.ConstraintType.Pin) as ObiConstraints<ObiPinConstraintsBatch>;
        var batch = pinConstraints.batches[0];
        batch.AddConstraint(0, character, Vector3.zero, Quaternion.identity);
        batch.AddConstraint(blueprint.activeParticleCount - 1, ropeHook.GetComponent<ObiColliderBase>(),
                                                          Vector3.zero, Quaternion.identity);
        batch.activeConstraintCount = 2;

        // Set the blueprint.
        _rope.ropeBlueprint = blueprint;
        _rope.GetComponent<MeshRenderer>().enabled = true;
    }

    // Reset the targets for the rockswing
    private void DetachSwingRock()
    {
        RockSwinging r = targetAnchor.GetComponentInParent<RockSwinging>();
        if (r != null)
        {
            r.currentBaseAnchor = null;
            r.currentHeadAnchor = null;
        }
        baseTargetAnchor = null;
        targetAnchor = null;
    }

    // Detach the hook from the gameobject it is attatched to and bring it back.
    public void DetachHook()
    {

        // Bring the hook back in a swing or pull state.
        if (currentRopeState == RopeState.Swing || currentRopeState == RopeState.Pull || currentRopeState == RopeState.OneEndTied)
        {
            ropeHook.RopeReturn(character.transform);
            DetachSwingRock();
            _ropeReturning = true;

            if (currentRopeState == RopeState.Swing)
            {
                _jimAnimator.SetTrigger("swingLand");
            }

            currentRopeState = RopeState.Idle;

        }
        // Bring the base of the rope back in a tied state.
        else if (currentRopeState == RopeState.Tied)
        {
            MagicRopeProjectileLogic ropeBaseLogic = character.GetComponent<MagicRopeProjectileLogic>();

            ropeHook.InstantReturn();

            if (ropeBaseLogic != null)
            {
                ropeBaseLogic.RopeReturn();
                DetachSwingRock();
                _rope.ropeBlueprint = null;
                _rope.GetComponent<MeshRenderer>().enabled = false;
            }
            baseTargetAnchor = null;

        }

        targetCone.TieSizeToggle(false);
        targetAnchor = null;
    }

    // When the rope is returned, set the rope state to idle as well as make the rope go away.
    public void RopeReturned()
    {
        if (currentRopeState == RopeState.Tied)
        {
            character.transform.parent = _startingBaseParent;
            character.transform.localPosition = _startingBasePosition;
        }

        _ropeReturning = false;
        _rope.ropeBlueprint = null;
        _rope.GetComponent<MeshRenderer>().enabled = false;
        currentRopeState = RopeState.Idle;
    }

    // Calculate the length of the rope.
    public float GetRopeLength()
    {
        return _rope.CalculateLength();
    }

    // Get the distance between the base of the rope and the targetAnchor the rope is attatched to.
    public float GetDistanceBetweenEnds()
    {
        return Vector3.Distance(character.transform.position, targetAnchor.transform.position);
    }

    // Calculate how much the rope is being strained. 
    public float CalculateStrain()
    {
        return _rope.CalculateLength() / _rope.restLength;
    }

    // Allowes the rope length to be adjusted to be longer or shorter.
    public void AdjustRopeLength(float length)
    {
        if (_rope.ropeBlueprint == null || length < 0)
        {
            return;
        }

        _cursor.ChangeLength(length);
    }

    private void FixedUpdate()
    {
        // Makes the rope grow or shrink depending if it is being launched or is returning.
        if ((currentRopeState == RopeState.Launched || _ropeReturning) && _rope.isLoaded)
        {
            AdjustRopeLength(Vector3.Distance(character.transform.position, ropeHook.transform.position));
        }

        // Adjust the rope so it will grow or shrink with an offset while swinging.
        if (currentRopeState == RopeState.Swing)
        {
            AdjustRopeLength(Vector3.Distance(character.transform.position, targetAnchor.transform.position) + currentRopeLengthOffset);
        }
    }
}
