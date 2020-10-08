using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathBall : MonoBehaviour
{
    #region variables

    [SerializeField] private GameObject player;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private ParticleSystem trailParticles;
    [SerializeField] private Transform spawnPoint;

    private Rigidbody2D rigidbody;
    private AudioSource audioSource;
    private bool notShooted;

    #endregion variables

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        notShooted = false;
    }

    private void Update()
    {
        transform.rotation *= Quaternion.Euler(0, 0, rotationSpeed);

        if(notShooted) {
            transform.position = spawnPoint.position;
        }
    }

    public void ShootDeathBall()
    {
        notShooted = false;
        rigidbody.velocity = Vector2.zero;
        Vector3 targetPosition = player.transform.position;
        Vector2 direction = LookAt(targetPosition);

        trailParticles.Play();
        audioSource.Play();
        rigidbody.velocity = direction * movementSpeed;

        Invoke("ResetDeathBall", 2);
    }

    private Vector2 LookAt(Vector3 lookAtTarget)
    {
        Vector2 direction = lookAtTarget - transform.position;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 100);

        return direction;
    }

    private void ResetDeathBall()
    {
        rigidbody.velocity = Vector2.zero;
        trailParticles.Stop();
        trailParticles.Clear();
    }

    public void SetNotShooted(bool notShooted)
    {
        this.notShooted = notShooted;
    }
}