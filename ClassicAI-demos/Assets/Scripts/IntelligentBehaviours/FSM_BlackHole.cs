﻿using System.Collections;
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

    // Update is called once per frame
    private void Update()
    {
        float distance = Vector3.Distance(this.transform.position, GameManager.instance.player.transform.position);
        distanceText.text = distance.ToString();

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

    private IEnumerator BackToIdle()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);

        animator.ResetTrigger("suckingAttack");
        animator.ResetTrigger("orbitsAttack");
        animator.ResetTrigger("bouncersAttack");
        animator.SetTrigger("backToIdle");
    }

    private IEnumerator BackToIdleFixed()
    {
        yield return new WaitForSeconds(27);

        animator.ResetTrigger("suckingAttack");
        animator.ResetTrigger("orbitsAttack");
        animator.ResetTrigger("bouncersAttack");
        animator.SetTrigger("backToIdle");
    }

    [StateEnterMethod("Base.Idle")]
    public void EnterIdle()
    {
        attacking = false;
    }

    [StateEnterMethod("Base.Balls center")]
    public void EnterBallsCenter()
    {
        attacking = true;
        orbitsAttack = false;
        holeBehaviour.SuckingAttack();

        StartCoroutine(BackToIdle());
    }

    [StateEnterMethod("Base.Bouncing balls")]
    public void EnterBouncingBalls()
    {
        attacking = true;
        orbitsAttack = false;
        holeBehaviour.BouncesAttack();

        StartCoroutine(BackToIdle());
    }

    [StateEnterMethod("Base.Big attack")]
    public void EnterBigAttack()
    {
        attacking = true;
        orbitsAttack = true;
        holeBehaviour.OrbitsAttack();

        StartCoroutine(BackToIdleFixed());
    }
}