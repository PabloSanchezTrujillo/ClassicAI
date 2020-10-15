using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGameOnEscape : MonoBehaviour
{

    [SerializeField] private bool canPassLevel = true; 

    // Update is called once per frame
    void Update()
    {
         if (Input.GetKeyDown(KeyCode.Escape)) 
         {
            Application.Quit();
         }

         if (Input.GetKeyDown((KeyCode.N)) && canPassLevel)
         {
             GameEventSystem.instance.LoadNextLevel();
         }
    }
}
