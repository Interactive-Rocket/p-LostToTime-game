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
    private bool abilityActive;
    private float loopSustain = 0.1f;
    private float lastInput;
    // Start is called before the first frame update
    void Start()
    {
        audioSourceAbility = GetComponents<AudioSource>()[0];
        audioSourceFootsteps = GetComponents<AudioSource>()[1];
        audioSourceFootsteps.clip = footstep;
        abilityActive = false;
        lastInput = 0;
        audioSourceFootsteps.volume = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        lastInput +=Time.deltaTime;
        if (!audioSourceAbility.isPlaying && lastInput > loopSustain)
            abilityActive = false;
    }

    public void PlayAbilitySound()
    {
        if (!abilityActive && !audioSourceAbility.isPlaying){
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
        if (!audioSourceFootsteps.isPlaying){
            audioSourceFootsteps.Play();
        }
        audioSourceFootsteps.pitch = speed;
    }
    public void StopFootsteps()
    {
        audioSourceFootsteps.Stop();
    }

}
