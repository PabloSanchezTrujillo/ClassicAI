using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DemonSoundManager : MonoBehaviour
{
    #region variables

    [SerializeField] private AudioClip introClip;
    [SerializeField] private AudioClip idleClip;
    [SerializeField] private AudioSource starAudioSource;
    [SerializeField] private AudioClip starAppearing;
    [SerializeField] private AudioClip attack1Clip;
    [SerializeField] private AudioClip attack2Clip;
    [SerializeField] private AudioClip attack3Clip;
    [SerializeField] private AudioClip demonDeath;

    private AudioSource audioSource;

    #endregion variables

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        audioSource.clip = introClip;
        audioSource.Play();
    }

    public void IdleShout()
    {
        float dice = Random.value;
        if(dice > 0.5) {
            audioSource.clip = idleClip;
            audioSource.Play();
        }
    }

    public void StarAppears()
    {
        starAudioSource.clip = starAppearing;
        starAudioSource.Play();
    }

    public void Attack1Sound()
    {
        audioSource.clip = attack1Clip;
        audioSource.Play();
    }

    public void Attack2Sound()
    {
        audioSource.clip = attack2Clip;
        audioSource.Play();
    }

    public void Attack3Sound()
    {
        audioSource.clip = attack3Clip;
        audioSource.Play();
    }

    public void DemonDeath()
    {
        audioSource.clip = demonDeath;
        audioSource.Play();
    }
}