using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSwinging : IObstacle
{
    public float swingRadius;
    public RopeAnchorPoint baseAnchor;
    public RopeAnchorPoint headAnchor;

    private PlayerGrapplingHook grapplingHook;

    private void Start()
    {
        grapplingHook = FindObjectOfType<PlayerGrapplingHook>();    
    }

    private void Update()
    {
    }

    public override void ResetObstacle()
    {
    }

    public override void UnresetObstacle()
    {
    }
}
