using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyTrackFollow : ControllableBase
{
    public JimController jimController;
    public Vector3 followOffset;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
