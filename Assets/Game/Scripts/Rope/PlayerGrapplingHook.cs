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

    [Header("Blueprint Resolution")]
    public float reeledInResolution = 0.0f;
    public float launchedResolution = 0.05f;

    private bool _adjustSwingLength = false;
    private float _ropeMass = 0.1f;
    private ObiRope _rope;
    private ObiRopeExtrudedRenderer _ropeRenderer;
    private MeshRenderer _ropeMeshRenderer;

    private ObiRopeCursor _cursor;
    private JimController _jimController;
    private Animator _jimAnimator;
    private bool _ropeGenerated = false;


    private bool _ropeReturning;

    private RopeState _ropeState;
    public enum RopeState
    {
        Idle,
        Launched,
        Landed,
        Swing,
        Pull
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
        _ropeMeshRenderer = GetComponent<MeshRenderer>();
        _ropeMeshRenderer.material = material;
        _ropeMeshRenderer.enabled = false;

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


        MagicRopeProjectileLogic ropeBaseLogic = character.GetComponent<MagicRopeProjectileLogic>();

        if (ropeBaseLogic != null)
        {
            ropeBaseLogic.SetupGrappleHook(this);
        }

        if (ropeHook != null)
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
        _jimController.anchor = targetAnchor;

        if (targetAnchor != null)
        {
            if (_ropeReturning == true)
            {
                RopeReturned();
            }

            if(targetAnchor.canAttach == false)
            {
                return;
            }
            // Check to see if there is already a projectile. If there is, use it, otherwise create a new one.

            if (ropeHook != null)
            {
                ropeHook.Launch(targetAnchor.transform);
                ropeHook.transform.position = character.transform.position;
            }
            else
            {
                Debug.LogError("Public parameter RopeHook is null in " + gameObject.name);
            }

            currentRopeState = RopeState.Launched;
            
            // Play cobra sound
            AudioManager.Instance.PlaySound("CobraStrike");

            blueprint.resolution = launchedResolution;

            if (_ropeGenerated)
            {
                _rope.GetComponent<MeshRenderer>().enabled = true;
                // Play rope cast
                AudioManager.Instance.PlaySound("RopeCast");
            }
            else
            {
                // The rope is generated on the first throw. We tried to generate it on awake/start, but the results where less than desireable. 
                StartCoroutine(GenerateRope());
                AudioManager.Instance.PlaySound("RopeCast");
            }
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

        //Stop the rope cast sound and play the impact sound
        AudioManager.Instance.StopSound("RopeCast");
        AudioManager.Instance.PlaySound("WoodImpact");

        if (targetAnchor.anchorType == RopeAnchorPoint.AnchorType.Swing)
        {
            currentRopeState = RopeState.Swing;
            _jimAnimator.SetTrigger("swingStart");

        }
        else if (targetAnchor.anchorType == RopeAnchorPoint.AnchorType.Pull)
        {
            currentRopeState = RopeState.Pull;
            //_jimAnimator.SetTrigger("returnToIdle");
        }
    }


    // This is needed to add the control points and pin constraints to the rope blueprint when it is launched.
    public IEnumerator GenerateRope()
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

        if (currentRopeState != PlayerGrapplingHook.RopeState.Idle)
        {
            _rope.GetComponent<MeshRenderer>().enabled = true;
        }
       
        _ropeGenerated = true;

    }

    // Detach the hook from the gameobject it is attatched to and bring it back.
    public void DetachHook()
    {
        // Bring the hook back in a swing or pull state.
        if (currentRopeState == RopeState.Swing || currentRopeState == RopeState.Pull)
        {
            ropeHook.RopeReturn(character.transform);
            _ropeReturning = true;

            if (currentRopeState == RopeState.Swing)
            {
                _jimAnimator.SetTrigger("swingLand");
            }
            currentRopeState = RopeState.Idle;   
        }

        targetAnchor = null;
    }

    // When the rope is returned, set the rope state to idle as well as make the rope go away.
    public void RopeReturned()
    {
        _ropeReturning = false;
        _rope.GetComponent<MeshRenderer>().enabled = false;
        currentRopeState = RopeState.Idle;
        blueprint.resolution = reeledInResolution;
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
