using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAttack : MonoBehaviour
{
    #region In Editor Variables

    private Transform targetPlayer;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private AnimationClip blinkAnimation;
    [SerializeField] private AnimationClip IdleAnimation;
    [SerializeField] private GameObject[] cannons;
    [SerializeField] private GameObject[] lasers;
    [SerializeField] private Transform[] cannon_pos;
    private Dissolver[][] dissolvers;

    #endregion

    private Animator[] lasers_anim;
    private LerpMovement[] cannons_lerp;
    private AudioSource[] lasers_sound;
    private bool inPosition;
    private bool fixedTarget;

    private void Awake()
    {
        lasers_anim = new Animator[lasers.Length];
        cannons_lerp = new LerpMovement[cannons.Length];
        dissolvers = new Dissolver[cannons.Length][];
        lasers_sound = new AudioSource[lasers.Length];
        targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;

    }
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < cannons.Length; i++)
        {
            cannons_lerp[i] = cannons[i].GetComponent<LerpMovement>();
            lasers_anim[i] = lasers[i].GetComponent<Animator>();
            dissolvers[i] = cannons[i].GetComponentsInChildren<Dissolver>();
            lasers_sound[i] = lasers[i].GetComponent<AudioSource>();
           lasers[i].GetComponent<PolygonCollider2D>().enabled = false;
            lasers[i].SetActive(false);
        }

        fixedTarget = false;

        for (int i = 0; i < cannons.Length; i++)
            StartCoroutine(CannonAttack(i));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (fixedTarget == false)
        {
            for (int i = 0; i < cannons.Length; i++)
            {
                if (cannons_lerp[i].hasFinishedLerping)
                    CannonLookAt(cannons[i]);                 
            }
        }
    }

    private void CannonLookAt(GameObject cannon)
    {
        Vector2 direction = targetPlayer.position - cannon.transform.position;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
        cannon.transform.rotation = Quaternion.Slerp(cannon.transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    private IEnumerator CannonAttack(int id)
    {
        cannons_lerp[id].StartLerping(cannons[id].transform.position, cannon_pos[id].position, 1.0f);

        // Wait for taking position
        yield return new WaitUntil(() => cannons_lerp[id].hasFinishedLerping);

        // Wait for aiming
        yield return new WaitForSeconds(2);

        //Activate blinking
        fixedTarget = true;
        lasers[id].SetActive(true);
        lasers_anim[id].SetTrigger("FixedTarget");
        PlayLaserSound(id);

        // Wait for animation end
        yield return new WaitForSeconds(blinkAnimation.length + IdleAnimation.length);

        // End Attack
        lasers[id].SetActive(false);
        //cannons_lerp[id].StartLerping(cannons[id].transform.position, cannons[id].transform.parent.position, 1.0f);
        foreach (Dissolver dissolver in dissolvers[id])
        {
            dissolver.Dissolve();
        }

        yield return new WaitForSeconds(1.0f);

        Destroy(this.gameObject);

    }

    private void PlayLaserSound(int id)
    {
        lasers_sound[id].Play();
    }
}
