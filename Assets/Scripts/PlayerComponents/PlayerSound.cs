using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private AudioSource audioSourceAbility;
    private AudioSource audioSourceFootsteps;
    public AudioClip abilityStartupSound;
    public AudioClip abilityLoopSound;
    public AudioClip footstep;
    public AudioClip jump;
    public AudioClip land;
    private bool abilityActive;
    private float loopSustain = 0.1f;
    private float lastInput;

    void Start()
    {
        audioSourceAbility = GetComponents<AudioSource>()[0];
        audioSourceFootsteps = GetComponents<AudioSource>()[1];
        audioSourceFootsteps.clip = footstep;
        abilityActive = false;
        lastInput = 0;
        audioSourceFootsteps.volume = 1f;
    }

    void Update()
    {
        if (PlayerManager.Instance != null) AudioListener.volume = PlayerManager.Instance.volume;
        lastInput += Time.deltaTime;
        if (!audioSourceAbility.isPlaying && lastInput > loopSustain)
            abilityActive = false;
    }

    public void PlayAbilitySound()
    {
        if (!abilityActive && !audioSourceAbility.isPlaying)
        {
            audioSourceAbility.clip = abilityStartupSound;
            audioSourceAbility.Play();
            abilityActive = true;
        }
        else if (abilityActive && !audioSourceAbility.isPlaying)
        {
            audioSourceAbility.clip = abilityLoopSound;
            audioSourceAbility.Play();
        }

        lastInput = 0;
    }

    public void PlayFootstep(float speed)
    {
        if (!audioSourceFootsteps.isPlaying)
        {
            audioSourceFootsteps.Play();
        }
        audioSourceFootsteps.pitch = speed;
    }

    public void StopFootsteps()
    {
        audioSourceFootsteps.Stop();
    }

    public void PlayJumpSound()
    {
        Debug.Log("Jumping in sound");
        AudioManager.Instance.PlayOneShot(jump, 0.7f);
    }

    public void PlayLandSound()
    {
        Debug.Log("Landing in sound");
        AudioManager.Instance.PlayOneShot(land);
    }

}
