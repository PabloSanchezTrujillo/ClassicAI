using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Boss : MonoBehaviour
{
    [SerializeField] public int currentHealth;
    [SerializeField] private int maxHealth;

    [SerializeField] private int hits = 0;

    [SerializeField] private Text healthText;

    [SerializeField] private bool isDebug = true;

    [SerializeField] private BulletColor currentColor;

    [SerializeField] private SpriteRenderer[] bossSpritesToChangeColor;

    [SerializeField] private int higherColorCanGet = 0;
    [SerializeField] private float timeToChangeCurrentColor = 6.0f;

    [SerializeField] private float delayedTimeForShowDissolve = 6.0f;

    private Bullet lastBullet;

    private GameObject boss;

    [SerializeField] private Animator bossAnimator;
    [SerializeField] private SpriteRenderer bossRenderer;

    private GameObject player;

    [SerializeField] private AudioSource audioTakenDamage;

    private void Awake()
    {
        boss = transform.GetChild(0).gameObject;
    }

    // Start is called before the first frame update
    private void Start()
    {
        player = GameManager.instance.player;

        currentHealth = maxHealth;
        if(isDebug) {
            healthText.text = currentHealth.ToString();
            healthText.color = GameManager.instance.colors[(int)currentColor];
        }

        TintBodyParts(currentColor);

        StartCoroutine(ChangeColorOverTime());
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Bullet bullet = other.GetComponent<Bullet>();

        if(bullet && lastBullet != bullet) {
            lastBullet = bullet;

            BulletColor bulletColor = currentColor;
            if(bullet.currentColor == bulletColor && currentHealth > 0) {
                Debug.Log("Hit");
                currentHealth--;
                hits++;

                GameEventSystem.instance.ShakeCamera();

                audioTakenDamage.Play();

                if(currentHealth > 0) {
                    if(isDebug) {
                        healthText.color = GameManager.instance.colors[(int)currentColor];
                        healthText.text = currentHealth.ToString();
                    }

                    StartCoroutine(BeingHitAnim());
                }
                else if(currentHealth <= 0) {
                    if(isDebug) {
                        healthText.color = Color.white;
                        healthText.text = currentHealth.ToString();
                    }
                    GameEventSystem.instance.BossDeath();
                    bossAnimator.SetTrigger("death");
                    StartCoroutine(DelayedStartDissolver());
                    player.GetComponent<PlayerActions>().InmortalityMode();
                }

                bullet.DestroyOnHit(true);
            }
            else {
                bullet.DestroyOnHit(false);
            }
        }
    }

    private IEnumerator ChangeColorOverTime()
    {
        while(currentHealth > 0) {
            yield return new WaitForSeconds(timeToChangeCurrentColor);
            if(currentHealth > 0) {
                currentColor = PickTintColor();
                TintBodyParts(currentColor);
            }
        }
    }

    private BulletColor PickTintColor()
    {
        int index = Random.Range(0, higherColorCanGet + 1);
        return (BulletColor)index;
    }

    private void TintBodyParts(BulletColor currentColor)
    {
        Color color = GameManager.instance.colors[(int)currentColor];
        foreach(SpriteRenderer renderer in bossSpritesToChangeColor) {
            renderer.color = color;
        }
    }

    private IEnumerator BeingHitAnim()
    {
        for(int i = 0; i < 8; i++) {
            bossRenderer.enabled = !bossRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator DelayedStartDissolver()
    {
        yield return new WaitForSeconds(delayedTimeForShowDissolve);
        GameEventSystem.instance.StartDissolve();
        yield return new WaitForSeconds(1.0f);

        if(player) {
            GameEventSystem.instance.LoadNextLevel();
        }
    }

    public BulletColor GetCurrentColor()
    {
        return currentColor;
    }
}