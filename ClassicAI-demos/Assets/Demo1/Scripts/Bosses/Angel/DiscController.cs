using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscController : MonoBehaviour
{
    [SerializeField] private GameObject[] big_discs;
    [SerializeField] private GameObject[] little_discs;
    [SerializeField] private float angle_separation;
    [SerializeField] private AnimationClip shake_anim;

    private int big_disc_count;
    private int little_disc_count;
    private Animator[] disc_anims;
    private Dissolver[] dissolvers;

    void Awake()
    {
        big_disc_count = big_discs.Length;
        little_disc_count = little_discs.Length;
        disc_anims = new Animator[big_disc_count];
        dissolvers = new Dissolver[big_disc_count];
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < little_disc_count; i++)
        {
            little_discs[i].SetActive(false);
        }

        for (int i = 0; i < big_disc_count; i++)
        {
            dissolvers[i] = big_discs[i].GetComponent<Dissolver>();
            disc_anims[i] = big_discs[i].GetComponent<Animator>();
            big_discs[i].SetActive(false);
        }

        StartCoroutine(DiscAttack());
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private IEnumerator DiscAttack()
    {
        float angle_range = (180 - 2 * big_disc_count * angle_separation) / big_disc_count;
        float angle_offset = 180 + angle_separation;
        float angle_inc = angle_range + 2 * angle_separation;
        Vector2 dir;
        float angle;
        for (int i = 0; i < big_disc_count; i++)
        {
            // Compute random direction
            angle = angle_offset + Random.Range(0, angle_range);
            dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            big_discs[i].SetActive(true);
            big_discs[i].GetComponent<Disc>().direction = dir;
            big_discs[i].GetComponent<Disc>().able_to_move = true;
            angle_offset += angle_inc;
        }

        yield return new WaitForSeconds(2.0f);


        for (int i = 0; i < big_disc_count; i++)
        {
            disc_anims[i].SetTrigger("StartToShake");
        }

        yield return new WaitForSeconds(shake_anim.length + 0.5f);

        float radius = 1.0f;
        for (int i = 0; i < little_disc_count; i++)
        {
            float discCrossOffset = i >= little_disc_count / big_disc_count && i < 2 * (little_disc_count / big_disc_count) ? Mathf.PI * 0.25f : 0f;

            angle = discCrossOffset + i * Mathf.PI * 2f / (little_disc_count / big_disc_count);
            dir =  new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            little_discs[i].GetComponent<Disc>().direction = dir;
            little_discs[i].GetComponent<Disc>().able_to_move = true;
            little_discs[i].SetActive(true);
        }

        for (int i = 0; i < big_disc_count; i++)
        {
            // Play dissolve anim
            dissolvers[i].Dissolve();
            big_discs[i].GetComponent<PolygonCollider2D>().enabled = false;
        }

        yield return new WaitForSeconds(5);

        Destroy(this.gameObject);

        yield return null;
    }
}
