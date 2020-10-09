using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimatorStateMachineUtil;

public class FSM_Skeleton : MonoBehaviour
{
    #region variables

    [SerializeField] private SkelletonBehaviour skeletonBehaviour;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float leftLimit;
    [SerializeField] private float rightLimit;

    private Animator animator;
    private Vector3 playerPosition;
    private bool attacking;
    private bool jawAttack;

    #endregion variables

    private void Awake()
    {
        animator = GetComponent<Animator>();

        attacking = false;
        jawAttack = false;
    }

    private void Update()
    {
        playerPosition = GameManager.instance.player.transform.position;
        if(!attacking) {
            if(playerPosition.x < leftLimit || playerPosition.x > rightLimit) {
                animator.SetTrigger("handsAttack");
            }
            else if(jawAttack) {
                animator.SetTrigger("mouthAttack");
            }
            else {
                animator.SetTrigger("projectilesAttack");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerActions>() && !attacking) {
            jawAttack = true;
        }
    }

    private IEnumerator BackToIdle()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);

        animator.SetTrigger("backIdle");
    }

    [StateEnterMethod("Base.Idle")]
    public void EnterIdle()
    {
        animator.ResetTrigger("mouthAttack");
        animator.ResetTrigger("handsAttack");
        animator.ResetTrigger("projectilesAttack");
        attacking = false;
    }

    [StateEnterMethod("Base.Jaw attack")]
    public void EnterJawAttack()
    {
        attacking = true;
        StartCoroutine(BackToIdle());
    }

    [StateExitMethod("Base.Jaw attack")]
    public void ExitJawAttack()
    {
        jawAttack = false;
    }

    [StateEnterMethod("Base.Hands attack")]
    public void EnterHandsAttack()
    {
        attacking = true;
        skeletonBehaviour.HandsAttack();
        StartCoroutine(BackToIdle());
    }

    [StateEnterMethod("Base.Projectiles")]
    public void EnterProjectiles()
    {
        attacking = true;
        StartCoroutine(skeletonBehaviour.ProjectilesAttack());
        StartCoroutine(BackToIdle());
    }
}