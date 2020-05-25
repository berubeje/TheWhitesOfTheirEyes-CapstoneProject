using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    public float acceleration = 10;
    public float maxSpeed = 8;
    public float jumpPower = 2;

    private Rigidbody unityRigidbody;
    private PlayerGrapplingHook grapplingHook;
    public bool onGround = false;
    public bool hookOut = false;
    public bool hookLaunched = false;

    public void Awake()
    {
        grapplingHook = GetComponentInChildren<PlayerGrapplingHook>();

        unityRigidbody = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
   
        //if(grapplingHook.launched == true)
        //{
        //    unityRigidbody.isKinematic = true;
        //}
        //else
        //{
        //    unityRigidbody.isKinematic = false;
        //}

        //if (!onGround && !grapplingHook.hookOut)
        //{
        //    return;
        //}
        //else if(hookOut)
        //{
        //    //unityRigidbody.AddForce(new Vector3(0, 0, Input.GetAxis("Vertical") * acceleration), ForceMode.Acceleration);

        //    //if (Input.GetButtonDown("Jump"))
        //    //{
        //    //    unityRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
        //    //}
        //}
        //else
        //{
        //    //unityRigidbody.AddForce(new Vector3(Input.GetAxis("Horizontal") * acceleration, 0, 0), ForceMode.Acceleration);
        //    //unityRigidbody.AddForce(new Vector3(0, 0, Input.GetAxis("Vertical") * acceleration), ForceMode.Acceleration);

        //    //if (Input.GetButtonDown("Jump"))
        //    //{
        //    //    unityRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
        //    //}
        //}

        //unityRigidbody.velocity = Vector3.ClampMagnitude(unityRigidbody.velocity, maxSpeed);
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