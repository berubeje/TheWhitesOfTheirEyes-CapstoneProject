///-------------------------------------------------------------------------------------------------
// file: AutoCameraLogic.cs
//
// author: Jesse Berube
// date: 06/18/2020
//
// summary: Logic to set up new positions for the camera, as well as tell the camera to follow or look at the player. No longer in use due to the dolly camera being superior, so the code might be a bit messy.
///-------------------------------------------------------------------------------------------------

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AutoCameraLogic : MonoBehaviour
{
    public Transform followTarget;
    public Transform targetTransform;


    private CinemachineVirtualCamera _autoCamera;
    private CinemachineTransposer _transposer;

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
        // If the camera is going to stay in one positions, the position can be changed by the transform. If its going to follow the player however, the camera needs to be repositions using the follow offset instead, or there will be camera jitters once the follow target is set.
        if (_targetPositionReached == false)
        {
            if (!_followPosition)
            {
                _targetPosition = targetTransform.position;

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
                _targetPosition = targetTransform.position - followTarget.position;

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

        // Check if the camera has reached its position target and is set to follow the player.
        if (_targetPositionReached && _followPosition && _setPositionTargetDone == false)
        {
            _transposer.m_FollowOffset = _autoCamera.transform.position - followTarget.position;

            _autoCamera.m_Follow = followTarget;
            _setPositionTargetDone = true;
        }

        // Check if the camera has reached its rotation target and is set to look at the player.
        if (_targetRotationReached && _followRotation && _setRotationTargetDone == false)
        {
            _autoCamera.m_LookAt = followTarget;
            _setRotationTargetDone = true;
        }

        // Move the reference transform with the camera so if the player backtracks, the reference transform will be where it left off.
        if (_targetPositionReached && _targetRotationReached && targetTransform != null)
        {
            targetTransform.transform.position = transform.position;
            targetTransform.transform.rotation = transform.rotation;
        }
    }

    // Lerps the camera to the new rotation if it has a transition time, otherwise it is teleported to the new position.
    private void MoveToPosition()
    {
        if (_targetPositionReached == false)
        {
            if (_timeToReachPosition > 0)
            {
                _tPosition += Time.deltaTime / _timeToReachPosition;


                // Because you cannot change the camera transform.position in script when the camera is set to follow the target, the offset needs to be changed instead so the camera is able to transition smoothly.

                if (!_followPosition)
                {
                    transform.position = Vector3.Lerp(_transitionStartPosition, _targetPosition, _tPosition);
                }
                else
                {
                    _transposer.m_FollowOffset = Vector3.Lerp(_transitionStartPosition, _targetPosition, _tPosition);
                }

                if (_tPosition >= 1.0f)
                {
                    _targetPositionReached = true;
                }
            }
            else
            {
                if (!_followPosition)
                {
                    transform.position = _targetPosition;
                }
                else
                {
                    _transposer.m_FollowOffset = _targetPosition;
                }

                _targetPositionReached = true;

            }
        }
    }

    //Lerps the camera to the new rotation if it has a transition time, otherwise it is teleported to the new position

    private void AdjustRotation()
    {
        if (_targetRotationReached == false && _autoCamera.m_LookAt == null)
        {
            if (_timeToReachRotation > 0)
            {
                _tRotation += Time.deltaTime / _timeToReachRotation;

                transform.rotation = Quaternion.Lerp(_transitionStartRotation, targetTransform.rotation, _tRotation);

                if (_tRotation >= 1.0f)
                {
                    _targetRotationReached = true;
                }
            }
            else
            {

                transform.rotation = targetTransform.rotation;

                _targetRotationReached = true;

            }
        }
    }

    // This is called by transition triggers in the world to start the camera transition process.
    public void SetCameraTransition(Transform referenceTransform, bool followTargetPosition, bool followTargetRotation, float positionSpeed, float rotationSpeed, bool centerX, bool centerY, bool centerZ)
    {
        if (_followPosition && followTargetPosition)
        {
            _transitionStartPosition = _transposer.m_FollowOffset;
        }
        else if (!_followPosition && !followTargetPosition)
        {
            _transitionStartPosition = transform.position;
        }
        else if (_followPosition && !followTargetPosition)
        {
            _autoCamera.m_Follow = null;
            _transitionStartPosition = transform.position;
        }
        else if (!_followPosition && followTargetPosition)
        {
            _transposer.m_FollowOffset = transform.position - followTarget.position;

            _autoCamera.m_Follow = followTarget;
            _transitionStartPosition = _transposer.m_FollowOffset;
        }


        _transitionStartRotation = transform.rotation;

        _centerX = centerX;
        _centerY = centerY;
        _centerZ = centerZ;

        targetTransform = referenceTransform;
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
