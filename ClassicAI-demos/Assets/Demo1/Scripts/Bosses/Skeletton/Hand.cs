using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Hand : MonoBehaviour
{

    [SerializeField]private int facing;

    private bool isMovingToPoint = false;
    private bool isAtacking = false;

    private LerpMovement lerper;

    private float timeToSidePoint;
    private float timeToCrossScreen;
    
    private float waitingSecondsOnSidePoint;

    public bool hasFinishedAttack = false;
    private Vector3 sidePointDestiny;

    private Vector3 originalPosition;

    private Collider2D collider;

    private SpriteRenderer renderer;

    public float alphaValue = 0.2f;
    
    private void Awake()
    {
        lerper = GetComponent<LerpMovement>();
        collider = GetComponent<Collider2D>();
        renderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
    }

    public void Init(Vector3 sidePointOrigin, Vector3 sidePointDestiny, int facing, float waitingSecondsOnSidePoint,
        float timeToSidePoint, float timeToCrossScreen)
    {
        
        this.facing = facing;
        this.timeToSidePoint = timeToSidePoint;
        this.timeToCrossScreen = timeToCrossScreen;
        this.waitingSecondsOnSidePoint = waitingSecondsOnSidePoint;
        ChangeFacing();
        Color colorHand = renderer.color;
        colorHand.a = alphaValue;
        renderer.color = colorHand;
        Attack(sidePointOrigin, sidePointDestiny);
    }

    public void ChangeFacing()
    {
        facing = facing * -1;
        float xScale = transform.localScale.x * -1;
        transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);
        float zRot = transform.localRotation.eulerAngles.z * -1;
        transform.localRotation = Quaternion.Euler(new Vector3(transform.localRotation.x, transform.localRotation.y, zRot));
        
    }

    public void Attack(Vector3 sidePointOrigin, Vector3 sidePointDestiny)
    {
        this.sidePointDestiny = sidePointDestiny;
        
        lerper.StartLerping(transform.position, sidePointOrigin, waitingSecondsOnSidePoint);
        isMovingToPoint = true;
        hasFinishedAttack = false;
        isAtacking = false;
    }

    public void RetreatTo(Vector3 retreatPos)
    {
        Color colorHand = renderer.color;
        colorHand.a = alphaValue;
        renderer.color = colorHand;
        lerper.StartLerping(transform.position, retreatPos, timeToSidePoint);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (isMovingToPoint)
        {
            if (lerper.hasFinishedLerping)
            {
                Color colorHand = renderer.color;
                colorHand.a = 1.0f;
                renderer.color = colorHand;
                StartCoroutine(WaitOnSidePoint());
            }
            
        }else if (isAtacking)
        {
            collider.enabled = true;
            if (lerper.hasFinishedLerping)
            {
                isAtacking = false;
                hasFinishedAttack = true;
                ChangeFacing();
            }
        }
    }

    IEnumerator WaitOnSidePoint()
    {
        //hacer animacion
        isMovingToPoint = false;
        yield return new WaitForSeconds(waitingSecondsOnSidePoint);
        //terminar animacion
        isAtacking = true;
        lerper.StartLerping(transform.position, sidePointDestiny, timeToCrossScreen );
    }
}
