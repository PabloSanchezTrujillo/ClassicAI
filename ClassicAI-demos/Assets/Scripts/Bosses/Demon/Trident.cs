using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Trident : MonoBehaviour
{
    #region variables

    [SerializeField] private float strengthOfShaking;
    [SerializeField] private float rangeOfShaking;
    [SerializeField] private float speedOfShaking;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float timeToShoot;
    [SerializeField] private ParticleSystem shootingParticles;

    private Rigidbody2D rigidbody;
    private Vector3 firstPosition;
    private bool shaking;
    private AudioSource audioSource;

    #endregion variables

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if(shaking) {
            Shaking();
        }
    }

    public void StartShaking()
    {
        shootingParticles.Stop();
        shootingParticles.Clear();
        rigidbody.velocity = Vector2.zero;
        firstPosition = transform.position;
        shaking = true;

        Invoke("Shoot", timeToShoot);
    }

    private void Shaking()
    {
        float randomX = Random.Range(-strengthOfShaking, strengthOfShaking);

        rigidbody.velocity += new Vector2(0, speedOfShaking) * Time.deltaTime;
        float clampPositionX = Mathf.Clamp(transform.position.x, firstPosition.x - rangeOfShaking, firstPosition.x + rangeOfShaking);
        transform.position.Set(clampPositionX, transform.position.y, transform.position.z);
    }

    private void Shoot()
    {
        shaking = false;
        rigidbody.velocity = Vector2.zero;
        shootingParticles.Play();
        audioSource.Play();

        rigidbody.velocity = new Vector2(0, 1) * movementSpeed * Time.deltaTime;

        Invoke("ReturnToPool", 2);
    }

    private void ReturnToPool()
    {
        rigidbody.velocity = Vector2.zero;
        shootingParticles.Stop();
        shootingParticles.Clear();
    }
}