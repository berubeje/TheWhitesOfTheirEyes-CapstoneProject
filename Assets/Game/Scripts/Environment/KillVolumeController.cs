using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillVolumeController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Animator>().SetBool("dead", true);
            InputManager.Instance.currentGameState = InputManager.GameStates.GameOver;
        }
    }
}
