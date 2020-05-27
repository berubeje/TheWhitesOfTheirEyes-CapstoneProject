using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class RopeStateController : MonoBehaviour
{
    public PlayerGrapplingHook ropeLogic;

    private Rigidbody _rigidBody;
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckRopeState();
    }

    private void CheckRopeState()
    {
        if (ropeLogic != null)
        {
            switch (ropeLogic.ropeState)
            {
                case PlayerGrapplingHook.RopeState.Launched:
                    {
                        _rigidBody.isKinematic = true;
                        break;
                    }

                case PlayerGrapplingHook.RopeState.Landed:
                    {
                        _rigidBody.isKinematic = false;
                        break;
                    }

                case PlayerGrapplingHook.RopeState.Swing:
                    {
                        _rigidBody.isKinematic = false;
                        _animator.applyRootMotion = false;
                        break;
                    }

                case PlayerGrapplingHook.RopeState.Pull:
                    {
                        _rigidBody.isKinematic = false;
                        _animator.applyRootMotion = true;
                        break;
                    }


            }

        }
    }
}
