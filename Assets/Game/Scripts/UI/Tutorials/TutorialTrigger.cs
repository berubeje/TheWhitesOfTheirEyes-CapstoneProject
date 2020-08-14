using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public Flowchart flowchart;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("SFLHDF");
            flowchart.gameObject.SetActive(true);
            Destroy(this);
        }
    }
}
