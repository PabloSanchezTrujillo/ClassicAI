using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletColor
{
    YELLOW, RED, PURPLE, BLUE
}

public class GameManager : MonoBehaviour
{
    public Color[] colors;

    public static GameManager instance;

    public GameObject player;
    
    private void Awake()
    {
        instance = this;
    }
}