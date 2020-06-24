///-------------------------------------------------------------------------------------------------
// file: VerySmartBossAIScript.cs
//
// author: Jesse Berube
// date: 2020-06-23
//
// summary: A very intelligent boss AI that will definantly be sticking around for the final version an will not at all be scrapped once the AI Tree is done
///-------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

public class VerySmartBossAIScript : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    public float distanceOffset;

    private Animator _animator;
    private int _currentIndex = 0;

    private Transform _currentTarget;
    private bool _turning = false;
    private bool _turningSet = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _currentTarget = waypoints[_currentIndex];
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, _currentTarget.position) <= distanceOffset)
        {
            if (_turningSet == false)
            {
                _animator.SetTrigger("Turn");
                _turningSet = true;
            }
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
        {
            if (_turning == true)
            {
                _turning = false;
                _turningSet = false;
                NextWaypoint();
            }

            Vector3 lookVector = new Vector3(_currentTarget.position.x, transform.position.y, _currentTarget.position.z);
            transform.LookAt(lookVector);
        }
        else if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Turn"))
        {
            _turning = true;
        }

    }

    private void NextWaypoint()
    {
        if(_currentIndex + 1 == waypoints.Count)
        {
            _currentIndex = 0;
        }
        else
        {
            _currentIndex++;
        }

        _currentTarget = waypoints[_currentIndex];
    }
}
