using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPassFirstTuto : MonoBehaviour
{

    private bool hasPressedZ = false;
    private bool hasPressedSpace = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            hasPressedZ = true;
        }
        
        if (Input.GetKey(KeyCode.Space))
        {
            hasPressedSpace = true;
        }

        if (hasPressedZ && hasPressedSpace)
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
