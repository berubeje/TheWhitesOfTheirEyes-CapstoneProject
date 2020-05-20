using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    public float acceleration = 10;
    public float maxSpeed = 8;
    public float jumpPower = 2;

    private Rigidbody unityRigidbody;
    private GrapplingHookType1 grapplingHook1;
    private GrapplingHookType2 grapplingHook2;
    public bool onGround = false;
    public bool hookOut = false;
    public bool hookLaunched = false;

    public void Awake()
    {
        grapplingHook1 = GetComponentInChildren<GrapplingHookType1>();
        grapplingHook2 = GetComponentInChildren<GrapplingHookType2>();

        unityRigidbody = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
   
        if(grapplingHook1 != null)
        {
            GrapplingHookType1 grapplingHook = grapplingHook1;
            hookOut = grapplingHook.hookOut;
    
        }
        else if(grapplingHook2 != null)
        {
            GrapplingHookType2 grapplingHook = grapplingHook2;
            hookOut = grapplingHook.hookOut;
            hookLaunched = grapplingHook.launched;
        }

        if(hookLaunched == true)
        {
            unityRigidbody.isKinematic = true;
        }
        else
        {
            unityRigidbody.isKinematic = false;
        }

        if (!onGround && !hookOut)
        {
            return;
        }
        else if(hookOut)
        {
            unityRigidbody.AddForce(new Vector3(0, 0, Input.GetAxis("Vertical") * acceleration), ForceMode.Acceleration);

            if (Input.GetButtonDown("Jump"))
            {
                unityRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            }
        }
        else
        {
            unityRigidbody.AddForce(new Vector3(Input.GetAxis("Horizontal") * acceleration, 0, 0), ForceMode.Acceleration);
            unityRigidbody.AddForce(new Vector3(0, 0, Input.GetAxis("Vertical") * acceleration), ForceMode.Acceleration);

            if (Input.GetButtonDown("Jump"))
            {
                unityRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            }
        }

        unityRigidbody.velocity = Vector3.ClampMagnitude(unityRigidbody.velocity, maxSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }
    }
}