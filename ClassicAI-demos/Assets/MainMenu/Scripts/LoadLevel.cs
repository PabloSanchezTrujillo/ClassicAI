using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene((int)Scenes.Scene.MainMenu, LoadSceneMode.Single);
    }

    public void LoadDemo1()
    {
        SceneManager.LoadScene((int)Scenes.Scene.Demo1_Sekele, LoadSceneMode.Single);
    }

    public void LoadDemo2()
    {
        SceneManager.LoadScene((int)Scenes.Scene.Demo2_Combat, LoadSceneMode.Single);
    }

    public void LoadDemo3()
    {
        SceneManager.LoadScene((int)Scenes.Scene.Demo3_LevelGenerator, LoadSceneMode.Single);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}