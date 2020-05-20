using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    public Transform followTarget;
    public float followSpeed;
    public Vector3 offset;

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, followTarget.position + offset, Time.deltaTime * followSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
