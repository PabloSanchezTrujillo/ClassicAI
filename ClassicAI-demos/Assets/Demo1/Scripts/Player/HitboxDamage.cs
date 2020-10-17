using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    #region variables

    [SerializeField] private PlayerActions playerActions;
    [SerializeField] private AnimationClip damagedAnimation;

    private CircleCollider2D hitbox;

    #endregion variables

    private void Awake()
    {
        hitbox = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Damage")) {
            playerActions.GetDamage();
            StartCoroutine(ReactivateCollider());
        }
    }

    private IEnumerator ReactivateCollider()
    {
        yield return new WaitForSeconds(damagedAnimation.length + 0.2f);
        hitbox.enabled = true;
    }
}