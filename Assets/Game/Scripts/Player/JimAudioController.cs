///-------------------------------------------------------------------------------------------------
// file: JimAudioController.cs
//
// author: Rishi Barnwal
// date: 24/06/2020
//
// summary: Implements the animation events for the player character
///-------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JimAudioController : MonoBehaviour
{
    public AudioClip[] dirtStepSounds;
    public AudioClip[] rollSounds;
    private AudioSource _audioSource;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void StepEvent()
    {
        // Choose a random step sound and play it
        int index = UnityEngine.Random.Range(0, dirtStepSounds.Length);
        _audioSource.clip = dirtStepSounds[index];

        _audioSource.Play();
    }

    private void RollEvent()
    {
        // Choose a random step sound and play it
        int index = UnityEngine.Random.Range(0, rollSounds.Length);
        _audioSource.clip = rollSounds[index];

        _audioSource.Play();
    }
}
