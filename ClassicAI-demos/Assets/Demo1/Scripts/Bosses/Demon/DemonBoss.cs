using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class DemonBoss : MonoBehaviour
{
    #region variables

    [Header("General")]
    [SerializeField] private float timeForFirstAttack;

    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private DemonSoundManager demonSoundManager;

    [Range(0, 1)]
    [SerializeField] private float firstAttackThreshold;

    [Range(0, 1)]
    [SerializeField] private float secondAttackThreshold;

    [Range(0, 1)]
    [SerializeField] private float thirdAttackThreshold;

    [Header("Tridents")]
    [SerializeField] private GameObject[] tridents;

    [SerializeField] private Transform[] tridentStartPosition;
    [SerializeField] private float timeBetweenTridents;

    [Header("Death balls")]
    [SerializeField] private GameObject[] deathBalls;

    [SerializeField] private Transform[] deathBallsPosition;
    [SerializeField] private float timeBetweenDeathBalls;

    [Header("V bullets")]
    [SerializeField] private GameObject vBullet;

    [SerializeField] private Transform[] vBulletsPositions;
    [SerializeField] private int vBulletNumber;
    [SerializeField] private float timeBetweenVBullets;
    [SerializeField] private Animator animator;

    private Boss boss;
    private bool died;

    #endregion variables

    private void Awake()
    {
        //animator = GetComponentInChildren<Animator>();
        boss = GetComponent<Boss>();
        died = false;
    }

    // Start is called before the first frame update
    private void Start()
    {
        //InvokeRepeating("ThrowDice", timeForFirstAttack, timeBetweenAttacks);
    }

    private void Update()
    {
        if(boss.currentHealth <= 0 && !died) {
            died = true;
            demonSoundManager.DemonDeath();
            StopAllCoroutines();
            foreach(GameObject trident in tridents) {
                Destroy(trident.gameObject);
            }
            foreach(GameObject deathBall in deathBalls) {
                Destroy(deathBall.gameObject);
            }
        }
    }

    /*
    private void ThrowDice()
    {
        if(GetComponent<Boss>().currentHealth <= 0)
            return;

        float dice = Random.value;

        if(dice <= firstAttackThreshold) {
            StartCoroutine(TridentAttack());
        }
        else if(dice > firstAttackThreshold && dice <= secondAttackThreshold) {
            StartCoroutine(DeathBallAttack());
        }
        else {
            StartCoroutine(VBulletAttack());
        }
    }
    */

    public IEnumerator TridentAttack()
    {
        animator.SetTrigger("Attack1");
        demonSoundManager.Attack1Sound();
        List<GameObject> tridentsList = tridents.ToList();
        List<Transform> positionsList = tridentStartPosition.ToList();

        for(int i = 0; i < tridents.Length; i++) {
            int randomIndex = Random.Range(0, tridentsList.Count);
            GameObject trident = tridentsList[randomIndex];
            Transform startPosition = positionsList[randomIndex];
            tridentsList.RemoveAt(randomIndex);
            positionsList.RemoveAt(randomIndex);

            yield return new WaitForSeconds(timeBetweenTridents);

            trident.transform.position = startPosition.position;
            trident.GetComponent<Trident>().StartShaking();
        }
    }

    public IEnumerator DeathBallAttack()
    {
        animator.SetTrigger("Attack2");
        demonSoundManager.Attack2Sound();

        for(int i = 0; i < deathBalls.Length; i++) {
            //deathBalls[i].transform.position = deathBallsPosition[i].position;
            deathBalls[i].GetComponent<DeathBall>().SetNotShooted(true);
            deathBalls[i].GetComponent<Animator>().SetTrigger("Appear");
        }

        List<GameObject> deathBallsList = deathBalls.ToList();
        for(int i = 0; i < deathBalls.Length; i++) {
            int randomIndex = Random.Range(0, deathBallsList.Count);

            yield return new WaitForSeconds(timeBetweenDeathBalls);

            if(deathBallsList[randomIndex].GetComponent<DeathBall>() != null) {
                deathBallsList[randomIndex].GetComponent<DeathBall>().ShootDeathBall();
                deathBallsList.RemoveAt(randomIndex);
            }
        }
    }

    public IEnumerator VBulletAttack()
    {
        animator.SetTrigger("Attack3");
        demonSoundManager.Attack3Sound();

        for(int i = 0; i < vBulletNumber; i++) {
            int randomIndex = Random.Range(0, vBulletsPositions.Length);

            yield return new WaitForSeconds(timeBetweenVBullets);

            GameObject vBulletObject = Instantiate(vBullet, vBulletsPositions[randomIndex].position, vBulletsPositions[randomIndex].rotation);
            vBulletObject.GetComponent<VBullet>().ShootVBullet();
        }
    }
}