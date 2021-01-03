using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimatorStateMachineUtil;
using UnityEngine.UI;

public class FSM_BlackHole : MonoBehaviour
{
    #region variables

    [SerializeField] private Sucker holeBehaviour;
    [SerializeField] private float shortDistance;
    [SerializeField] private float longDistance;
    [SerializeField] private Text distanceText;
    [SerializeField] private float timeBetweenAttacks;

    private Animator animator;
    private bool attacking;
    private bool orbitsAttack;

    #endregion variables

    private void Awake()
    {
        animator = GetComponent<Animator>();

        attacking = false;
        orbitsAttack = false;
    }

    private void Update()
    {
        float distance = Vector3.Distance(this.transform.position, GameManager.instance.player.transform.position);
        distanceText.text = distance.ToString();

        // Selects an attack depending on the distance to the player and if it is not already attacking
        if(!attacking) {
            if(distance < shortDistance) {
                animator.SetTrigger("bouncersAttack");
            }
            else if(distance > longDistance && !orbitsAttack) {
                animator.SetTrigger("orbitsAttack");
            }
            else {
                animator.SetTrigger("suckingAttack");
            }
        }
    }

    /// <summary>
    /// Returns the boss state back to the Idle state after some seconds and resets all the triggers in the state machine
    /// </summary>
    private IEnumerator BackToIdle()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);

        animator.ResetTrigger("suckingAttack");
        animator.ResetTrigger("orbitsAttack");
        animator.ResetTrigger("bouncersAttack");
        animator.SetTrigger("backToIdle");
    }

    /// <summary>
    /// Returns the boss state back to the Idle state after some fixed seconds and resets all the triggers in the state machine.
    /// This method is necessary for the Big attack state because the animation and the attack duration does not match,
    /// the 27 seconds allow the boss to finish the attack and move back to the center of the screen
    /// </summary>
    /// <returns></returns>
    private IEnumerator BackToIdleFixed()
    {
        yield return new WaitForSeconds(27);

        animator.ResetTrigger("suckingAttack");
        animator.ResetTrigger("orbitsAttack");
        animator.ResetTrigger("bouncersAttack");
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
    /// Balls attack state
    /// </summary>
    [StateEnterMethod("Base.Balls center")]
    public void EnterBallsCenter()
    {
        attacking = true; // Disables any other attack until this one is finished
        orbitsAttack = false; // Disables the orbits attack
        holeBehaviour.SuckingAttack();

        StartCoroutine(BackToIdle());
    }

    /// <summary>
    /// Bouncing balls attack state
    /// </summary>
    [StateEnterMethod("Base.Bouncing balls")]
    public void EnterBouncingBalls()
    {
        attacking = true; // Disables any other attack until this one is finished
        orbitsAttack = false; // Disables the orbits attack
        holeBehaviour.BouncesAttack();

        StartCoroutine(BackToIdle());
    }

    /// <summary>
    /// Big attack state
    /// </summary>
    [StateEnterMethod("Base.Big attack")]
    public void EnterBigAttack()
    {
        attacking = true; // Disables any other attack until this one is finished
        orbitsAttack = true; // Enables the big attack again, avoiding problems with the boss position
        holeBehaviour.OrbitsAttack();

        StartCoroutine(BackToIdleFixed());
    }
}