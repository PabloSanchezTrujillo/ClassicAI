using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelLoaderWithMultipleTexts : MonoBehaviour
{

    [SerializeField] private float fadeTime = 0.2f;

    [SerializeField] private float showTime = 1.5f;
    
    [TextArea(5,30)]
    [SerializeField] private string[] texts;

    [SerializeField] private Text text;

    public int counter = 0;

    private bool isShowing = false;
    
    // Start is called before the first frame update
    void Start()
    {
        GameEventSystem.instance.loadNextLevel += ShowText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowText()
    {
        if (!isShowing)
        {
            isShowing = true;
            StartCoroutine(FadeInText());
        }
    }
    
    public IEnumerator FadeInText()
    {
        
        
        text.text = texts[counter];
        text.CrossFadeAlpha(1.0f, fadeTime, true);
        yield return new WaitForSeconds(showTime);

        StartCoroutine(FadeOutText());
    }
    
    public IEnumerator FadeOutText()
    {
        text.CrossFadeAlpha(0.0f, fadeTime, true);
        yield return new WaitForSeconds(fadeTime);
        
        counter++;
        if (counter < texts.Length)
        {
            StartCoroutine(FadeInText());
        }
    }
}
