using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevelOnCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
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
