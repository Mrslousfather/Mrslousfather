using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicbgm : MonoBehaviour
{
    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
       
    }

    public void PlayMusic()
    {
        if (audioSource && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
