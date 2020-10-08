using System;
using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
using UnityEngine;

public class CameraShakerListener : MonoBehaviour
{
    [Header("Camera shaking parameters")]
    [SerializeField] private float magnitude;

    [SerializeField] private float roughness;
    [SerializeField] private float fadeInTime;
    [SerializeField] private float fadeOutTime;

    private CameraShaker shaker;
    
    private void Awake()
    {
        shaker = GetComponent<CameraShaker>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameEventSystem.instance.shakeCamera += ShakeCamera;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShakeCamera()
    {
        if(shaker)
            shaker.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
    }
}
