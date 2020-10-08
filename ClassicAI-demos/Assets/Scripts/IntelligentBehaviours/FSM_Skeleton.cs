using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimatorStateMachineUtil;

public class FSM_Skeleton : MonoBehaviour
{
    #region variables

    [SerializeField] private SkelletonBehaviour skeletonBehaviour;

    private Animator animator;

    #endregion variables

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerActions>()) {
            skeletonBehaviour.JawAttack();
        }
    }

    [StateEnterMethod("Base.Mouth attack")]
    public void MouthAttack()
    {
        print("Jaw attack");
    }
}