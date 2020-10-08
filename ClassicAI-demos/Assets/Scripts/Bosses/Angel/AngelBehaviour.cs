using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AngelBehaviour : MonoBehaviour
{

    [SerializeField] private GameObject bossGameObject;

    [SerializeField] private Animator animator;

    //si esta quitecito atacando
    public bool isIdle = false;

    private bool firstTime = false;
    private bool isAlive = true;

    [SerializeField] private float maxPorcentageRandomAttack = 100.0f;

    [SerializeField] private float attackLasersTreshold;
    [SerializeField] private float attackFeathersTreshold;
    [SerializeField] private float attackDiscsTreshold;

    [SerializeField] private float timeBtwAttacks = 4.0f;

    [SerializeField] private GameObject LaserAttack;
    [SerializeField] private GameObject FeatherAttack;
    [SerializeField] private GameObject DiscAttack;

    private GameObject player;
    private Boss boss;

    private void Awake()
    {
        boss = GetComponent<Boss>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        isAlive = boss.currentHealth > 0.0f;
        isIdle = animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && isAlive;

        if (isIdle)
        {
            FollowPlayer();
            if (!firstTime)
            {
                firstTime = true;
                StartCoroutine(AttackLoop());
            }
        }
    }

    private void FollowPlayer()
    {
        if (player)
        {
            float deltaX = player.transform.position.x - bossGameObject.transform.position.x;
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.right * deltaX, 0.01f);
        }
    }

    IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(1);
        while (isAlive)
        {
            //tirar dado
            float value = Random.Range(0.0f, maxPorcentageRandomAttack);


            if (value >= attackLasersTreshold && value < attackFeathersTreshold)
            {
                Instantiate(LaserAttack, new Vector3(0, transform.position.y + 1, transform.position.z), Quaternion.identity);
                yield return new WaitForSeconds(7);
            }
            else if (value >= attackFeathersTreshold && value < attackDiscsTreshold)
            {
                animator.SetTrigger("MakeAttack2");
                Instantiate(FeatherAttack, new Vector3(-0.5f, transform.position.y + 1, transform.position.z), Quaternion.identity);
                yield return new WaitForSeconds(7);
            }
            else if (value >= attackDiscsTreshold)
            {
                animator.SetTrigger("MakeAttack3");
                Instantiate(DiscAttack, new Vector3(0, transform.position.y, transform.position.z), Quaternion.identity);
                yield return new WaitForSeconds(5);
            }


            yield return new WaitForSeconds(timeBtwAttacks);
        }
    }
}

