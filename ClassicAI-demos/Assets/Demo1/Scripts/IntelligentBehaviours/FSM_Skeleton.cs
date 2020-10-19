using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimatorStateMachineUtil;

public class FSM_Skeleton : MonoBehaviour
{
    #region variables

    public bool JawAttack { get; set; }

    [SerializeField] private SkelletonBehaviour skeletonBehaviour;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float leftLimit;
    [SerializeField] private float rightLimit;
    [SerializeField] private EdgeCollider2D jawCollider;

    private Animator animator;
    private Vector3 playerPosition;
    private bool attacking;

    #endregion variables

    private void Awake()
    {
        animator = GetComponent<Animator>();

        attacking = false;
        JawAttack = false;
    }

    private void Update()
    {
        playerPosition = GameManager.instance.player.transform.position;
        if(!attacking) {
            if(playerPosition.x < leftLimit || playerPosition.x > rightLimit) {
                animator.SetTrigger("handsAttack");
            }
            else if(JawAttack) {
                animator.SetTrigger("mouthAttack");
            }
            else {
                animator.SetTrigger("projectilesAttack");
            }
        }
    }

    private IEnumerator BackToIdle()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);

        animator.SetTrigger("backIdle");
    }

    private IEnumerator BackToIdleFixed()
    {
        yield return new WaitForSeconds(2);

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
        jawCollider.enabled = true;
        skeletonBehaviour.JawAttack();
        StartCoroutine(BackToIdleFixed());
    }

    [StateExitMethod("Base.Jaw attack")]
    public void ExitJawAttack()
    {
        JawAttack = false;
        jawCollider.enabled = false;
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