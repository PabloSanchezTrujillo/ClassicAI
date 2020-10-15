using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class RandomAudioPlay : MonoBehaviour
{

    [SerializeField] private AudioClip[] clips;

    [SerializeField] private AudioSource audio;

    private void Awake()
    {
        if (!audio)
            audio = GetComponent<AudioSource>();
    }

    public void PlayRandomSound()
    {
        if (!audio.isPlaying)
        {
            audio.clip = clips[Random.Range(0, clips.Length)];
            audio.Play();
        }
    }
}
