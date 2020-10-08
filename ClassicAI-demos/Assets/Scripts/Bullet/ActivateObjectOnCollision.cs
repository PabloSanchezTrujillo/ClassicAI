using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObjectOnCollision : MonoBehaviour
{

    [SerializeField] private GameObject objectToActivate;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {

            objectToActivate.SetActive(true);
           
        }
    }
}
