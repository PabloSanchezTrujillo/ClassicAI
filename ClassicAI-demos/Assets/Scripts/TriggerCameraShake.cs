using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCameraShake : MonoBehaviour
{


    private RandomAudioPlay randomClip;
    
    private void Awake()
    {
        randomClip = GetComponent<RandomAudioPlay>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        Debug.Log(other.gameObject.name);
        
        randomClip.PlayRandomSound();
        
        /*if(other.gameObject.name.Contains(("Ojo")) || other.gameObject.name.Contains(("Boca")))
        {
            GameEventSystem.instance.ShakeCamera();
        }*/
        
        
        GameEventSystem.instance.ShakeCamera();
        
    }
}
