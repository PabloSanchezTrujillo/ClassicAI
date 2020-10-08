using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressAnyKey : MonoBehaviour
{
    private AudioSource audio;
    private bool justPerformed = false;
    
    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey && !justPerformed)
        {
            GameEventSystem.instance.LoadNextLevel();
            audio.Play();
            justPerformed = true;
        }   
    }
}
