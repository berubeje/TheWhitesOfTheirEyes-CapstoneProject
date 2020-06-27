using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSwinging : IObstacle
{
    public GameObject door;
    public float swingRadius;
    public float reelInSpeed;
    public float swingSpeed;

    [Space]
    public float rockLaunchMagnitude;
    public float rockLaunchDistance;
    public float launchSpeed;
    public float launchAcceleration;

    public SplineRoute splineRoute;
    public RopeAnchorPoint currentBaseAnchor;

    // Set up event for when the base of the rope is attached to something
    public RopeAnchorPoint currentHeadAnchor
    {
        get { return _headAnchor; }
        set
        {
            if (_headAnchor == value)
            {
                return;
            }
            _headAnchor = value;

            if (OnRopeTie != null && _headAnchor != null)
            {
                OnRopeTie(currentBaseAnchor);
            }

            if(_headAnchor == null)
            {
                LaunchRock();
            }
        }
    }
    public delegate void OnRopeTieDelegate(RopeAnchorPoint anchorPoint);
    public event OnRopeTieDelegate OnRopeTie;

    private RopeAnchorPoint _headAnchor;
    private Rigidbody _rigidbody;

    private Vector3 _initialBoulderPosition;
    private Vector3 _initialDoorPosition;
    private Quaternion _initialBoulderRotation;
    private Quaternion _initialDoorRotation;

    private float _interpolant;
    private bool _isRopeTied = false;
    private bool _isReeledIn = false;
    private bool _isLaunched = false;
    private Vector3 _reelInDestination;
    private Vector3 _forwardLimitVector;
    private Vector3 _backwardLimitVector;
    private Vector3 _pendulumArm;
    private Vector3 _lookDirection;
    private Vector3 _initialPosition;
    private Vector3 _swingForward;
    private Quaternion _initialRotation;
    private Quaternion _targetRotation;

    private Vector3 _p0;
    private Vector3 _p1;
    private Vector3 _p2;
    private Vector3 _p3;
    private float _t;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _initialDoorPosition = door.transform.position;
        _initialBoulderPosition = transform.position;
        _initialDoorRotation = door.transform.rotation;
        _initialBoulderRotation = transform.rotation;

        OnRopeTie += OnRopeTied;
    }

    private void FixedUpdate()
    {
        if (_isRopeTied)
        {
            Vector3 targetPosition;
            Quaternion targetRotation;
            switch (_isReeledIn)
            {
                case false:
                    targetPosition = Vector3.Lerp(_initialPosition, _reelInDestination, _interpolant);
                    targetRotation = Quaternion.Lerp(_initialRotation, _targetRotation, _interpolant);

                    _rigidbody.MovePosition(targetPosition);
                    _rigidbody.MoveRotation(targetRotation);
                    _interpolant += reelInSpeed * Time.deltaTime;

                    if (_interpolant >= 1.0f)
                    {
                        _interpolant = 0.0f;
                        _isReeledIn = true;
                    }
                    break;

                case true:

                    _pendulumArm = currentBaseAnchor.transform.position - currentHeadAnchor.transform.position;
                    targetPosition = currentBaseAnchor.transform.position + Vector3.Slerp(_backwardLimitVector, _forwardLimitVector, _interpolant);
                    targetRotation = Quaternion.LookRotation(_pendulumArm);

                    _rigidbody.MovePosition(targetPosition);
                    _rigidbody.MoveRotation(targetRotation);
                    _interpolant += swingSpeed * Time.deltaTime;

                    if (_interpolant >= 1.0f)
                    {
                        swingSpeed *= -1;
                    }
                    else if (_interpolant <= 0.0f)
                    {
                        swingSpeed *= -1;
                    }
                    
                    break;
            }
        }
        else if (_isLaunched && _isReeledIn)
        {
            FollowLaunchSpline();
        }
    }

    private void OnRopeTied(RopeAnchorPoint anchorPoint)
    {
        // Start swing if object is tied to a swing point
        if(currentBaseAnchor.anchorType == RopeAnchorPoint.AnchorType.Swing)
        {
            // Set the backward limit of the swing
            _backwardLimitVector = currentHeadAnchor.transform.position - currentBaseAnchor.transform.position;

            // Direction to look is negative of backward limit
            _lookDirection = currentBaseAnchor.transform.position - currentHeadAnchor.transform.position;
            _targetRotation = Quaternion.LookRotation(_lookDirection); 

            // The pendulum starts at the back
            _pendulumArm = _backwardLimitVector;

            // Normalize backward vector and multiply it by swing radius
            _backwardLimitVector.Normalize();
            _backwardLimitVector *= swingRadius;

            // The forward limit is just the backward limit with the x and z multiplied by -1
            _forwardLimitVector = _backwardLimitVector;
            _forwardLimitVector.x *= -1;
            _forwardLimitVector.z *= -1;

            _initialPosition = currentHeadAnchor.transform.position;
            _initialRotation = currentHeadAnchor.transform.rotation;

            _reelInDestination = currentBaseAnchor.transform.position + _backwardLimitVector;
            _interpolant = 0.0f;
            _isRopeTied = true;
        }
    }

    private void LaunchRock()
    {
        // Calculate the forward of the swing
        _swingForward = Vector3.Cross(-transform.right, Vector3.up).normalized;

        _isRopeTied = false;

        splineRoute.controlPoints[0].position = transform.position;
        splineRoute.controlPoints[1].position = transform.position + (transform.up * rockLaunchMagnitude);

        RaycastHit hit;
        if (Physics.Raycast(transform.position + (_swingForward * rockLaunchDistance), Vector3.down, out hit))
        {
            splineRoute.controlPoints[3].position = hit.point + new Vector3(0, transform.localScale.y / 2, 0);
        }

        splineRoute.controlPoints[2].position = splineRoute.controlPoints[3].position + new Vector3(0, 2, 0);
        _t = 0.0f;
        _isLaunched = true;

    }

    private void FollowLaunchSpline()
    {
        _p0 = splineRoute.controlPoints[0].position;
        _p1 = splineRoute.controlPoints[1].position;
        _p2 = splineRoute.controlPoints[2].position;
        _p3 = splineRoute.controlPoints[3].position;

        launchSpeed += launchAcceleration;
        _t += launchSpeed * Time.deltaTime;

        if (_t >= 1)
        {
            _isReeledIn = false;
            _isLaunched = false;
            return;
        }

        Vector3 targetPosition = Mathf.Pow(1 - _t, 3) * _p0 +
             3 * Mathf.Pow(1 - _t, 2) * _t * _p1 +
             3 * (1 - _t) * Mathf.Pow(_t, 2) * _p2 +
             Mathf.Pow(_t, 3) * _p3;

        _rigidbody.MovePosition(targetPosition);
    }
    public override void ResetObstacle()
    {
        door.transform.position = _initialDoorPosition;
        transform.position = _initialBoulderPosition;
        door.transform.rotation = _initialDoorRotation;
        transform.rotation = _initialBoulderRotation;
    }

    public override void UnresetObstacle()
    {
    }

    private void OnDrawGizmos()
    {
        if (_isRopeTied)
        {
            Debug.DrawRay(currentBaseAnchor.transform.position, _backwardLimitVector, Color.red);
            Debug.DrawRay(currentBaseAnchor.transform.position, _forwardLimitVector, Color.green);
        }
    }
}
