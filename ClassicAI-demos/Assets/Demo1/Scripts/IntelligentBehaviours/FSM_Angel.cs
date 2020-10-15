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

    // Update is called once per frame
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);
        distanceText.text = distance.ToString();

        if(!attacking) {
            if(distance < shortDistance) {
                animator.SetTrigger("BallsAttack");
            }
            else if(distance > longDistance) {
                animator.SetTrigger("FeathersAttack");
            }
            else {
                animator.SetTrigger("LaserAttack");
            }
        }
    }

    public IEnumerator BackToIdle()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);

        animator.ResetTrigger("BallsAttack");
        animator.ResetTrigger("FeatherAttack");
        animator.ResetTrigger("LaserAttack");
        animator.SetTrigger("backToIdle");
    }

    [StateEnterMethod("Base.Idle")]
    public void EnterIdle()
    {
        attacking = false;
    }

    [StateEnterMethod("Base.Laser attack")]
    public void EnterLaserAttack()
    {
        attacking = true;
        angelBehaviour.TriggerLaserAttack();

        StartCoroutine(BackToIdle());
    }

    [StateEnterMethod("Base.Balls attack")]
    public void EnterBallsAttack()
    {
        attacking = true;
        angelBehaviour.TriggerBallsAttack();

        StartCoroutine(BackToIdle());
    }

    [StateEnterMethod("Base.Feathers attack")]
    public void EnterFeathersAttack()
    {
        attacking = true;
        angelBehaviour.TriggersFeathersAttack();

        StartCoroutine(BackToIdle());
    }
}