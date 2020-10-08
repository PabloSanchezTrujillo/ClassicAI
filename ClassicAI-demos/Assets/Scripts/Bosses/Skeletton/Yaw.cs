using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yaw : MonoBehaviour
{

    private bool goingDown = false;
    public bool isAttacking = false;
    
    [SerializeField] private float distanceDown;

    private Vector2 endPosition;
    private Vector2 startPosition;
    
    private LerpMovement lerper;

    [SerializeField]
    private float timeGoingDown;

    [SerializeField] private float timeGoingUp;

    [SerializeField] private float timeToRecoverYaw;

    private void Awake()
    {
        lerper = GetComponent<LerpMovement>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (lerper.hasFinishedLerping)
        {
            if (goingDown)
            {
                StartCoroutine(GoingUp());
            }
            else
            {
                isAttacking = false;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    public void Attack()
    {
        if (!isAttacking)
        {
            goingDown = true;
            isAttacking = true;
            startPosition = transform.position;
            endPosition = startPosition + (Vector2) Vector3.down * distanceDown;
            lerper.StartLerping(startPosition, endPosition, timeGoingDown);
        }
    }

    IEnumerator GoingUp()
    {
        yield return new WaitForSeconds(timeToRecoverYaw);
        goingDown = false;
        lerper.StartLerping(endPosition, startPosition, timeGoingUp);
        //isAttacking = false;
    }
}
