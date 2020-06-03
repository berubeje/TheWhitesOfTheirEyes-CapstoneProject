using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    public bool isColliding = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.tag != "Ground")
    //    {
    //        isColliding = true;
    //        Debug.Log("Colliding with " + collision.gameObject.name);
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.tag != "Ground")
    //    {
    //        isColliding = true;
    //        Debug.Log("No longer Colliding with " + collision.gameObject.name);
    //    }
    //}
}
