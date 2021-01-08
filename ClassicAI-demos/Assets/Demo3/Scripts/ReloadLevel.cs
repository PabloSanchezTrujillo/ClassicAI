using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadLevel : MonoBehaviour
{
    /// <summary>
    /// Reloads the level from the start
    /// </summary>
    public void Reload()
    {
        SceneManager.LoadScene((int)Scenes.Scene.Demo3_LevelGenerator, LoadSceneMode.Single);
    }
}