using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingArc : MonoBehaviour
{
    public Transform anchor;
    public float swingArcWidth;
    public float swingArcLimit;
    public float swingSpeed;

    private float _speedMultiplier;
    private int _direction = 1;
    private Vector3 _origin;

    private Rigidbody _rigidbody;
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _origin = new Vector3(
            anchor.position.x,
            anchor.position.y - Vector3.Distance(anchor.position, transform.position),
            anchor.position.z
            );
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(CalculateArcPosition());
    }

    public Vector3 CalculateArcPosition()
    {
        Vector3 pendulumArm = anchor.position - transform.position;
        float angle = Vector3.Angle(Vector3.up, pendulumArm);
        
        if(angle >= swingArcLimit)
        {
            angle = swingArcLimit;
            _direction = _direction == 1 ? -1 : 1; 
        }

        float anglePercent = angle / swingArcLimit;

        _speedMultiplier = _direction * (1.01f - Mathf.Round(anglePercent * 100f) / 100f);

        Vector3 moveAmount = transform.forward * swingSpeed * _speedMultiplier;
        Vector3 newPosition = transform.position + moveAmount;
        newPosition.y = _origin.y;
        newPosition.y += swingArcWidth * (_origin - newPosition).sqrMagnitude;

        return newPosition;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, anchor.position - transform.position, Color.white);
    }
}
