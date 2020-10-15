using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VBullet : MonoBehaviour
{
    #region variables

    [SerializeField] private float movementSpeed;
    [SerializeField] private AnimationClip vBulletAttack;
    [SerializeField] private AudioClip VBulletShooted;
    [SerializeField] private AudioClip VBulletSmash;

    private Animator animator;
    private Rigidbody2D rigidbody;
    private AudioSource audioSource;
    private bool animationLaunched;

    #endregion variables

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        animationLaunched = false;
    }

    public void ShootVBullet()
    {
        rigidbody.velocity = new Vector2(0, -1) * movementSpeed;
        audioSource.clip = VBulletShooted;
        audioSource.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "AnimationTrigger" && !animationLaunched) {
            rigidbody.velocity = Vector2.zero;
            animationLaunched = true;
            animator.SetTrigger("Attack");
            audioSource.clip = VBulletSmash;
            audioSource.Play();

            Destroy(this.gameObject, vBulletAttack.length);
        }
    }
}