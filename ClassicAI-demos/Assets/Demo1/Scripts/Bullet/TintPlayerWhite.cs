using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TintPlayerWhite : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {

            PlayerActions player = collision.GetComponent<PlayerActions>();
            player.GetWhite();
            
        }
    }
}
