using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerPerception : MonoBehaviour
{
    #region variables

    [SerializeField] private PlayerActions playerActions;

    #endregion variables

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Damage")) {
            playerActions.AttackingObject = collision.gameObject;
            playerActions.AttackIncoming = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Damage")) {
            playerActions.AttackIncoming = false;
        }
    }
}