using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyTrackFollow : ControllableBase
{
    public JimController jimController;
    public Vector3 followOffset;

    private void Awake()
    {
        //followOffset = transform.position - jimController.transform.position;
    }
    void LateUpdate()
    {
        transform.eulerAngles = new Vector3(0, jimController.transform.eulerAngles.y, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
