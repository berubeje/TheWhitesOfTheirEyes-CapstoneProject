using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineWallDestroy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Boulder"))
        {
            gameObject.SetActive(false);
        }
    }
}
