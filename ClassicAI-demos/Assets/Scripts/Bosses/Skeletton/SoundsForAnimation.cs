using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsForAnimation : MonoBehaviour
{
    private AudioSource audio;

    
    
    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    public void PlaySoundForAnimation(AudioClip sound)
    {
        audio.clip = sound;
        audio.loop = sound.name.Contains("Charging");   
        audio.Play();
    }
}
