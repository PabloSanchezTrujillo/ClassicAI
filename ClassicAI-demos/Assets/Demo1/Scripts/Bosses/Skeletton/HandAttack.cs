using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HandAttack : MonoBehaviour
{
    public Transform[] leftSidePoints;
    
    public Transform[] rightSidePoints;

    public GameObject handPrefab;

    private Hand leftHand;
    private Hand rightHand;

    [SerializeField] private int numAttacks = 2;

    private Transform[] originPointsForTheLeftHand;
    private Transform[] destinyPointsForTheLeftHand;
    
    private Transform[] originPointsForTheRightHand;
    private Transform[] destinyPointsForTheRightHand;

    [SerializeField] private float waitingSecondsOnPoint = 0.5f;
    [SerializeField] private float timeToGoToSidePoint = 1.0f;
    [SerializeField] private float timeCrossScreen = 1.0f;
    
    private bool isAttacking = true;

    private Collider2D leftCollider;
    private Collider2D rightCollider;
    
    public void Init(int numAttacks)
    {
        leftHand = InitOneHand(-1, false);
        rightHand = InitOneHand(1, true);

        this.numAttacks = numAttacks;
        this.numAttacks--;
    }
    
    private Hand InitOneHand(int facing, bool isRight)
    {
        GameObject handObject = Instantiate(handPrefab, transform);
        Hand hand = handObject.GetComponent<Hand>();
        

        if (isRight)
        {
            originPointsForTheRightHand = rightSidePoints;
            destinyPointsForTheRightHand = leftSidePoints;
            int index = PickIndexOfArray();
        
            hand.Init(originPointsForTheRightHand[index].position, 
                destinyPointsForTheRightHand[index].position, facing, 
                waitingSecondsOnPoint, timeToGoToSidePoint, timeCrossScreen);
            hand.ChangeFacing();
            rightCollider = handObject.GetComponent<Collider2D>();
            
        }
        else
        {
            originPointsForTheLeftHand = leftSidePoints;
            destinyPointsForTheLeftHand = rightSidePoints;
            int index = PickIndexOfArray();
        
            hand.Init(originPointsForTheLeftHand[index].position, 
                destinyPointsForTheLeftHand[index].position, facing,
                waitingSecondsOnPoint,timeToGoToSidePoint, timeCrossScreen);
            leftCollider = handObject.GetComponent<Collider2D>();
            
        }
        
        return hand;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (isAttacking && rightHand.hasFinishedAttack && leftHand.hasFinishedAttack)
        {
            if (numAttacks > 0)
            {
                Attack();
                numAttacks--;
            }
            else
            {
                isAttacking = false;
                rightCollider.enabled = false;
                leftCollider.enabled = false;
                rightHand.RetreatTo(transform.position);
                leftHand.RetreatTo(transform.position);
                StartCoroutine(DelayDestroy());
            }

        }
    }

    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(timeToGoToSidePoint);
        Destroy(gameObject);
    }
    
    private void OnDestroy()
    {
        Destroy(rightHand);
        Destroy(leftHand);
    }

    private void Attack()
    {

        Transform[] auxOrigin = originPointsForTheLeftHand;
        originPointsForTheLeftHand = originPointsForTheRightHand;
        originPointsForTheRightHand = auxOrigin;

        Transform[] auxDestiny = destinyPointsForTheLeftHand;
        destinyPointsForTheLeftHand = destinyPointsForTheRightHand;
        destinyPointsForTheRightHand = auxDestiny;
        
        int indexLeft = PickIndexOfArray();
        leftHand.Attack(originPointsForTheLeftHand[indexLeft].position, 
            destinyPointsForTheLeftHand[indexLeft].position);
        
        int indexRight = PickIndexOfArray();
        rightHand.Attack(originPointsForTheRightHand[indexRight].position, 
            destinyPointsForTheRightHand[indexRight].position);
        
        rightCollider.enabled = true;
        leftCollider.enabled = true;
    }
    
    private int PickIndexOfArray()
    {
        return Random.Range(0, 3);
    }
    
}
