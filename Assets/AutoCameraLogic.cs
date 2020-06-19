///-------------------------------------------------------------------------------------------------
// file: AutoCameraLogic.cs
//
// author: Jesse Berube
// date: 06/18/2020
//
// summary: Logic to set up new positions for the camera, as well as tell the camera to follow or look at the player
///-------------------------------------------------------------------------------------------------

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AutoCameraLogic : MonoBehaviour
{
    public Transform followTarget;

    private CinemachineVirtualCamera _autoCamera;
    private CinemachineTransposer _transposer;
    private Transform _targetTransform;

    private bool _followPosition;
    private bool _followRotation;

    private bool _centerX;
    private bool _centerY;
    private bool _centerZ;

    private Vector3 _targetPosition;

    private float _timeToReachPosition;
    private float _timeToReachRotation;
    private float _tPosition;
    private float _tRotation;

    private bool _targetPositionReached = true;
    private bool _targetRotationReached = true;

    private Vector3 _transitionStartPosition;
    private Quaternion _transitionStartRotation;

    private bool _setPositionTargetDone = false;
    private bool _setRotationTargetDone = false;


    private void Awake()
    {
        _autoCamera = GetComponent<CinemachineVirtualCamera>();
        _transposer = _autoCamera.GetCinemachineComponent<CinemachineTransposer>();
    }

    private void Update()
    {
        if (_targetPositionReached == false)
        {
            if (!_followPosition)
            {
                _targetPosition = _targetTransform.position;

                if (_centerX)
                {
                    _targetPosition.x = followTarget.position.x;
                }

                if (_centerY)
                {
                    _targetPosition.y = followTarget.position.y;
                }

                if (_centerZ)
                {
                    _targetPosition.z = followTarget.position.z;
                }
            }
            else
            {
                _targetPosition = _targetTransform.position - followTarget.position;

                if (_centerX)
                {
                    _targetPosition.x = 0;
                }

                if (_centerY)
                {
                    _targetPosition.y = 0;
                }

                if (_centerZ)
                {
                    _targetPosition.z = 0;
                }
            }
        }

        MoveToPosition();
        AdjustRotation();

        if(_targetPositionReached && _followPosition && _setPositionTargetDone == false)
        {
            _transposer.m_FollowOffset = _autoCamera.transform.position - followTarget.position;

            _autoCamera.m_Follow = followTarget;
            _setPositionTargetDone = true;
        }   

        if(_targetRotationReached && _followRotation && _setRotationTargetDone == false)
        {
            _autoCamera.m_LookAt = followTarget;
            _setRotationTargetDone = true;
        }

        if(_targetPositionReached && _targetRotationReached && _targetTransform != null) 
        {
            _targetTransform.transform.position = transform.position;
            _targetTransform.transform.rotation = transform.rotation;
        }
    }

    private void MoveToPosition()
    {
        if (_targetPositionReached == false)
        {
            _tPosition += Time.deltaTime / _timeToReachPosition;

            if (!_followPosition)
            {
                transform.position = Vector3.Lerp(_transitionStartPosition, _targetPosition, _tPosition);
            }
            else
            {
                _transposer.m_FollowOffset = Vector3.Lerp(_transitionStartPosition, _targetPosition, _tPosition);
            }

            if(_tPosition >= 1.0f)
            {
                _targetPositionReached = true;
            }
        }
    }

    private void AdjustRotation()
    {
        if (_targetRotationReached == false && _autoCamera.m_LookAt == null)
        {
            _tRotation += Time.deltaTime / _timeToReachRotation;

            transform.rotation = Quaternion.Lerp(_transitionStartRotation, _targetTransform.rotation, _tRotation);

            if (_tRotation >= 1.0f)
            {
                _targetRotationReached = true;
            }
        }
    }

    public void SetCameraTransition(Transform referenceTransform, bool followTargetPosition, bool followTargetRotation, float positionSpeed, float rotationSpeed, bool centerX, bool centerY, bool centerZ)
    {
        if(_followPosition && followTargetPosition)
        {
            _transitionStartPosition = _transposer.m_FollowOffset;
        }
        else if(!_followPosition && !followTargetPosition)
        {
            _transitionStartPosition = transform.position;
        }
        else if(_followPosition && !followTargetPosition)
        {
            _autoCamera.m_Follow = null;
            _transitionStartPosition = transform.position;
        }
        else if(!_followPosition && followTargetPosition)
        {
            _transposer.m_FollowOffset = transform.position - followTarget.position;

            _autoCamera.m_Follow = followTarget;
            _transitionStartPosition = _transposer.m_FollowOffset;
        }


        _transitionStartRotation = transform.rotation;

        _centerX = centerX;
        _centerY = centerY;
        _centerZ = centerZ;

        _targetTransform = referenceTransform;
        _timeToReachPosition = positionSpeed;
        _timeToReachRotation = rotationSpeed;
        _followPosition = followTargetPosition;
        _followRotation = followTargetRotation;

        _tPosition = 0;
        _tRotation = 0;

        _setPositionTargetDone = false;
        _setRotationTargetDone = false;
        _targetPositionReached = false;


        if (_followRotation)
        {
            _targetRotationReached = true;
            _autoCamera.m_LookAt = followTarget.transform;
        }
        else
        {
            _targetRotationReached = false;
            _autoCamera.m_LookAt = null;
        }
    }
}
