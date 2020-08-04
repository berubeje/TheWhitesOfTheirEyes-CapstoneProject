using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAudioController : MonoBehaviour
{
    public AudioClip[] stepSounds;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void StepEvent()
    {
        // Choose a random step sound and play it
        int index = Random.Range(0, stepSounds.Length);
        _audioSource.clip = stepSounds[index];

        _audioSource.Play();
    }

    private void AttackEvent()
    {
        AudioManager.Instance.PlaySound("BossAttack");
    }
}
