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

    // Update is called once per frame
    private void Update()
    {
        playerPosition = GameManager.instance.player.transform.position;
        float distance = Vector3.Distance(transform.position, playerPosition);
        distanceText.text = distance.ToString();

        if(!attacking) {
            if(distance < shortDistance) {
                animator.SetTrigger("Attack2");
            }
            else if(distance > longDistance) {
                animator.SetTrigger("Attack1");
            }
            else {
                animator.SetTrigger("Attack3");
            }
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
        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");
        animator.ResetTrigger("Attack3");
        attacking = false;
    }

    [StateEnterMethod("Base.Tridents attack")]
    public void EnterTridentsAttack()
    {
        attacking = true;
        StartCoroutine(demonBehaviour.TridentAttack());
        StartCoroutine(BackToIdle());
    }

    [StateEnterMethod("Base.Balls attack")]
    public void EnterBallsAttack()
    {
        attacking = true;
        StartCoroutine(demonBehaviour.DeathBallAttack());
        StartCoroutine(BackToIdle());
    }

    [StateEnterMethod("Base.V attack")]
    public void EnterVAttack()
    {
        attacking = true;
        StartCoroutine(demonBehaviour.VBulletAttack());
        StartCoroutine(BackToIdle());
    }
}