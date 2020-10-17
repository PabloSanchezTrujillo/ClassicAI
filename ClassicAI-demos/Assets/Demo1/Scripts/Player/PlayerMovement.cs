using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class PlayerMovement : MonoBehaviour
{
    #region variables

    public Vector2 MovementVector { get; set; }

    [SerializeField] private Transform lookAtTarget;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float timeToStartBattle;

    private Rigidbody2D rigidbody;
    private PandaBehaviour playerBehaviour;
    private PlayerActions playerActions;

    #endregion variables

    // Start is called before the first frame update
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerActions = GetComponent<PlayerActions>();
        playerBehaviour = GetComponent<PandaBehaviour>();
    }

    private void Start()
    {
        StartCoroutine(StartBattle());
    }

    private IEnumerator StartBattle()
    {
        yield return new WaitForSeconds(timeToStartBattle);

        playerBehaviour.enabled = true;
        playerActions.enabled = true;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        float Y = Input.GetAxis("Vertical");
        float X = Input.GetAxis("Horizontal");

        rigidbody.velocity += MovementVector * movementSpeed * Time.deltaTime;
    }

    public Vector3 GetLookAtPosition()
    {
        return lookAtTarget.position;
    }
}