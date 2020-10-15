using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableBullet : MonoBehaviour
{

    [SerializeField] private GameObject objectToDisable;

    [SerializeField] private BulletColor currentColor;

    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private ParticleSystem idleParticles;
    [SerializeField] private ParticleSystem pickUpParticles;

    [SerializeField] private bool isWhite = false;

    [SerializeField] private GameObject text;

    private AudioSource audio;
    
    private Collider2D collider2d;
    
    private void Awake()
    {
        collider2d = GetComponent<Collider2D>();
        audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (!isWhite)
        {
            Color color = GameManager.instance.colors[(int) currentColor];
            renderer.color = color;
            var mainNode = idleParticles.main;
            mainNode.startColor = color;

            var mainNode2 = pickUpParticles.main;
            mainNode2.startColor = color;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {

            PlayerActions player = collision.GetComponent<PlayerActions>();
            player.enabled = true;
            player.PassToNextLevel();
            player.isWhite = false;
            audio.Play();
            collider2d.enabled = false;
            renderer.enabled = false;
            idleParticles.Stop();
            pickUpParticles.Play();
            text.SetActive(false);
            GameEventSystem.instance.ShakeCamera();
           
        }
    }
}
