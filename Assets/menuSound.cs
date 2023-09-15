using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class menuSound : MonoBehaviour
{
    AudioSource audioSource;
    float playTime;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    
    public void StopSound()
    {
        playTime = audioSource.time;
        audioSource.Stop();
    }

    public void StartSound()
    {
        audioSource.time = playTime;
        audioSource.Play();
    }
}
