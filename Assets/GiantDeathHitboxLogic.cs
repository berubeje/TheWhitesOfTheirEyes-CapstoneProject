///-------------------------------------------------------------------------------------------------
// file: GiantDeathHitboxLogic.cs
//
// author: Jesse Berube
// date: 2020-06-23
//
// summary: A script to kill the boss in aplha build when a certain thing drops on the giants head.
///-------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

public class GiantDeathHitboxLogic : MonoBehaviour
{

    public List<GameObject> killObjects = new List<GameObject>();

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (GameObject killObject in killObjects)
        {
            if (other.gameObject == killObject)
            {
                _animator.SetTrigger("Die");
                return;
            }
        }
    }
}
