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

    /// <summary>
    /// Sets the boss state back to the idle state after some seconds
    /// </summary>
    private IEnumerator BackToIdle()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);

        animator.SetTrigger("backIdle");
    }

    /// <summary>
    /// Sets the boss state back to the idle state after some fixed seconds.
    /// Those fixed seconds prevent the boss to do another attack while the animation is still running.
    /// This is neccesary because the attack duration is different to the animation duration
    /// </summary>
    private IEnumerator BackToIdleFixed()
    {
        yield return new WaitForSeconds(2);

        animator.SetTrigger("backIdle");
    }

    /// <summary>
    /// Idle state. Resets all the triggers in the state machine and allows another attack
    /// </summary>
    [StateEnterMethod("Base.Idle")]
    public void EnterIdle()
    {
        animator.ResetTrigger("mouthAttack");
        animator.ResetTrigger("handsAttack");
        animator.ResetTrigger("projectilesAttack");
        attacking = false;
    }

    /// <summary>
    /// Jaw attack state
    /// </summary>
    [StateEnterMethod("Base.Jaw attack")]
    public void EnterJawAttack()
    {
        attacking = true; // Disables any other attack until this one is finished
        jawCollider.enabled = true;
        skeletonBehaviour.JawAttack();

        StartCoroutine(BackToIdleFixed());
    }

    /// <summary>
    /// Jaw attack state
    /// </summary>
    [StateExitMethod("Base.Jaw attack")]
    public void ExitJawAttack()
    {
        JawAttack = false; // Disables any other attack until this one is finished
        jawCollider.enabled = false;
    }

    /// <summary>
    /// Base hands attack state
    /// </summary>
    [StateEnterMethod("Base.Hands attack")]
    public void EnterHandsAttack()
    {
        attacking = true; // Disables any other attack until this one is finished
        skeletonBehaviour.HandsAttack();

        StartCoroutine(BackToIdle());
    }

    /// <summary>
    /// Projectiles attack state
    /// </summary>
    [StateEnterMethod("Base.Projectiles")]
    public void EnterProjectiles()
    {
        attacking = true; // Disables any other attack until this one is finished
        StartCoroutine(skeletonBehaviour.ProjectilesAttack());

        StartCoroutine(BackToIdle());
    }
}