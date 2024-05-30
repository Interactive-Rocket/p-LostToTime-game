using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundController : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private TimeEntity timeEntity;
    [SerializeField]
    public AudioClip audioClip;
    [SerializeField]
    public float playbackTime;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null){
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        timeEntity = GetComponent<TimeEntity>();
        
    }

    // Update is called once per frame
    void Update()
    {
        // Reverse the playback if time is reversed
        audioSource.pitch = timeEntity._timeScale;

        // Handle backwards audio
        if (timeEntity.IsRewinding)
        {
            if (audioClip != null){
                audioSource.clip = audioClip;
                
                // Assumes that the rewind speed is constant
                if (!audioSource.isPlaying && playbackTime != 0){
                    audioSource.Play();
                    
                    // Seeking to the end throws an error
                    float maxSeekTime = audioSource.clip.length * 0.95f;
                    audioSource.time = math.min(playbackTime,maxSeekTime);
                }
            }

        }
        else{
            audioClip = audioSource.clip;
            playbackTime = audioSource.time;
        }
    }

    public void Play(AudioClip clip){
        if (timeEntity == null){
            print("time entity missing huh");
        }
        // Start reproduction of new sounds only if not rewinding
        if (!timeEntity.IsRewinding){
            audioSource.clip = audioClip = clip;
            playbackTime = 0;
            audioSource.Play();
        }
    }

    public void SeekTo(float partition){
        audioSource.time = partition * audioClip.length;
    }
}
