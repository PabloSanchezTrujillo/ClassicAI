using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TriggerText : MonoBehaviour
{

    private TMP_Text text;
    [SerializeField] private float fadeTime;

    private float currentAlpha = 0.0f;
    
    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        text.alpha = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            StartCoroutine(FadeImage(false));
        }
    }
    
    
    IEnumerator FadeImage(bool fadeAway)
    {
        
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            while (currentAlpha >= 0.0f)
            {
                // set color with i as alpha
                currentAlpha -= Time.deltaTime / fadeTime;
                text.alpha = currentAlpha;
                yield return null;
            }

            currentAlpha = 0.0f;
            text.alpha = 0.0f;

        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second backwards
            while (currentAlpha <= 1.0f)
            {
                // set color with i as alpha
                currentAlpha += Time.deltaTime / fadeTime;
                text.alpha = currentAlpha;
                yield return null;
            }
            
            currentAlpha = 1.0f;
            text.alpha = 1.0f;

        }
    }
}
