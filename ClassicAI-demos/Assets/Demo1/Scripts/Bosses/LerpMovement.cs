using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpMovement : MonoBehaviour
{
    
    private bool shouldLerp = false;

    private float timeStartedLerping;
    
    private float lerpTime;

    private Vector2 endPosition;
    private Vector2 startPosition;

    public bool hasFinishedLerping = false;
    
    public void StartLerping(Vector3 startPosition, Vector3 endPosition, float lerpTime)
    {

        this.startPosition = startPosition;
        this.endPosition = endPosition;
        this.lerpTime = lerpTime;
        
        timeStartedLerping = Time.time;
        shouldLerp = true;
        hasFinishedLerping = false;
    }
    
    
    // Update is called once per frame
    void Update()
    {
        if (shouldLerp)
        {
            transform.position = Lerp(startPosition, endPosition, timeStartedLerping, lerpTime);
            
        }

        if (transform.position.Equals(endPosition))
        {
            shouldLerp = false;
            hasFinishedLerping = true;
        }
    }

    public Vector3 Lerp(Vector3 start,Vector3 end,float timeStartedLerping,float lerpTime = 1)
    {
        float timeSinceStarted = Time.time - timeStartedLerping;

        float percentageComplete = timeSinceStarted / lerpTime;

        percentageComplete = Mathf.Clamp(percentageComplete, 0.0f, 1.0f);
        
        var result = Vector3.Lerp(start, end, percentageComplete);

        return result;
    }
    
}
