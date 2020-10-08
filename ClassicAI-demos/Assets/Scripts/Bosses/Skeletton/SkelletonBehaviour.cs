using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkelletonBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject bossGameObject;

    [SerializeField] private Animator animator;

    //si esta quitecito atacando
    public bool isIdle = false;

    private bool firstTime = false;
    private bool isAlive = true;

    [SerializeField] private float maxPorcentageRandomAttack = 100.0f;

    [SerializeField] private float attackProjectilesTreshold;
    [SerializeField] private float attackJawTreshold;
    [SerializeField] private float attackHandsTreshold;

    [SerializeField] private float timeBtwAttacks = 4.0f;

    [SerializeField] private GameObject projectilesSpawnerPrefab;
    [SerializeField] private GameObject handsAttackPrefab;

    [SerializeField] private Transform projectilesSpawnPoint;
    [SerializeField] private Transform handsSpawnPoint;

    [SerializeField] private AudioSource castingHandsAudio;

    private GameObject player;
    private Boss boss;

    private void Awake()
    {
        boss = GetComponent<Boss>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        player = GameManager.instance.player;
    }

    // Update is called once per frame
    private void Update()
    {
        isAlive = boss.currentHealth > 0.0f;
        isIdle = animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && isAlive;

        if(isIdle) {
            FollowPlayer();
            if(!firstTime) {
                firstTime = true;
                //StartCoroutine(AttackLoop());
            }
        }
    }

    private void FollowPlayer()
    {
        if(player) {
            float deltaX = player.transform.position.x - bossGameObject.transform.position.x;
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.right * deltaX, 0.1f);
        }
    }

    private IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(1);
        while(isAlive) {
            //tirar dado
            float value = Random.Range(0.0f, maxPorcentageRandomAttack);

            if(value >= attackProjectilesTreshold && value < attackJawTreshold) {
                if(player) {
                    animator.SetTrigger("projectilesAttack");
                    yield return new WaitForSeconds(1.0f);
                    GameObject bulletSpawner = Instantiate(projectilesSpawnerPrefab, projectilesSpawnPoint.position,
                        Quaternion.identity);
                }
            }
            else if(value >= attackJawTreshold && value < attackHandsTreshold) {
                animator.SetTrigger("mouthAttack");
            }
            else if(value >= attackHandsTreshold) {
                int numHandAttacks = Random.Range(1, 3);

                castingHandsAudio.Play();

                GameObject handObject = Instantiate(handsAttackPrefab, handsSpawnPoint.position, Quaternion.identity);
                HandAttack handAttack = handObject.GetComponent<HandAttack>();
                handAttack.Init(numHandAttacks);
            }

            yield return new WaitForSeconds(timeBtwAttacks);
        }
    }

    public IEnumerator ProjectilesAttack()
    {
        animator.SetTrigger("projectilesAttack");
        yield return new WaitForSeconds(1.0f);
        GameObject bulletSpawner = Instantiate(projectilesSpawnerPrefab, projectilesSpawnPoint.position,
            Quaternion.identity);
    }

    public void JawAttack()
    {
        animator.SetTrigger("mouthAttack");
    }

    public void HandsAttack()
    {
        int numHandAttacks = Random.Range(1, 3);

        castingHandsAudio.Play();

        GameObject handObject = Instantiate(handsAttackPrefab, handsSpawnPoint.position, Quaternion.identity);
        HandAttack handAttack = handObject.GetComponent<HandAttack>();
        handAttack.Init(numHandAttacks);
    }
}