using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private UnityEngine.UI.Text textArea;

    /// <summary>
    /// Index of the scene that will get loaded after the animation from <see cref="anim"/> ends
    /// </summary>
    [SerializeField] private int nextScene = 0;

    /// <summary>
    /// Time before the transition begins. Useful for waiting for an animation to end or something
    /// </summary>
    [SerializeField] private float preTransitionWaitTime = 0f;

    [Header("Screen Fade")]

    /// <summary>
    /// Time it takes for the black screen to appear
    /// </summary>
    [SerializeField] private float fadeinDuration = 0.3f;

    /// <summary>
    /// Time it takes for the black screen to disappear
    /// </summary>
    [SerializeField] private float fadeoutDuration = 0.3f;

    [Header("Mid-transition")]

    /// <summary>
    /// Time between the end of the fade out and the start of the fadeIn
    /// </summary>
    [SerializeField] private float dialogueDuration = 0.7f;

    [Header("Text Fade")]

    /// <summary>
    /// Time it takes for the text to appear
    /// </summary>
    [SerializeField] private float textFadeinDuration = 0.3f;

    /// <summary>
    /// Time it takes for the text to disappear
    /// </summary>
    [SerializeField] private float textFadeoutDuration = 0.3f;

    [Space]

    /// <summary>
    /// Time before the text fade in begins.
    /// </summary>
    [SerializeField] private float beforeTextFadeinWaitTime = 0f;

    /// <summary>
    /// Time before the text fade out begins.
    /// </summary>
    [SerializeField] private float afterTextFadeoutWaitTime = 0f;

    private bool fadingOut;

    private bool fadingIn;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        textArea = GetComponentInChildren<UnityEngine.UI.Text>();

        GameEventSystem.instance.loadNextLevel += LoadNextLevel;
        
        //Fade out
        textArea.canvasRenderer.SetAlpha(0f);
        canvasGroup.alpha = 1;
        StartCoroutine(FadeOut());
    }

    private void Update()
    {
        if (fadingIn)
        {
            if (canvasGroup.alpha < 1.0f)
            {
                canvasGroup.alpha += Time.deltaTime / fadeinDuration;
            }
            else
            {
                fadingIn = false;
                canvasGroup.alpha = 1f;
            }
        }

        if (fadingOut)
        {
            if (canvasGroup.alpha > 0.0f)
            {
                canvasGroup.alpha -= Time.deltaTime / fadeoutDuration;
            }
            else
            {
                fadingOut = false;
                canvasGroup.alpha = 0f;
            }
        }
    }

    public void LoadNextLevel()
    {
        StartCoroutine(Transition(nextScene));
    }

    private IEnumerator Transition(int levelIndex)
    {
        // Wait pre-transition
        yield return new WaitForSeconds(preTransitionWaitTime);

        // Fade in
        yield return FadeIn();

        // Fade text in
        yield return FadeTextIn();

        // Wait dialogue time
        yield return new WaitForSeconds(dialogueDuration);

        // Fade text out
        yield return FadeTextOut();

        // Load the scene
        SceneManager.LoadScene(levelIndex);
    }

    private IEnumerator FadeOut()
    {
        fadingOut = true;
        yield return new WaitForSeconds(fadeoutDuration);
    }

    private IEnumerator FadeIn()
    {
        fadingIn = true;
        yield return new WaitForSeconds(fadeinDuration);
    }

    private IEnumerator FadeTextIn()
    {
        // Wait before fade
        yield return new WaitForSeconds(beforeTextFadeinWaitTime);

        // Fade
        textArea.CrossFadeAlpha(1.0f, textFadeinDuration, true);
        yield return new WaitForSeconds(textFadeinDuration);
    }

    private IEnumerator FadeTextOut()
    {
        // Fade
        textArea.CrossFadeAlpha(0.0f, textFadeoutDuration, true);
        yield return new WaitForSeconds(textFadeoutDuration);

        // Wait after fade
        yield return new WaitForSeconds(afterTextFadeoutWaitTime);
    }
}
