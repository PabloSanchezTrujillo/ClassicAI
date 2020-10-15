using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnBulletHit : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        Bullet bullet = other.GetComponent<Bullet>();

        if (bullet)
        {
            bullet.DestroyOnHit(false);
            gameObject.SetActive(false);
        }
    }
}
