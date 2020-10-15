using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BulletSkelleton : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float speed;

    private Vector3 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject player = GameManager.instance.player;
        
        if (player)
        {
            direction = (player.transform.position - transform.position);
            direction.z = 0;
            direction = direction.normalized;
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + direction * (speed * Time.fixedDeltaTime));
    }
    
}
