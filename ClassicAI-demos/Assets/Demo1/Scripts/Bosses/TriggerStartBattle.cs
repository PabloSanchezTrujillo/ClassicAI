using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class TriggerStartBattle : MonoBehaviour
{
    [SerializeField] private GameObject boss;

    [SerializeField] private GameObject cameraGameplay;
    [SerializeField] private GameObject hall;
    [SerializeField] private GameObject colliderOfTheHallEntrance;

    [SerializeField] private float timeToMakeThePlayerWait = 1.0f;

    private PlayerMovement playerMovement;
    private PlayerActions playerActions;
    private LookAtTarget lookAtTarget;
    private PandaBehaviour playerBehaviourTree;

    [SerializeField] private GameObject newLookAtObject;
    [SerializeField] private bool returnPlayerActions = true;

    [SerializeField] private float timeBossToAppear = 0.0f;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        GameObject player = other.gameObject;

        if(player.tag.Equals("Player")) {
            cameraGameplay.SetActive(true);
            if(boss) {
                if(timeBossToAppear == 0.0f)
                    boss.SetActive(true);
                else {
                    StartCoroutine(ShowBoss());
                }
            }

            hall.SetActive(false);
            colliderOfTheHallEntrance.SetActive(true);

            if(timeToMakeThePlayerWait > 0.0f) {
                playerMovement = player.GetComponent<PlayerMovement>();
                playerActions = player.GetComponent<PlayerActions>();
                lookAtTarget = player.GetComponent<LookAtTarget>();
                playerBehaviourTree = player.GetComponent<PandaBehaviour>();
                playerMovement.enabled = false;
                playerActions.enabled = false;
                StartCoroutine(StartFight());
            }
        }
    }

    private IEnumerator ShowBoss()
    {
        yield return new WaitForSeconds(timeBossToAppear);
        boss.SetActive(true);
    }

    private IEnumerator StartFight()
    {
        yield return new WaitForSeconds(timeToMakeThePlayerWait + timeBossToAppear);
        playerMovement.enabled = true;
        playerBehaviourTree.enabled = true;
        if(returnPlayerActions)
            playerActions.enabled = true;
        lookAtTarget.enabled = true;
        lookAtTarget.target = newLookAtObject.transform;
        Destroy(gameObject);
    }
}