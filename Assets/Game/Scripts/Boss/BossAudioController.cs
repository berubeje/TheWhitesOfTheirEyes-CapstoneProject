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
    private void WindUpEvent()
    {
        AudioManager.Instance.PlaySound("BossWindUp");
    }

    private void RockTumbleEvent()
    {
        AudioManager.Instance.PlaySound("BossRockTumble");
    }
    private void FallEvent()
    {
        AudioManager.Instance.PlaySound("BossFall");
    }
    private void DieEvent()
    {
        AudioManager.Instance.PlaySound("BossDie");
    }

    private void FacePlantEvent()
    {
        AudioManager.Instance.PlaySound("BossFacePlant");
    }
}
