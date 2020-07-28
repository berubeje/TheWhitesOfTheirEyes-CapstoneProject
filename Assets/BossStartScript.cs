using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStartScript : MonoBehaviour
{
    public BossController boss;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<JimController>() != null)
        {
            boss.bossStart = true;
            Destroy(this.gameObject);
        }
    }
}
