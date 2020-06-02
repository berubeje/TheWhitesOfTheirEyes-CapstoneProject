using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class PlayerGrapplingHook : MonoBehaviour
{
    public ObiSolver solver;
    public ObiCollider character;
    public Material material;
    public ObiRopeSection section;

    public GameObject ropeProjectile;
    public RopeAnchorPoint targetAnchor;
    public TargetingConeLogic targetCone;
    public bool mouseTargeting = false;

    private float _ropeMass = 0.1f;
    private float _resolution = 0.5f;
    private ObiRope _rope;
    private ObiRopeBlueprint _blueprint;
    private ObiRopeExtrudedRenderer _ropeRenderer;

    private ObiRopeCursor _cursor;

    private RaycastHit _hookAttachment;
    private bool _attached = false;

    private GameObject _launchedProjectile;
    private JimController _jimController;
    private Animator _jimAnimator;
            

    public RopeState ropeState;
    public enum RopeState
    {
        Idle,
        Launched,
        Landed,
        Swing,
        Pull
    }



    void Awake()
    {
        
        // Create both the rope and the solver:	
        _rope = gameObject.AddComponent<ObiRope>();
        _ropeRenderer = gameObject.AddComponent<ObiRopeExtrudedRenderer>();
        _ropeRenderer.section = section;
        _ropeRenderer.uvScale = new Vector2(1, 5);
        _ropeRenderer.normalizeV = false;
        _ropeRenderer.uvAnchor = 1;
        _rope.GetComponent<MeshRenderer>().material = material;

        // Setup a blueprint for the rope:
        _blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
        _blueprint.resolution = _resolution;

        // Tweak rope parameters:
        _rope.maxBending = 0.02f;

        // Add a cursor to be able to change rope length:
        _cursor = _rope.gameObject.AddComponent<ObiRopeCursor>();
        _cursor.cursorMu = 0;
        _cursor.direction = true;

        //Grab a reference to the player controller and animator
        _jimController = GetComponentInParent<JimController>();
        if(_jimController == null)
        {
            Debug.LogError("No Jim Controller found in any parent.");
        }

        _jimAnimator = GetComponentInParent<Animator>();
        if (_jimAnimator == null)
        {
            Debug.LogError("No animator found in any parent.");
        }
    }

    private void OnDestroy()
    {
        DestroyImmediate(_blueprint);
    }

    public void ActivateTargeting()
    {
        targetCone.gameObject.SetActive(true);
    }

    /**
	 * Raycast against the scene to see if we can attach the hook to something.
	 */
    public void LaunchHook()
    {
        if (ropeState == RopeState.Launched)
        {
            return;
        }

        if(mouseTargeting)
        {
            // Get the mouse position in the scene, in the same XY plane as this object:
            Vector3 mouse = Input.mousePosition;
            Vector3 mouseInScene = Camera.main.ScreenToWorldPoint(mouse);

            // Get a ray from the character to the mouse:
            Ray ray = Camera.main.ScreenPointToRay(mouse);

            // Raycast to see what we hit:
            if (Physics.Raycast(ray, out _hookAttachment))
            {
                targetAnchor = _hookAttachment.transform.GetComponent<RopeAnchorPoint>();
            }
        }
        else
        {
            targetAnchor = targetCone.GetTarget();
            targetCone.gameObject.SetActive(false);
        }



        if (targetAnchor != null)
        {
            _launchedProjectile = Instantiate(ropeProjectile, character.transform.position, ropeProjectile.transform.rotation);
            MagicRopeProjectileLogic projectileLogic = _launchedProjectile.GetComponent<MagicRopeProjectileLogic>();
            projectileLogic.SetupProjectile(targetAnchor.transform.position, this, targetAnchor.gameObject);
            ropeState = RopeState.Launched;
            StartCoroutine(AttachHook());
        }
    }

    public void TargetReached()
    {
        // _cursor.ChangeLength(0.0f);

        _attached = true;
        ropeState = RopeState.Landed;


        _rope.ropeBlueprint = null;
        StartCoroutine(AttachHook());

        if(targetAnchor.anchorType == RopeAnchorPoint.AnchorType.Swing)
        {
            ropeState = RopeState.Swing;
            _jimController.anchor = targetAnchor.transform;
            _jimAnimator.SetTrigger("swingStart");

        }
        else
        {
            ropeState = RopeState.Pull;
        }
    }

    private IEnumerator AttachHook()
    {
        yield return 0;
        Vector3 localHit = _rope.transform.InverseTransformPoint(targetAnchor.transform.position);

        // Procedurally generate the rope path (a simple straight line):
        _blueprint.path.Clear();
        _blueprint.path.AddControlPoint(Vector3.zero, -localHit.normalized, localHit.normalized, Vector3.up, _ropeMass, 0.1f, 1, 1, Color.white, "Hook start");
        _blueprint.path.AddControlPoint(localHit, -localHit.normalized, localHit.normalized, Vector3.up, _ropeMass, 0.1f, 1, 1, Color.white, "Hook end");
        _blueprint.path.FlushEvents();

        // Generate the particle representation of the rope (wait until it has finished):
        yield return _blueprint.Generate();

        // Pin both ends of the rope (this enables two-way interaction between character and rope):
        var pinConstraints = _blueprint.GetConstraintsByType(Oni.ConstraintType.Pin) as ObiConstraints<ObiPinConstraintsBatch>;
        var batch = pinConstraints.batches[0];
        batch.AddConstraint(0, character, Vector3.zero, Quaternion.identity);
        batch.AddConstraint(_blueprint.activeParticleCount - 1, _launchedProjectile.GetComponent<ObiColliderBase>(),
                                                          Vector3.zero, Quaternion.identity);
        batch.activeConstraintCount = 2;

        // Set the blueprint (this adds particles/constraints to the solver and starts simulating them).
        _rope.ropeBlueprint = _blueprint;
        _rope.GetComponent<MeshRenderer>().enabled = true;

    }

    public void DetachHook()
    {
        // Set the rope blueprint to null (automatically removes the previous blueprint from the solver, if any).
        _rope.ropeBlueprint = null;
        _rope.GetComponent<MeshRenderer>().enabled = false;
        Destroy(_launchedProjectile);
        _attached = false;

        if(ropeState == RopeState.Swing)
        {
            _jimAnimator.SetTrigger("swingLand");
            _jimAnimator.SetBool("swingIdle", false);
        }


        ropeState = RopeState.Idle;
        targetAnchor = null;
    }

    public float GetRopeLength()
    {
        return _rope.CalculateLength();
    }
    public float GetDistanceBetweenEnds()
    {
        return Vector3.Distance(character.transform.position, targetAnchor.transform.position);
    }

    public float CalculateStrain()
    {
        return _rope.CalculateLength() / _rope.restLength;
    }

    public void AdjustRopeLength(float length)
    {
        if(_rope.ropeBlueprint == null)
        {
            return;
        }

        _cursor.ChangeLength(length);
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!_rope.isLoaded)
                LaunchHook();
            else
                DetachHook();
        }
    }

    private void FixedUpdate()
    {
        if (ropeState == RopeState.Launched && _rope.isLoaded)
        {
            AdjustRopeLength(Vector3.Distance(character.transform.position, _launchedProjectile.transform.position));
        }
    }
}
