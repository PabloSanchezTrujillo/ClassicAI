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
    [SerializeField] private float dodgeAngle;

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

    /// <summary>
    /// Behaviour tree task - Goes through all the colors until matching the boss color
    /// </summary>
    [Task]
    private void SameColor()
    {
        // If an attack is coming cancels the action
        if(AttackIncoming) {
            Task.current.Fail();
        }

        // Disables the player movement during the color changing
        playerMovement.MovementVector = Vector2.zero;

        // Changes color until matching the boss color
        if(currentColor == boss.GetCurrentColor()) {
            Task.current.Succeed();
        }
        else {
            ChangeColor();
        }
    }

    /// <summary>
    /// Changes to the next color
    /// </summary>
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

    /// <summary>
    /// Behaviour tree task - Reloads the spaceship bullet
    /// </summary>
    [Task]
    private void Reload()
    {
        // If an attack is coming, cancels the action
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

        // Starts the reloading process when entering in this task
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

    /// <summary>
    /// Behaviour tree task - Fires the bullet
    /// </summary>
    [Task]
    private void Fire()
    {
        if(!hasBullet) {
            Task.current.Succeed();
        }

        // Fires the bullet when entering in this task
        if(Task.current.isStarting) {
            hasBulletParticles.Stop();
            hasBulletParticles.Clear();
            audioSource.clip = shootClip;
            audioSource.Play();

            // Checks that the spaceship is not in white color
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

    /// <summary>
    /// Behaviour tree task - Dodge danger action
    /// </summary>
    [Task]
    private void Dodge()
    {
        CalculateDodge();

        // The task succeeds when there are no more attacks incoming
        if(!AttackIncoming) {
            Task.current.Succeed();
        }
    }

    /// <summary>
    /// Behaviour tree task - Returns the spaceship to the center of the room after dodging an attack
    /// </summary>
    [Task]
    private void ReturnToCenter()
    {
        // Cancels the action if an attack is coming
        if(AttackIncoming) {
            Task.current.Fail();
        }

        // Completes the action when the spaceship reaches the safe zone
        if(Vector2.Distance(centerPoint.position, transform.position) < safeZone) {
            Task.current.Succeed();
        }

        print("Distancia: " + Vector2.Distance(centerPoint.position, transform.position));
        Vector2 direction = (centerPoint.position - transform.position).normalized;
        playerMovement.MovementVector = direction;
    }

    /// <summary>
    /// Calculates how to dodge the danger
    /// </summary>
    private void CalculateDodge()
    {
        Vector2 direction = (AttackingObject.transform.position - transform.position).normalized; // Direction where the danger is coming from
        Vector2 dodgeDirection = Vector2.zero;
        float randomValue = -1;

        if(Task.current.isStarting) {
            randomValue = Random.value;
        }

        /*
        // OLD VERSION
        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) { // Object comes from LEFT or RIGHT
            /*if(randomValue != -1) { // First dodge
                direction.y = (Random.value > 0.5) ? 0.7f : -0.7f;
                savedDirectionY = direction.y;
            }
            else {
                direction.y = savedDirectionY;
            }
        }
        else { // Object comes from TOP or BOTTOM
            /*if(randomValue != -1) { // First dodge
                direction.x = (Random.value > 0.5) ? 0.7f : -0.7f;
                savedDirectionX = direction.x;
            }
            else {
                direction.x = savedDirectionX;
            }
        }
        */

        // NEW VERSION
        if(direction.y > 0) { // Object comes from TOP
            if(direction.x > 0) { // Object comes from TOP-RIGHT
                print("Top-Right");
                // Dodging formula -> (x-sin(a), y-cos(a))
                dodgeDirection = new Vector2(direction.x - Mathf.Sin(-dodgeAngle), direction.y - Mathf.Cos(-dodgeAngle));
            }
            else { // Object comes from TOP-LEFT
                print("Top-Left");
                dodgeDirection = new Vector2(direction.x - Mathf.Sin(dodgeAngle), direction.y - Mathf.Cos(dodgeAngle));
            }
        }
        else { // Object comes from BOTTOM
            if(direction.x > 0) { // Object comes from BOTTOM-RIGHT
                print("Bottom-Right");
                dodgeDirection = new Vector2(direction.x - Mathf.Sin(-dodgeAngle), direction.y - Mathf.Cos(-dodgeAngle));
            }
            else { // Object comes from BOTTOM-LEFT
                print("Bottom-Left");
                dodgeDirection = new Vector2(direction.x - Mathf.Sin(dodgeAngle), direction.y - Mathf.Cos(dodgeAngle));
            }
        }

        //print("Random value: " + randomValue);
        //print("Direction: " + (direction));
        playerMovement.MovementVector = -dodgeDirection;
    }

    /// <summary>
    /// Tints the spaceship with the new color
    /// </summary>
    private void Tint(int colorIndex)
    {
        if(colorIndex < 4) {
            sprite.color = GameManager.instance.colors[colorIndex]; // New selected color

            // Changing the color to all the spacechip particles
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

    /// <summary>
    /// Reactivates the movement after the reloading action
    /// </summary>
    private void ReactivateMovement()
    {
        playerMovement.enabled = true;
        bulletReloaded = true;
        reloading = false;
    }

    /// <summary>
    /// Finishes the reloading action
    /// </summary>
    private void ReloadFinished()
    {
        hasBulletParticles.Play();
        reloadFinished = true;
        hasBullet = true;
    }

    /// <summary>
    /// The spaceship receives damage
    /// </summary>
    public void GetDamage()
    {
        // Decreases the spaceship lives
        circleCollider.enabled = false; // Disables the spaceship collider so it can have a brief inmortality time after each hit
        mainCameraShake.ShakeOnce(magnitude, roughness, fadeInTime, fadeOutTime); // Camera shake for each hit
        rigidbody.velocity = Vector2.zero;
        currentHealth--;
        if(currentHealth < 0)
            currentHealth = 0;
        healthIcons[currentHealth].SetActive(false);

        // When reaching 0 health the spaceship gets destroyed
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

    /// <summary>
    /// Unlocks the next color when passing to the next level
    /// </summary>
    public void PassToNextLevel()
    {
        maxColorIndex++;
        currentColor = (BulletColor)maxColorIndex;
        Tint(maxColorIndex);
    }

    /// <summary>
    /// Activates the inmortality mode in the spaceship but it cannot move
    /// </summary>
    public void InmortalityMode()
    {
        playerMovement.enabled = false;
        circleCollider.enabled = false;
        inmortalityMode = true;
    }
}