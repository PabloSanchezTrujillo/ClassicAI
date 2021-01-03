using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AnimatorStateMachineUtil;

public class FSM_Angel : MonoBehaviour
{
    #region variables

    [SerializeField] private AngelBehaviour angelBehaviour;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float shortDistance;
    [SerializeField] private float longDistance;
    [SerializeField] private Text distanceText;

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
        float distance = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);
        distanceText.text = distance.ToString();

        // Selects an attack depending on the distance to the player and if it is not already attacking
        if(!attacking) {
            if(distance < shortDistance) {
                animator.SetTrigger("BallsAttack");
            }
            else if(distance > longDistance) {
                animator.SetTrigger("FeathersAttack");
            }
            else {
                animator.SetTrigger("BallsAttack");
            }
        }
    }

    /// <summary>
    /// Resets the boss state to the Idle state after some seconds and resets all the triggers in the state machine
    /// </summary>
    public IEnumerator BackToIdle()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);

        animator.ResetTrigger("BallsAttack");
        animator.ResetTrigger("FeatherAttack");
        animator.ResetTrigger("LaserAttack");
        animator.SetTrigger("backToIdle");
    }

    /// <summary>
    /// Idle state, enables the next attack
    /// </summary>
    [StateEnterMethod("Base.Idle")]
    public void EnterIdle()
    {
        attacking = false;
    }

    /// <summary>
    /// Laser attack state
    /// </summary>
    [StateEnterMethod("Base.Laser attack")]
    public void EnterLaserAttack()
    {
        attacking = true; // Disables any other attack until this one is finished
        angelBehaviour.TriggerLaserAttack();

        StartCoroutine(BackToIdle());
    }

    /// <summary>
    /// Balls attack state
    /// </summary>
    [StateEnterMethod("Base.Balls attack")]
    public void EnterBallsAttack()
    {
        attacking = true; // Disables any other attack until this one is finished
        angelBehaviour.TriggerBallsAttack();

        StartCoroutine(BackToIdle());
    }

    /// <summary>
    /// Feathers attack state
    /// </summary>
    [StateEnterMethod("Base.Feathers attack")]
    public void EnterFeathersAttack()
    {
        attacking = true; // Disables any other attack until this one is finished
        angelBehaviour.TriggersFeathersAttack();

        StartCoroutine(BackToIdle());
    }
}