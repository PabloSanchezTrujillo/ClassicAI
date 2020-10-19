using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sucker : MonoBehaviour
{
    [SerializeField] private GameObject boulder;

    [SerializeField] private GameObject player;

    [SerializeField] private Transform center;

    [Header("Sucking Attack")]
    [SerializeField] private float proyectilesSuckingSpeed = 3;

    [SerializeField] private float suckingAttackDuration = 6;
    [SerializeField] private int numberOfProjectiles;

    [Header("Orbits Attack")]
    [SerializeField] private float blackholeOffset = 5;

    [SerializeField] private float blackholeScalingDuration = 1f;

    [SerializeField] private float blackholeScalingFactor = 10;

    [Space]
    [SerializeField] private List<float> orbitsRadius = new List<float>() { 12, 15.5f, 19 };

    [SerializeField] private List<float> orbitalsSpeed = new List<float>() { 0.5f, 0.4f, 0.3f };

    [Space]
    [SerializeField] private float orbitsAttackDuration = 20;

    [Header("Bouncers Attack")]
    [SerializeField] private float proyectilesBouncersSpeed = 10;

    [Space]
    [SerializeField] private float bouncersAttackDuration = 15;

    private Vector3 initPos;

    private float adjustedOffset = 2;

    private float targetScale;

    private bool scalingActive;

    private float currentScale;

    private float invertingFactor = 1;

    private List<Boulder> orbitingBoulders = new List<Boulder>();
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        currentScale = this.transform.localScale.x;

        initPos = center.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if(player) {
            if(player.transform.position.x > center.position.x) {
                adjustedOffset = -blackholeOffset;
            }
            else {
                adjustedOffset = blackholeOffset;
            }
        }

        if(scalingActive) {
            if(invertingFactor == 1 ? currentScale < targetScale : currentScale > targetScale) {
                currentScale += invertingFactor * Time.deltaTime * blackholeScalingFactor / blackholeScalingDuration;
                center.localScale = new Vector3(currentScale, currentScale, 1);
            }
            else {
                center.localScale = new Vector3(targetScale, targetScale, 1);
                scalingActive = false;
            }
        }
    }

    public void SuckingAttack()
    {
        StartCoroutine(SuckingAttackWithWait());
    }

    public void OrbitsAttack()
    {
        // Spawn orbitals
        StartCoroutine(OrbitalsAttackWithWait());
    }

    public void BouncesAttack()
    {
        // Throw Boulders
        StartCoroutine(BouncersAttackWithWait());
    }

    public IEnumerator SuckingAttackWithWait()
    {
        animator.SetTrigger("suckingAttack");

        SpawnCircle(15, numberOfProjectiles, proyectilesSuckingSpeed);

        yield return new WaitForSeconds(suckingAttackDuration / 30);

        SpawnCircle(15, numberOfProjectiles, proyectilesSuckingSpeed, 0.6f);

        yield return new WaitForSeconds(suckingAttackDuration / 15);

        SpawnCircle(15, numberOfProjectiles, proyectilesSuckingSpeed + 2);

        yield return new WaitForSeconds(suckingAttackDuration / 15);

        SpawnCircle(15, numberOfProjectiles, proyectilesSuckingSpeed, 0.3f);

        yield return new WaitForSeconds(suckingAttackDuration / 30);

        SpawnCircle(15, numberOfProjectiles, proyectilesSuckingSpeed);

        yield return new WaitForSeconds(4 * suckingAttackDuration / 5);
    }

    public IEnumerator OrbitalsAttackWithWait()
    {
        animator.SetTrigger("orbitsAttack");
        float degrees90 = Mathf.PI / 2;

        // Scale up and move to the left or right
        BlackHoleTransition(blackholeScalingFactor, 1, true, center.position + new Vector3(adjustedOffset, 0, 0));

        float firstTime = Time.time;
        // Wait for the sucker to be big and offset
        yield return new WaitForSeconds(blackholeScalingDuration);

        // Spawn orbitals
        int NWaves = 3;

        for(int i = 0; i < NWaves - 1; i++) {
            // First, second ... and n waves
            SpawnOrbit(orbitsRadius[0], 1, -orbitalsSpeed[0] * -Mathf.Sign(adjustedOffset), degrees90);
            yield return new WaitForSeconds(0.5f);

            //SpawnOrbit(orbitals[1], 1, orbitalSpeeds[1] * -Mathf.Sign(adjustedOffset), -degrees90);
            //yield return new WaitForSeconds(0.5f);

            SpawnOrbit(orbitsRadius[1], 1, -orbitalsSpeed[1] * -Mathf.Sign(adjustedOffset), degrees90);
            SpawnOrbit(orbitsRadius[2], 2, orbitalsSpeed[2] * -Mathf.Sign(adjustedOffset), degrees90);
            yield return new WaitForSeconds(0.5f);

            //SpawnOrbit(orbitals[0], 1, -orbitalSpeeds[0] * -Mathf.Sign(adjustedOffset), degrees90);
            //yield return new WaitForSeconds(0.5f);

            //SpawnOrbit(orbitals[2], 1, orbitalSpeeds[2] * -Mathf.Sign(adjustedOffset), -degrees90);
            //yield return new WaitForSeconds(0.5f);

            SpawnOrbit(orbitsRadius[0], 1, orbitalsSpeed[0] * -Mathf.Sign(adjustedOffset), -degrees90);
            SpawnOrbit(orbitsRadius[1], 2, orbitalsSpeed[1] * -Mathf.Sign(adjustedOffset), degrees90);

            // Wait for next wave
            yield return new WaitForSeconds(orbitsAttackDuration / NWaves);
        }

        // Last wave with no new spawns
        yield return new WaitForSeconds(orbitsAttackDuration / NWaves);

        // Back to normal position
        BlackHoleTransition(1, -1, true, initPos);

        // Destroy boulders
        foreach(Boulder boulder in orbitingBoulders) {
            boulder.DestroyWithFade();
        }
        orbitingBoulders.Clear();

        yield return new WaitForSeconds(blackholeScalingDuration);

        float lastTime = Time.time;
        print("Tiempo: " + (lastTime - firstTime));
    }

    public IEnumerator BouncersAttackWithWait()
    {
        animator.SetTrigger("bouncersAttack");
        // Throw Bouncers
        int NWaves = 3;

        for(int i = 0; i < NWaves - 1; i++) {
            SpawnBouncer(UnityEngine.Random.Range(-25, 25), proyectilesBouncersSpeed);

            yield return new WaitForSeconds(0.2f);

            SpawnBouncer(UnityEngine.Random.Range(65, 115), proyectilesBouncersSpeed);

            yield return new WaitForSeconds(0.2f);

            SpawnBouncer(UnityEngine.Random.Range(155, 205), proyectilesBouncersSpeed);

            yield return new WaitForSeconds(0.2f);

            SpawnBouncer(UnityEngine.Random.Range(245, 295), proyectilesBouncersSpeed);

            // Wait for the second wave
            yield return new WaitForSeconds(bouncersAttackDuration / NWaves);
        }

        // Last round with no spawns
        yield return new WaitForSeconds(bouncersAttackDuration / NWaves);

        // Destroy boulders
        foreach(Boulder boulder in orbitingBoulders) {
            boulder.DestroyWithFade();
        }
        orbitingBoulders.Clear();
    }

    private void SpawnCircle(float radius, int NBoulders, float boulderSpeed, float initialAngleOffset = 0f)
    {
        if(NBoulders == 0)
            return;

        // Radianes
        float angle = 2 * Mathf.PI / NBoulders;

        for(int i = 0; i < NBoulders; i++) {
            float singleAngle = angle * i + initialAngleOffset;
            Vector3 position = new Vector3(center.position.x + Mathf.Cos(singleAngle) * radius, center.position.y + Mathf.Sin(singleAngle) * radius, center.position.z);

            GameObject newBoulder = Instantiate(boulder, position, new Quaternion());
            newBoulder.GetComponent<Boulder>().InitSuck(center, boulderSpeed);
        }
    }

    private void BlackHoleTransition(float targetScale, float invertFactor, bool scalingActive, Vector3 nextPos)
    {
        this.targetScale = targetScale;
        invertingFactor = invertFactor;
        this.scalingActive = scalingActive;

        // Move to the given position
        center.gameObject.GetComponent<LerpMovement>().StartLerping(center.position, nextPos, blackholeScalingDuration);
    }

    private void SpawnOrbit(float radius, int NBoulders, float boulderSpeed, float initialAngleOffset = 0f)
    {
        if(NBoulders == 0)
            return;

        // Radianes
        float angle = 2 * Mathf.PI / NBoulders;

        for(int i = 0; i < NBoulders; i++) {
            float singleAngle = angle * i + initialAngleOffset;
            Vector3 position = new Vector3(center.position.x + Mathf.Cos(singleAngle) * radius, center.position.y + Mathf.Sin(singleAngle) * radius, center.position.z);

            GameObject newBoulder = Instantiate(boulder, position, new Quaternion());
            newBoulder.GetComponent<Boulder>().InitOrbit(center, boulderSpeed);

            orbitingBoulders.Add(newBoulder.GetComponent<Boulder>());
        }
    }

    private void SpawnBouncer(float angle, float boulderSpeed)
    {
        GameObject newBoulder = Instantiate(boulder, center.position, new Quaternion());
        newBoulder.GetComponent<Boulder>().InitBouncer(center, angle, boulderSpeed);

        orbitingBoulders.Add(newBoulder.GetComponent<Boulder>());
    }
}