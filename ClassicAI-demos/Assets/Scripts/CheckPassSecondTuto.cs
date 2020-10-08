using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPassSecondTuto : MonoBehaviour
{

    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.X))
        {
            StartCoroutine(DelayedNextLevel());
        }
        
    }

    IEnumerator DelayedNextLevel()
    {
        yield return new WaitForSeconds(4.0f);
        GameEventSystem.instance.LoadNextLevel();
    }
}
