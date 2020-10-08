using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventSystem : MonoBehaviour
{
    
    public static GameEventSystem instance;

    private void Awake()
    {
        instance = this;
    }

    public event Action startDissolve;
    public void StartDissolve()
    {
        if(startDissolve != null)
        {
            startDissolve();
        }
    }

    public event Action shakeCamera;
    public void ShakeCamera()
    {
        if(shakeCamera != null)
        {
            shakeCamera();
        }
    }

    public event Action loadNextLevel;
    public void LoadNextLevel()
    {
        if(loadNextLevel != null)
        {
            loadNextLevel();
        }
    }

    public event Action restartScreen;
    public void RestartScreen()
    {
        if(restartScreen != null)
        {
            restartScreen();
        }
    }
    
    public event Action bossDeath;
    public void BossDeath()
    {
        if(bossDeath != null)
        {
            bossDeath();
        }
    }
}
