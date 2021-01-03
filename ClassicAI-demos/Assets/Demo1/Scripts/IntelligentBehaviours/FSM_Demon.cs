using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimatorStateMachineUtil;
using UnityEngine.UI;

public class FSM_Demon : MonoBehaviour
{
    #region variables

    [SerializeField] private DemonBoss demonBehaviour;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float shortDistance;
    [SerializeField] private float longDistance;
    [SerializeField] private Text distanceText;

    private Vector3 playerPosition;
    private bool attacking;
    private Animator animator;

    #endregion variables

    private void Awake()
    {
        animator = GetComponent<Animator>();

        attacking = false;
    }

    private void Update()
    {
        playerPosition = GameManager.instance.player.transform.position;
        float distance = Vector3.Distance(playerPosition, transform.position);
        distanceText.text = distance.ToString();

        if(!attacking) {
            if(distance < shortDistance) {
                animator.SetTrigger("ballsAttack");
            }
            else if(distance > longDistance) {
                animator.SetTrigger("tridentAttack");
            }
            else {
                animator.SetTrigger("vAttack");
            }
        }
    }

    /// <summary>
    /// Sets the boss state back to the Idle state after some seconds
    /// </summary>
    private IEnumerator BackToIdle()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);

        animator.SetTrigger("backIdle");
    }

    /// <summary>
    /// Idle state, enables the next attack
    /// </summary>
    [StateEnterMethod("Base.Idle")]
    public void EnterIdle()
    {
        animator.ResetTrigger("ballsAttack");
        animator.ResetTrigger("tridentAttack");
        animator.ResetTrigger("vAttack");
        attacking = false;
    }

    /// <summary>
    /// Tridents attack state
    /// </summary>
    [StateEnterMethod("Base.Tridents attack")]
    public void EnterTridentsAttack()
    {
        attacking = true; // Disables any other attack until this one is finished
        StartCoroutine(demonBehaviour.TridentAttack());

        StartCoroutine(BackToIdle());
    }

    /// <summary>
    /// Balls attack state
    /// </summary>
    [StateEnterMethod("Base.Balls attack")]
    public void EnterBallsAttack()
    {
        attacking = true; // Disables any other attack until this one is finished
        StartCoroutine(demonBehaviour.DeathBallAttack());

        StartCoroutine(BackToIdle());
    }

    /// <summary>
    /// V attack state
    /// </summary>
    [StateEnterMethod("Base.V attack")]
    public void EnterVAttack()
    {
        attacking = true; // Disables any other attack until this one is finished
        StartCoroutine(demonBehaviour.VBulletAttack());

        StartCoroutine(BackToIdle());
    }
}