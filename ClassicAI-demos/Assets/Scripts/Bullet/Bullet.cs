using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    public BulletColor currentColor;

    [SerializeField] private float speed = 6.0f;
    
    private Vector3 direction;
    
    private Rigidbody2D rb;

    private SpriteRenderer renderer;

    [SerializeField]private ParticleSystem particles;

    [SerializeField]private ParticleSystem explosionBoss;
    [SerializeField]private ParticleSystem explosionProjectiles;
    
    private bool hit = false;

    private Collider2D collider;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!hit)
            rb.MovePosition(transform.position + direction * (speed * Time.fixedDeltaTime) );
    }

    public void Init(Vector3 direction, BulletColor currentColor)
    {
        this.direction = direction;
        this.currentColor = currentColor;
        
        Color color = GameManager.instance.colors[(int) currentColor];
        renderer.color = color;
        var mainNode = particles.main;
        mainNode.startColor = color;
    }

    public void DestroyOnHit(bool isBoss)
    {

        hit = true;

        collider.enabled = false;
        
        particles.Stop();
        
        Color color = GameManager.instance.colors[(int) currentColor];
        
        renderer.enabled = false;

        if (isBoss)
        {
            var mainNode = explosionBoss.main;
            mainNode.startColor = color;
            explosionBoss.Play();
        }
        else
        {
            var mainNode = explosionProjectiles.main;
            mainNode.startColor = color;
            explosionProjectiles.Play();
        }

        
        StartCoroutine(DelayedDestroy());
        
    }

    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
