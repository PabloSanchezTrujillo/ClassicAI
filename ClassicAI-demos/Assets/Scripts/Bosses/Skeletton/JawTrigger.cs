using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JawTrigger : MonoBehaviour
{
    #region variables

    [SerializeField] private FSM_Skeleton FSMskeleton;

    #endregion variables

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerActions>()) {
            FSMskeleton.JawAttack = true;
        }
    }
}