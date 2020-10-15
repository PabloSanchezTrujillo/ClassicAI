using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LevelCompleteTrigger : MonoBehaviour
{
    [SerializeField] private LevelLoader levelLoader;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!levelLoader)
            {
                Debug.LogError("LevelLoader not found");
                return;
            }

            levelLoader.LoadNextLevel();
        }
    }
}
