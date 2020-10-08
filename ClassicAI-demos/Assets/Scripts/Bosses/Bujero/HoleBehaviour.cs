using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HoleBehaviour : MonoBehaviour
{

    [SerializeField] private Sucker suckerBoss;

    [SerializeField] private Animator animator;

    [SerializeField] private AudioSource soundsPlayer;

    [SerializeField] private AudioClip attack1Sound;
    [SerializeField] private AudioClip attack2Sound;

    [SerializeField] private AudioClip deathSound;

    //si esta quitecito atacando
    public bool isIdle = false;

    private bool isAlive = true;
    private bool isDying = false;
    private bool rocksDestroyed = false;

    [SerializeField] private float maxPercentageRandomAttack = 100.0f;

    [SerializeField] private float attackSuckingTreshold;
    [SerializeField] private float attackOrbitsTreshold;
    [SerializeField] private float attackBouncersTreshold;

    [SerializeField] private float timeBtwAttacks = 4.0f;

    private Boss boss;

    private void Awake()
    {
        boss = GetComponent<Boss>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AttackLoop());
    }

    // Update is called once per frame
    void Update()
    {
        isAlive = boss.currentHealth > 0.0f;
        isIdle = animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && isAlive;
        isDying = animator.GetCurrentAnimatorStateInfo(0).IsName("Muerte");

        if (isDying && !rocksDestroyed)
        {
            StopAllCoroutines();

            foreach (Boulder rock in FindObjectsOfType<Boulder>())
            {
                rock.DestroyWithFade();
            }

            rocksDestroyed = true;
        }
    }

    IEnumerator AttackLoop()
    {
        yield return new WaitUntil(() => isIdle);

        while (isAlive)
        {
            //tirar dado
            float value = Random.Range(0.0f, maxPercentageRandomAttack);

            if (value >= attackSuckingTreshold && value < attackOrbitsTreshold)
            {
                animator.SetTrigger("suckingAttack");

                yield return suckerBoss.SuckingAttackWithWait();
            }
            else if (value >= attackOrbitsTreshold && value < attackBouncersTreshold)
            {
                animator.SetTrigger("orbitsAttack");

                yield return suckerBoss.OrbitalsAttackWithWait();
            }
            else if (value >= attackBouncersTreshold)
            {
                animator.SetTrigger("bouncersAttack");

                yield return suckerBoss.BouncersAttackWithWait();
            }

            animator.SetTrigger("backToIdle");

            yield return new WaitForSeconds(timeBtwAttacks);
        }

        rocksDestroyed = false;
    }

    void PlayAttack1Sound()
    {
        if (soundsPlayer.isPlaying)
            return;

        soundsPlayer.clip = attack1Sound;
        soundsPlayer.Play();
    }

    void PlayAttack2Sound()
    {
        if (soundsPlayer.isPlaying)
            return;

        soundsPlayer.clip = attack2Sound;
        soundsPlayer.Play();
    }

    void PlayDeathSound()
    {
        soundsPlayer.clip = deathSound;
        soundsPlayer.Play();
    }
}
