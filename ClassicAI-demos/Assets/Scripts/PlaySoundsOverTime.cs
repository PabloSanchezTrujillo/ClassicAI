using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlaySoundsOverTime : MonoBehaviour
{
    [SerializeField] private float firstDelay = 10.0f;
    
    [SerializeField] private float minTimeSpacing = 5.0f;
    
    [SerializeField] private float maxTimeSpacing = 12.0f;

    private RandomAudioPlay audioRandom;

    private bool canPlay = true;
    
    private void Awake()
    {
        audioRandom = GetComponent<RandomAudioPlay>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameEventSystem.instance.bossDeath += StopPlaying;
        StartCoroutine(PlayRandomSound());
    }

    IEnumerator PlayRandomSound()
    {
        yield return new WaitForSeconds(firstDelay);
        
        while (canPlay)
        {
            yield return new WaitForSeconds(Random.Range(minTimeSpacing, maxTimeSpacing));
            if(canPlay)
                audioRandom.PlayRandomSound();
        }

    }

    private void StopPlaying()
    {
        canPlay = false;
        StopCoroutine(PlayRandomSound());
    }
    
}
