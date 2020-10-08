using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartScreenListener : MonoBehaviour
{
    [SerializeField] private GameObject restartScreen;

    private bool isHidden = false;
    
    // Start is called before the first frame update
    void Start()
    {
        GameEventSystem.instance.restartScreen += ShowRestartScreen;
        GameEventSystem.instance.loadNextLevel += HideRestartScreen;
    }


    public void HideRestartScreen()
    {
        restartScreen.SetActive(false);
        isHidden = true;
    }
    
    public void ShowRestartScreen()
    {
        if(!isHidden)
            StartCoroutine(ShowDelayed());
    }

    IEnumerator ShowDelayed()
    {
        yield return new WaitForSeconds(1.0f);
        restartScreen.SetActive(true);
    }
    
    
    
}
