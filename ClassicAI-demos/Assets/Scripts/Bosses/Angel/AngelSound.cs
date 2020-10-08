using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelSound : MonoBehaviour
{
    [SerializeField] AudioSource audioHandler;
    [SerializeField] AudioClip[] clips;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void PlayIntroSound()
    {
        audioHandler.clip = clips[0];
        audioHandler.Play();
    }

    private void PlayWingSound()
    {
        audioHandler.clip = clips[1];
        audioHandler.Play();
    }

    private void PlayDiscSound()
    {
        audioHandler.clip = clips[2];
        audioHandler.Play();
    }

    private void PlayDeahtSound()
    {
        audioHandler.clip = clips[3];
        audioHandler.Play();
    }
}
