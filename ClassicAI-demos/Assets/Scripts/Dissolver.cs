using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolver : MonoBehaviour
{
    private Material material;

    private bool isDying;

    private bool isSpawning;

    private float t = 1f;

    public float dissolveTime = 1f;

    private void Start()
    {
        material = GetComponent<Renderer>().material;
        GameEventSystem.instance.startDissolve += Dissolve;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Dissolve();
        //}

        if(isDying)
            FadeOut(Time.deltaTime / dissolveTime);

        if(isSpawning)
            FadeIn(Time.deltaTime / dissolveTime);
    }

    /// <summary>
    /// Call this to start the dissolve animation
    /// </summary>
    public void Dissolve()
    {
        t = 1f;
        isDying = true;
    }

    /// <summary>
    /// Call this to start the dissolve animation backwards. For like spawning a new boss or something
    /// </summary>
    public void DissolveInverse()
    {
        t = 0f;
        isSpawning = true;
    }

    private void FadeIn(float step)
    {
        if(t <= 1.1f) {
            material.SetFloat("_Step", Mathf.Sin(t));

            t += step;
        }
        else {
            isSpawning = false;
        }
    }

    private void FadeOut(float step)
    {
        if(t >= -0.1f) {
            material.SetFloat("_Step", Mathf.Sin(t));

            t -= step;
        }
        else {
            isDying = false;
        }
    }
}