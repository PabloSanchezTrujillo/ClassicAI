using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAudio : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private AudioSource source;

    private bool isFading;

    private float startVolume = 0f;
    private float targetVolume = 1f;

    private float step = 0f;

    [SerializeField]
    private float volumeValueWhenPlayerDies = 0.2f;
    
    // Start is called before the first frame update
    void Start()
    {
        GameEventSystem.instance.bossDeath += FadeOut;
        GameEventSystem.instance.loadNextLevel += TurnOff;
        GameEventSystem.instance.restartScreen += FadeOutWhenPlayerDies;
        FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        // Fade in
        if (isFading)
        {
            source.volume = Mathf.Lerp(startVolume, targetVolume, step);

            if (Mathf.Abs(source.volume - targetVolume) > Mathf.Epsilon)
            {
                step += Time.deltaTime / fadeDuration;
            }
            else
            {
                source.volume = targetVolume;
                isFading = false;
            }
        }
    }

    // Fade in
    void FadeIn()
    {
        isFading = true;
        startVolume = 0f;
        targetVolume = source.volume;
        step = 0f;
    }

    // Fade out
    void FadeOut()
    {
        isFading = true;
        startVolume = source.volume;
        targetVolume = 0f;
        step = 0f;
    }

    void FadeOutWhenPlayerDies()
    {
        isFading = true;
        startVolume = source.volume;
        targetVolume = volumeValueWhenPlayerDies;
        step = 0f;
    }
    
    private void TurnOff()
    {
        source.Stop();
    }
}
