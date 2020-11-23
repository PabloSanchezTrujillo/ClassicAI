using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;
using System.Configuration;

public class PlayerActions : MonoBehaviour
{
    #region variables

    public bool AttackIncoming { get; set; }
    public GameObject AttackingObject { get; set; }

    [Header("Camera shaking parameters")]
    [SerializeField] private float magnitude;

    [SerializeField] private float roughness;
    [SerializeField] private float fadeInTime;
    [SerializeField] private float fadeOutTime;

    [Header("Player parameters")]
    [SerializeField] private GameObject bulletSpawner;

    [SerializeField] private GameObject bullet;
    [SerializeField] private AnimationClip damagedAnimation;
    [SerializeField] private CameraShaker mainCameraShake;
    [SerializeField] private CameraShakeInstance cameraShakeInstance;
    [SerializeField] private int currentHealth;
    [SerializeField] private GameObject[] healthIcons;
    [SerializeField] private BulletColor currentColor;
    [SerializeField] private int maxColorIndex;
    [SerializeField] private GameObject destroyParticles;
    [SerializeField] private ParticleSystem reloadParticles;
    [SerializeField] private ParticleSystem reloadSubParticles;
    [SerializeField] private ParticleSystem hasBulletParticles;
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private Boss boss;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private float safeZone;

    [Header("Sounds")]
    [SerializeField] private AudioClip getDamageClip;

    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip reloadClip;
    [SerializeField] private AudioClip changeColorClip;
    [SerializeField] private AudioClip lastHitClip;

    public bool isWhite = true;

    private Animator animator;
    private Rigidbody2D rigidbody;
    private PlayerMovement playerMovement;
    private AudioSource audioSource;
    private int maxHealth;
    private bool hasBullet;
    private bool reloading;
    private bool bulletReloaded;
    private bool reloadFinished;
    private SpriteRenderer sprite;
    private bool inmortalityMode;
    private float savedDirectionY;
    private float savedDirectionX;

    #endregion variables

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        maxHealth = healthIcons.Length;
        currentHealth = maxHealth;
        hasBullet = false;
        bulletReloaded = false;
        reloading = false;
        reloadFinished = false;
        //currentColor = (int) currentColor;
        inmortalityMode = false;
        AttackIncoming = false;
    }

    public void GetWhite()
    {
        sprite.color = Color.yellow;
        this.enabled = false;
    }

    private void Start()
    {
        if(!isWhite) {
            Color color = GameManager.instance.colors[(int)currentColor];
            sprite.color = color;

            ParticleSystem.MainModule bulletParticleSettings = hasBulletParticles.main;
            bulletParticleSettings.startColor =
            new ParticleSystem.MinMaxGradient(GameManager.instance.colors[(int)currentColor]);

            color = GameManager.instance.colors[(int)currentColor];
            color.a = 0.3f;

            ParticleSystem.MainModule reloadParticleSettings = reloadParticles.main;
            reloadParticleSettings.startColor = new ParticleSystem.MinMaxGradient(color);

            ParticleSystem.MainModule reloadSubParticleSettings = reloadSubParticles.main;
            reloadSubParticleSettings.startColor = new ParticleSystem.MinMaxGradient(color);
        }

        enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if(inmortalityMode)
            return;

        // [Space] - Shoot
        if(Input.GetKeyDown(KeyCode.Space)) {
            if(hasBullet && bulletReloaded) {
                Fire();
            }
        }

        // [Z] - Reload
        if(Input.GetKey(KeyCode.Z)) {
            if(!reloading && !hasBullet) {
                Reload();
            }
        }
        if(Input.GetKeyUp(KeyCode.Z) && !reloadFinished) {
            reloadParticles.Stop();
            reloadParticles.Clear();
            audioSource.Stop();
            CancelInvoke();
            ReactivateMovement();
            bulletReloaded = false;
        }

        // [X] - Change color
        if(Input.GetKeyDown(KeyCode.X)) {
            if(maxColorIndex != 0) {
                ChangeColor();
                hasBullet = false;
                bulletReloaded = false;
            }
        }
    }

    [Task]
    private void DummyTask()
    {
        print("Dummy task");
        Task.current.Succeed();
    }

    [Task]
    private void SameColor()
    {
        if(AttackIncoming) {
            Task.current.Fail();
        }

        playerMovement.MovementVector = Vector2.zero;

        if(currentColor == boss.GetCurrentColor()) {
            Task.current.Succeed();
        }
        else {
            ChangeColor();
        }
    }

    private void ChangeColor()
    {
        hasBulletParticles.Stop();
        hasBulletParticles.Clear();
        audioSource.clip = changeColorClip;
        audioSource.Play();

        currentColor++;
        if((int)currentColor > maxColorIndex) {
            currentColor = 0;
        }

        Tint((int)currentColor);
        Task.current.Succeed();
    }

    [Task]
    private void Reload()
    {
        if(AttackIncoming) {
            reloadParticles.Stop();
            reloadParticles.Clear();
            audioSource.Stop();
            CancelInvoke();
            ReactivateMovement();
            bulletReloaded = false;
            Task.current.Fail();
        }
        if(hasBullet) {
            Task.current.Succeed();
        }

        if(Task.current.isStarting) {
            audioSource.clip = reloadClip;
            audioSource.Play();
            reloading = true;
            reloadFinished = false;
            reloadParticles.Play();
            playerMovement.enabled = false;
            Invoke("ReactivateMovement", reloadParticles.main.duration);
            Invoke("ReloadFinished", reloadParticles.main.duration);
        }
    }

    [Task]
    private void Fire()
    {
        if(!hasBullet) {
            Task.current.Succeed();
        }

        if(Task.current.isStarting) {
            hasBulletParticles.Stop();
            hasBulletParticles.Clear();
            audioSource.clip = shootClip;
            audioSource.Play();

            if(currentColor >= 0) {
                GameObject bulletObject =
                    Instantiate(bullet, bulletSpawner.transform.position, bulletSpawner.transform.rotation);
                hasBullet = false;
                bulletReloaded = false;

                if(bulletObject.GetComponent<Bullet>() != null) {
                    Vector3 direction = (playerMovement.GetLookAtPosition() - bulletObject.transform.position).normalized;
                    bulletObject.GetComponent<Bullet>().Init(direction, currentColor);
                }
            }
        }
    }

    [Task]
    private void Dodge()
    {
        CalculateDodge();

        if(!AttackIncoming) {
            Task.current.Succeed();
        }
    }

    [Task]
    private void ReturnToCenter()
    {
        if(AttackIncoming) {
            Task.current.Fail();
            //print("FAIL");
        }
        if(Vector2.Distance(centerPoint.position, transform.position) < safeZone) {
            Task.current.Succeed();
            //print("SUCEED");
        }

        print("Distancia: " + Vector2.Distance(centerPoint.position, transform.position));
        Vector2 direction = (centerPoint.position - transform.position).normalized;
        playerMovement.MovementVector = direction;
    }

    private void CalculateDodge()
    {
        Vector2 direction = (AttackingObject.transform.position - transform.position).normalized;
        float randomValue = -1;

        if(Task.current.isStarting) {
            randomValue = Random.value;
        }

        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) { // Comes from LEFT or RIGHT
            if(randomValue != -1) {
                direction.y = (Random.value > 0.5) ? 0.7f : -0.7f;
                savedDirectionY = direction.y;
            }
            else {
                direction.y = savedDirectionY;
            }
            //direction.y -= 0.7f;
        }
        else { // Comes from TOP or BOTTOM
            if(randomValue != -1) {
                direction.x = (Random.value > 0.5) ? 0.7f : -0.7f;
                savedDirectionX = direction.x;
            }
            else {
                direction.x = savedDirectionX;
            }
            //direction.x += 0.7f;
        }

        //print("Random value: " + randomValue);
        //print("Direction: " + (-direction));
        playerMovement.MovementVector = -direction;
    }

    private void Tint(int colorIndex)
    {
        if(colorIndex < 4) {
            sprite.color = GameManager.instance.colors[colorIndex];

            ParticleSystem.MainModule bulletParticleSettings = hasBulletParticles.main;
            bulletParticleSettings.startColor =
             new ParticleSystem.MinMaxGradient(GameManager.instance.colors[colorIndex]);
            ParticleSystem.MainModule reloadParticleSettings = reloadParticles.main;
            Color color = GameManager.instance.colors[colorIndex];
            color.a = 0.3f;
            reloadParticleSettings.startColor = new ParticleSystem.MinMaxGradient(color);
            ParticleSystem.MainModule reloadSubParticleSettings = reloadSubParticles.main;
            reloadSubParticleSettings.startColor = new ParticleSystem.MinMaxGradient(color);
        }
    }

    private void ReactivateMovement()
    {
        playerMovement.enabled = true;
        bulletReloaded = true;
        reloading = false;
    }

    private void ReloadFinished()
    {
        hasBulletParticles.Play();
        reloadFinished = true;
        hasBullet = true;
    }

    public void GetDamage()
    {
        circleCollider.enabled = false;
        mainCameraShake.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime);
        rigidbody.velocity = Vector2.zero;
        currentHealth--;
        if(currentHealth < 0)
            currentHealth = 0;
        healthIcons[currentHealth].SetActive(false);

        if(currentHealth == 0) {
            audioSource.Stop();
            audioSource.clip = lastHitClip;
            audioSource.Play();
            playerMovement.enabled = false;
            destroyParticles.SetActive(true);
            sprite.enabled = false;

            Destroy(this.gameObject, damagedAnimation.length);
            GameEventSystem.instance.RestartScreen();
        }
        else {
            animator.SetTrigger("isDamaged");
            audioSource.Stop();
            audioSource.clip = getDamageClip;
            audioSource.Play();
        }
    }

    public void PassToNextLevel()
    {
        maxColorIndex++;
        currentColor = (BulletColor)maxColorIndex;
        Tint(maxColorIndex);
    }

    public void InmortalityMode()
    {
        playerMovement.enabled = false;
        circleCollider.enabled = false;
        inmortalityMode = true;
    }
}