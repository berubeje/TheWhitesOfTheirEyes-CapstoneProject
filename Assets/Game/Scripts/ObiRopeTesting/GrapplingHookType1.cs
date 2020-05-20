﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

/**
 * Sample component that shows how to use Obi Rope to create a grappling hook for a 2.5D game.
 * 95% of the code is the grappling hook logic (user input, scene raycasting, launching, attaching the hook, etc) and parameter setup,
 * to show how to use Obi completely at runtime. This might not be practical for real-world scenarios,
 * but illustrates how to do it.
 *
 * Note that the choice of using actual rope simulation for grapple dynamics is debatable. Usually
 * a simple spring works better both in terms of performance and controllability. 
 *
 * If complex interaction is required with the scene, a purely geometry-based approach (ala Worms ninja rope) can
 * be the right choice under certain circumstances.
 */
public class GrapplingHookType1 : MonoBehaviour
{

    public ObiSolver solver;
    public ObiCollider character;
    public float hookExtendRetractSpeed = 2;
    public Material material;
    public ObiRopeSection section;
    public float ropeMass = 0.1f;
    public GameObject ropeProjectile;
    public bool hookOut = false;

    private ObiRope rope;
    private ObiRopeBlueprint blueprint;
    private ObiRopeExtrudedRenderer ropeRenderer;

    private ObiRopeCursor cursor;

    private RaycastHit hookAttachment;
    private bool attached = false;


    void Awake()
    {

        // Create both the rope and the solver:	
        rope = gameObject.AddComponent<ObiRope>();
        ropeRenderer = gameObject.AddComponent<ObiRopeExtrudedRenderer>();
        ropeRenderer.section = section;
        ropeRenderer.uvScale = new Vector2(1, 5);
        ropeRenderer.normalizeV = false;
        ropeRenderer.uvAnchor = 1;
        rope.GetComponent<MeshRenderer>().material = material;

        // Setup a blueprint for the rope:
        blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
        blueprint.resolution = 0.5f;

        // Tweak rope parameters:
        rope.maxBending = 0.02f;

        // Add a cursor to be able to change rope length:
        cursor = rope.gameObject.AddComponent<ObiRopeCursor>();
        cursor.cursorMu = 0;
        cursor.direction = true;
    }

    private void OnDestroy()
    {
        DestroyImmediate(blueprint);
    }

    /**
	 * Raycast against the scene to see if we can attach the hook to something.
	 */
    private void LaunchHook()
    {

        // Get the mouse position in the scene, in the same XY plane as this object:
        Vector3 mouse = Input.mousePosition;
        Vector3 mouseInScene = Camera.main.ScreenToWorldPoint(mouse);

        // Get a ray from the character to the mouse:
        Ray ray = Camera.main.ScreenPointToRay(mouse);

        // Raycast to see what we hit:
        if (Physics.Raycast(ray, out hookAttachment))
        {
            GameObject projectile = Instantiate(ropeProjectile, character.transform.position, ropeProjectile.transform.rotation);
            MagicRopeProjectileLogic projectileLogic = projectile.GetComponent<MagicRopeProjectileLogic>();
            projectileLogic.SetupProjectile(hookAttachment.point, this, hookAttachment.collider.gameObject);

        }

    }

    public void TargetReached()
    {
        StartCoroutine(AttachHook());
    }

    private IEnumerator AttachHook()
    {
        yield return 0;
        Vector3 localHit = rope.transform.InverseTransformPoint(hookAttachment.point);

        // Procedurally generate the rope path (a simple straight line):
        blueprint.path.Clear();
        blueprint.path.AddControlPoint(Vector3.zero, -localHit.normalized, localHit.normalized, Vector3.up, ropeMass, 0.1f, 1, 1, Color.white, "Hook start");
        blueprint.path.AddControlPoint(localHit, -localHit.normalized, localHit.normalized, Vector3.up, ropeMass, 0.1f, 1, 1, Color.white, "Hook end");
        blueprint.path.FlushEvents();

        // Generate the particle representation of the rope (wait until it has finished):
        yield return blueprint.Generate();

        // Pin both ends of the rope (this enables two-way interaction between character and rope):
        var pinConstraints = blueprint.GetConstraintsByType(Oni.ConstraintType.Pin) as ObiConstraints<ObiPinConstraintsBatch>;
        var batch = pinConstraints.batches[0];
        batch.AddConstraint(0, character, transform.localPosition, Quaternion.identity);
        batch.AddConstraint(blueprint.activeParticleCount - 1, hookAttachment.collider.GetComponent<ObiColliderBase>(),
                                                          hookAttachment.collider.transform.InverseTransformPoint(hookAttachment.point), Quaternion.identity);
        batch.activeConstraintCount = 2;
        
        // Set the blueprint (this adds particles/constraints to the solver and starts simulating them).
        rope.ropeBlueprint = blueprint;
        rope.GetComponent<MeshRenderer>().enabled = true;
        hookOut = true;
    }

    private void DetachHook()
    {
        // Set the rope blueprint to null (automatically removes the previous blueprint from the solver, if any).
        rope.ropeBlueprint = null;
        rope.GetComponent<MeshRenderer>().enabled = false;
        hookOut = false;
    }


    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (!rope.isLoaded)
                LaunchHook();
            else
                DetachHook();
        }

        if (rope.isLoaded)
        {
            if (Input.GetKey(KeyCode.I))
            {
                cursor.ChangeLength(rope.restLength - hookExtendRetractSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.O))
            {
                cursor.ChangeLength(rope.restLength + hookExtendRetractSpeed * Time.deltaTime);
            }
        }
    }
}